using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public float speed;
    public string m_JoyStickVerticalName;
    public string m_JoyStickHorizontalName;
    public float m_AnalogMinThreshold = 0.8f;

    private float m_JoystickVertical;
    private float m_JoystickHorizontal;
    private Vector3 m_Movement;
    private Rigidbody m_Rigidbody;
    

    void Start()
    {
        m_JoyStickHorizontalName = "JoystickHorizontal1";
        m_JoyStickVerticalName = "JoystickVertical1";
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Movement = new Vector3(0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        // m_JoystickHorizontal = Input.GetAxis(m_JoyStickHorizontalName);
        // m_JoystickVertical = Input.GetAxis(m_JoyStickVerticalName);
    }

    void FixedUpdate()
    {
        
        m_JoystickHorizontal = Input.GetAxis(m_JoyStickHorizontalName);
        m_JoystickVertical = Input.GetAxis(m_JoyStickVerticalName);

        m_Rigidbody.AddForce(-m_Movement, ForceMode.VelocityChange);

        Vector3 addMovement = new Vector3(0,0,0); // add == 0
        addMovement.x = m_JoystickHorizontal;
        addMovement.z = -m_JoystickVertical;
        
        addMovement *= speed;  // add == 5

        m_Rigidbody.AddForce(addMovement, ForceMode.VelocityChange);    // add == 5, m == 5, addforce 5
        Debug.Log("Movement: "+ m_Movement);
        Debug.Log("add Movement: "+ addMovement);
        Debug.Log("Velocity: "+ m_Rigidbody.velocity);
        m_Movement = addMovement;   // add == 5, m == 5
        
        // Vector3 addMovement = new Vector3(0,0,0);
        // addMovement -= m_Movement;
        // if (Input.GetKey("w")){
        //     addMovement += new Vector3(0,0,1);
        // }
        // if (Input.GetKey("a")){
        //     addMovement += new Vector3(-1,0,0);
        // }
        // if (Input.GetKey("s")){
        //     addMovement += new Vector3(0,0,-1);
        // }
        // if (Input.GetKey("d")){
        //     addMovement += new Vector3(1,0,0);
        // }

        // m_Rigidbody.AddForce(addMovement, ForceMode.VelocityChange);
        // m_Movement = addMovement;
    }
}   
