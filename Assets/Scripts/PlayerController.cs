using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public float m_Speed = 12f;
    public float m_TurnSpeed = 10f;
    public float m_AnalogMinThreshold = 0.2f;
    public float m_GravityScale = 10f;
    public float m_KnockbackDistance = 5f;
    
    public float m_ShoveIncrement = 10f;
    private float m_MinShove = 2f;
    private float m_MaxShovePressure = 15f;
    
    private string m_AButtonName;
    private string m_BButtonName;
    private string m_JoyStickVerticalName;
    private string m_JoyStickHorizontalName;
    private float m_JoystickVertical;
    private float m_JoystickHorizontal;
    private float m_KnockbackCounter;
    private float m_KnockbackTime;
    private bool m_Knockback_State;
    private Vector3 m_Movement;
    private float m_ShovePressure;
    private float m_ShoveTime;
    private bool m_ShoveState;
    private CharacterController m_Controller;
    private IEnumerator m_CurrentCoroutine;

    void Awake()
    {
        m_Controller = GetComponent<CharacterController>();
    }
    
    void Start()
    {
        m_JoyStickHorizontalName = "JoystickHorizontal" + m_PlayerNumber;
        m_JoyStickVerticalName = "JoystickVertical" + m_PlayerNumber;
        m_AButtonName = "AButton" + m_PlayerNumber;
        m_BButtonName = "BButton" + m_PlayerNumber;
        m_ShovePressure = 0f;
        m_ShoveTime = 2f;
        m_KnockbackTime = 3f;
        m_Knockback_State = false;
        m_ShoveState = false;
        m_CurrentCoroutine = null;
    }

    void Update()
    {
        m_JoystickHorizontal = Input.GetAxis(m_JoyStickHorizontalName);
        m_JoystickVertical = Input.GetAxis(m_JoyStickVerticalName);
       
        if (!m_Knockback_State) {
            // if charging shove, wait shove and cant move
            // if not in shove dash mode, move
            Shove();
            if (m_ShovePressure <= 0f && !m_ShoveState) Move();
        }
        else {
            m_KnockbackCounter -= Time.deltaTime;
        }
        KnockbackDetection();
        m_Movement.y = m_Movement.y + (Physics.gravity.y * m_GravityScale * Time.deltaTime);
        m_Controller.Move(m_Movement);
        
    }

    private void Move()
    {
        Vector3 input = Vector3.zero;
        if (Mathf.Abs(m_JoystickHorizontal) > m_AnalogMinThreshold) input.x = m_JoystickHorizontal;
        if (Mathf.Abs(m_JoystickVertical) > m_AnalogMinThreshold) input.z = -m_JoystickVertical;
        
        m_Movement = input * m_Speed * Time.deltaTime;
        if (input != Vector3.zero) {
            Quaternion newRotation = Quaternion.LookRotation(input, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, m_TurnSpeed * Time.deltaTime);
        } 
    }

    private void Shove()
    {
        if (Input.GetButton(m_AButtonName)) {
            if (m_ShovePressure < m_MaxShovePressure) {
                m_ShovePressure += m_ShoveIncrement * Time.deltaTime;
            }
            else {
                m_ShovePressure = m_MaxShovePressure;
            }
        }
        else {
            if (m_ShovePressure > 0f) {
                m_ShovePressure += m_MinShove;
                Vector3 shoveEnd = transform.position + transform.forward * m_ShovePressure;
                if (!m_ShoveState) {
                    m_ShoveState = true;
                    m_CurrentCoroutine = MovementLoop(transform.position, shoveEnd, m_ShoveTime);
                    StartCoroutine(m_CurrentCoroutine);
                    m_ShoveState = false;
                }
                m_ShovePressure = 0f;
            }
        }
    }

    private IEnumerator MovementLoop(Vector3 start, Vector3 end, float timeTaken)
    {
        float elapsedTime = 0f;
        while (elapsedTime < timeTaken)
        {
            elapsedTime += 0.2f * Time.deltaTime;
            if (elapsedTime > timeTaken) {
                elapsedTime = timeTaken;
            }
            float perc = elapsedTime / timeTaken;
            perc = Mathf.Sin(perc * Mathf.PI * 0.5f); // smoothing
            
            transform.position = Vector3.Lerp(transform.position, end, perc);
            if (Vector3.Distance(transform.position, end) < 0.1f) {
                Debug.Log("Reached");
                transform.position = end;
                yield break;
            }
            yield return new WaitForEndOfFrame();
            // yield return null;   // one Update() step
        }
    }


    // void OnCollisionEnter(Collision other)
    // {
    //     if (other.gameObject.tag == "Player" && m_ShoveState) {
    //         Vector3 pushDirection = other.transform.position - transform.position;
    //         pushDirection = -pushDirection.normalized;
    //         other.gameObject.GetComponent<PlayerController>().Knockback(pushDirection);
    //     }
    // }

    private void KnockbackDetection() {
        float radius = 3f;
        int playerMask = 1 << 9;    // layer 9: Player
        if (Input.GetButtonDown(m_BButtonName)) {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, playerMask);
            foreach (Collider col in colliders) {
                // Debug.Log(col.gameObject.GetComponent<PlayerController>().m_PlayerNumber);
                if (col.gameObject.GetComponent<PlayerController>().m_PlayerNumber != this.m_PlayerNumber) {
                    Vector3 pushDirection = col.transform.position - transform.position;
                    pushDirection = pushDirection.normalized;
                    col.gameObject.GetComponent<PlayerController>().Knockback(pushDirection);
                }
            }
        }
    }


    public void Knockback(Vector3 direction) {
        m_Knockback_State = true;

        // Stop current shove coroutine
        if (m_CurrentCoroutine != null) StopCoroutine(m_CurrentCoroutine);
        // m_ShoveState = false;
        Vector3 knockbackEnd = transform.position + direction * m_KnockbackDistance;
        Debug.Log(knockbackEnd);
        m_CurrentCoroutine = MovementLoop(transform.position, knockbackEnd, m_KnockbackTime);
        StartCoroutine(m_CurrentCoroutine);
        m_Knockback_State = false;
    }
}
