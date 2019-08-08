using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // Start is called before the first frame update
    public int m_ItemId;
    public string m_ItemName;
    public float m_MaxTime = 15f;
    public float m_RotateSpeed = 30f;
    public bool m_Thrown;
    public float m_ThrowKnockForce;
    
    private float m_Timer;
    private Rigidbody m_RigidBody;

    void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_ThrowKnockForce = 1f;
    }
    protected virtual void Start()
    {
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        Countdown();
    }

    protected void ResetTimer() {
        m_Timer = m_MaxTime;
    }

    protected void Countdown() {
        m_Timer -= Time.deltaTime;
        if (m_Timer < 0f) {
            ExpireItem();
        }
    }

    protected void ExpireItem()
    {
        GameObject.Find("ItemManager").GetComponent<ItemSpawner>().m_ItemsToSpawn++;
        Destroy(gameObject);
    }

    protected void CheckGround() {
        transform.Rotate(Vector3.up * m_RotateSpeed * Time.deltaTime, Space.Self);
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Ground") {
            m_Thrown = false;
        }
        if (m_Thrown && other.gameObject.tag == "Player")  {
            Vector3 direction = other.transform.position - transform.position;
            direction.y = 0f;
            direction.Normalize();
            // Debug.Log(m_RigidBody.velocity.magnitude);
            Vector3 force = direction * m_RigidBody.velocity.magnitude * m_ThrowKnockForce;
            other.gameObject.GetComponent<PlayerState>().DoForce(force);
        }
    }




}
