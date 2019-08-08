using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalk : MonoBehaviour
{    
    private string m_JoyStickHorizontalName;
    private string m_JoyStickVerticalName;
    private PlayerState m_PlayerState;
    private Rigidbody m_Rigidbody;

    void Awake()
    {
        m_PlayerState = GetComponent<PlayerState>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Start() {
        m_PlayerState.m_CurrentMovement = Vector3.zero;
        m_JoyStickHorizontalName = "JoystickHorizontal" + m_PlayerState.m_PlayerNumber;
        m_JoyStickVerticalName = "JoystickVertical" + m_PlayerState.m_PlayerNumber;
    }

    void FixedUpdate()
    {
        // m_Rigidbody.AddForce(-currentMovement, ForceMode.VelocityChange);
        float h = Input.GetAxis(m_JoyStickHorizontalName);
        float v = -Input.GetAxis(m_JoyStickVerticalName);

        if (m_PlayerState.m_CanWalk) {
            Walk(h,v);
        }
        if (m_PlayerState.m_CanRotate) {
            Rotate(h,v);
        }
        m_PlayerState.m_MovementMagnitude = m_PlayerState.m_CurrentMovement.magnitude;
    }

    void Walk(float h, float v)
    {
        m_PlayerState.m_PreviousMovement = new Vector3(h, 0, v);
        m_PlayerState.m_PreviousMovement *= m_PlayerState.m_Speed;
        m_Rigidbody.AddForce(m_PlayerState.m_PreviousMovement - m_PlayerState.m_CurrentMovement, ForceMode.VelocityChange);
        m_PlayerState.m_CurrentMovement = m_PlayerState.m_PreviousMovement;
    }


    void Rotate(float h, float v)
    {
        m_PlayerState.m_PreviousMovement = new Vector3(h,0,v);
        if (m_PlayerState.m_PreviousMovement != Vector3.zero) {
            Quaternion newRotation = Quaternion.LookRotation(m_PlayerState.m_PreviousMovement, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, m_PlayerState.m_TurnSpeed * Time.deltaTime);
        }
    }
}
