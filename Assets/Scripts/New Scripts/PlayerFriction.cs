using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFriction : MonoBehaviour
{
    public float frictionMagnitude;
    private PlayerWalk m_PlayerWalk;
    private Rigidbody m_Rigidbody;
    // private Vector3 previousMovement;
        
    void Awake()
    {
        m_PlayerWalk = GetComponent<PlayerWalk>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        ApplyFriction();
        // if (Input.GetKey("j")){
        //     m_Rigidbody.AddForce(new Vector3(-11,0,0), ForceMode.VelocityChange);
        //     // m_Rigidbody.AddForce(new Vector3(10,0,0));
        // }
        // if (Input.GetKey("k")){
        //     m_Rigidbody.AddForce(new Vector3(11,0,0), ForceMode.VelocityChange);
        //     // m_Rigidbody.AddForce(new Vector3(10,0,0));
        // }
    }

    void ApplyFriction()
    {
        Vector3 currentMovement = m_PlayerWalk.m_CurrentMovement;
        Vector3 currentVelocity = m_Rigidbody.velocity;
        Vector3 currentForce = currentVelocity - currentMovement;

        if ((currentForce != Vector3.zero) ){
            // Debug.Log("FORCE APPLIED" + currentForce);
            Vector3 frictionForce  = -currentForce/currentForce.magnitude * frictionMagnitude;
            if ( (Mathf.Abs(currentForce.x) <= frictionMagnitude) && (Mathf.Abs(currentForce.z) <= frictionMagnitude) ){
                currentForce.y = 0;
                m_Rigidbody.AddForce(-currentForce, ForceMode.VelocityChange);

            } else {
                frictionForce.y = 0;
                m_Rigidbody.AddForce(frictionForce, ForceMode.VelocityChange);
            }
        }
    }
}
