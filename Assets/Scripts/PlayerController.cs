using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public float m_MovementSpeed = 5f;
    public float m_TurnSpeed = 10f;
    public float m_AnalogMinThreshold = 0.2f;
    public float m_GravityScale = 10f;
    public float m_KnockbackDistance = 5f;
    public float m_ChargeShoveIncrement = 10f;
    public float m_MinChargeShovePressure = 1f;
    public float m_MaxChargeShovePressure = 8f;
    public Image m_selectImage;

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
    private float m_ChargeShovePressure;
    private float m_ShoveTime;
    private bool m_ShoveState;
    private bool m_ChargeState;
    private CharacterController m_Controller;
    private IEnumerator m_CurrentCoroutine;
    private int count;

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
        m_ChargeShovePressure = 0f;
        m_ShoveTime = 2f;
        m_KnockbackTime = 3f;
        count = 0;
        
        m_ChargeState = false;
        m_ShoveState = false;
        m_Knockback_State = false;
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
            if (m_ChargeShovePressure <= 0f && !m_ShoveState) Move();
        }
        else {
            m_KnockbackCounter -= Time.deltaTime;
        }
        // if (m_ShoveState) Debug.Log("Shove");
        
        
    }

    private void Move()
    {
        Vector3 input = Vector3.zero;
        if (Mathf.Abs(m_JoystickHorizontal) > m_AnalogMinThreshold) input.x = m_JoystickHorizontal;
        if (Mathf.Abs(m_JoystickVertical) > m_AnalogMinThreshold) input.z = -m_JoystickVertical;
        
        m_Movement = input * m_MovementSpeed * Time.deltaTime;
        if (input != Vector3.zero) {
            Quaternion newRotation = Quaternion.LookRotation(input, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, m_TurnSpeed * Time.deltaTime);
        }
        m_Movement.y = m_Movement.y + (Physics.gravity.y * m_GravityScale * Time.deltaTime);
        m_Controller.Move(m_Movement);
    }

    private void Shove()
    {
        if (Input.GetButton(m_AButtonName)) {
            m_ChargeState = true;
            ChargeShove();
        }
        else {
            if (m_ChargeShovePressure > 0f) {
                m_ChargeState = false;
                m_ChargeShovePressure += m_MinChargeShovePressure;

                Vector3 shoveEnd = transform.position + transform.forward * m_ChargeShovePressure;
                if (!m_ShoveState) {
                    m_ShoveState = true;
                    m_CurrentCoroutine = DashLoop(transform.position, shoveEnd, m_ShoveTime, m_ShoveState);
                    StartCoroutine(m_CurrentCoroutine);
                }
            }
            m_ChargeShovePressure = 0f;
            m_ChargeState = false;
        }
    }

    private void ChargeShove() {
        if (m_ChargeShovePressure < m_MaxChargeShovePressure) {
            m_ChargeShovePressure += m_ChargeShoveIncrement * Time.deltaTime;
        }
        else {
            m_ChargeShovePressure = m_MaxChargeShovePressure;
        }
    }

    // Lose control and dash from start point to end point in timeTaken
    private IEnumerator DashLoop(Vector3 start, Vector3 end, float timeTaken, bool shoveState)
    {
        float elapsedTime = 0f;
        while (elapsedTime < timeTaken)
        {
            elapsedTime += 0.2f * Time.deltaTime;
            if (elapsedTime > timeTaken) elapsedTime = timeTaken;
            float perc = elapsedTime / timeTaken;
            perc = Mathf.Sin(perc * Mathf.PI * 0.5f); // smoothing
            transform.position = Vector3.Lerp(transform.position, end, perc);
            
            if (shoveState) {
                KnockbackDetection();
            }

            if (Vector3.Distance(transform.position, end) < 0.1f) {
                transform.position = end;
                if (shoveState) {
                    m_ShoveState = false;
                }
                else {
                    m_Knockback_State = false;
                }
                yield break;
            }
            
            yield return new WaitForEndOfFrame();
        }
    }

    
    private IEnumerator KnockbackLoop(Vector3 start, Vector3 end, float increment, float timeTaken)
    {
        float elapsedTime = 0f;
        while (elapsedTime < timeTaken)
        {
            elapsedTime += increment * Time.deltaTime;
            if (elapsedTime > timeTaken) elapsedTime = timeTaken;
            float perc = elapsedTime / timeTaken;
            perc = Mathf.Sin(perc * Mathf.PI * 0.5f); // smoothing
            transform.position = Vector3.Lerp(transform.position, end, perc);

            if (Vector3.Distance(transform.position, end) < 0.1f) {
                transform.position = end;
                m_Knockback_State = false;
                yield break;
            }
            
            yield return new WaitForEndOfFrame();
        }
    }


    private void KnockbackDetection() {
        float radius = 1f;
        int playerMask = 1 << 9;    // layer 9: Player
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, playerMask);
        foreach (Collider col in colliders) {
            // Debug.Log(col.gameObject.GetComponent<PlayerController>().m_PlayerNumber);
            PlayerController otherPlayer = col.gameObject.GetComponent<PlayerController>();
            if (otherPlayer.m_PlayerNumber != this.m_PlayerNumber && !otherPlayer.GetKnockbackState()) {
                count++;
                Vector3 pushDirection = col.transform.position - transform.position;
                pushDirection = pushDirection.normalized;
                otherPlayer.SetKnockbackState(true);
                Debug.Log("knockback called: " + count);
                otherPlayer.Knockback(pushDirection);
            }
        }
    }

    public void Knockback(Vector3 direction) {
        m_ChargeState = false;
        m_ShoveState = false;

        // Stop current shove coroutine
        if (m_CurrentCoroutine != null) StopCoroutine(m_CurrentCoroutine);
        // Debug.Log("Stopped");
        Vector3 knockbackEnd = transform.position + direction * m_KnockbackDistance;
        m_CurrentCoroutine = DashLoop(transform.position, knockbackEnd, m_KnockbackTime, false);
        StartCoroutine(m_CurrentCoroutine);
    }

    public bool GetKnockbackState() {
        return m_Knockback_State;
    }

    public void SetKnockbackState(bool state) {
        m_Knockback_State = state;
    }

    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }

    public float GetChargeShovePressure() {
        return m_ChargeShovePressure;
    }

}
