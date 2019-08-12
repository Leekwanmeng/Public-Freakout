using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFriction : MonoBehaviour
{
    private PlayerState m_PlayerState;
    private Rigidbody m_Rigidbody;
    // private Vector3 previousMovement;
        
    void Awake()
    {
        m_PlayerState = GetComponent<PlayerState>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        ApplyFriction();

    }

    void ApplyFriction()
    {
        Vector3 currentMovement = m_PlayerState.m_CurrentMovement;
        Vector3 currentVelocity = m_Rigidbody.velocity;
        Vector3 currentForce = currentVelocity - currentMovement;

        if ((currentForce != Vector3.zero) ){
            // Debug.Log("FORCE APPLIED" + currentForce);
            Vector3 frictionForce  = -currentForce/currentForce.magnitude * m_PlayerState.m_FrictionMagnitude;
            if ( (Mathf.Abs(currentForce.x) <= m_PlayerState.m_FrictionMagnitude) && (Mathf.Abs(currentForce.z) <= m_PlayerState.m_FrictionMagnitude) ){
                currentForce.y = 0;
                m_Rigidbody.AddForce(-currentForce, ForceMode.VelocityChange);

            } else {
                frictionForce.y = 0;
                m_Rigidbody.AddForce(frictionForce, ForceMode.VelocityChange);
            }
        }
    }
}
