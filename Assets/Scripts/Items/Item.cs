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
    private float m_FlashMinTime = 3f;
    public bool m_Thrown;
    public float m_ThrowKnockForce;
    
    //AMMO
    public float m_Ammo = 100f;
    
    private float m_Timer;
    private Rigidbody m_RigidBody;
    private MeshRenderer m_Mesh;

    void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_Mesh = GetComponentInChildren<MeshRenderer>();
        m_ThrowKnockForce = 2.0f;
    }
    protected virtual void Start()
    {
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Timer < m_FlashMinTime) {
            StartCoroutine(Flash());
        }
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

    public void ExpireItem()
    {
        GameObject.Find("ItemManager").GetComponent<ItemSpawner>().m_ItemsToSpawn++;
        Destroy(gameObject);
    }

    IEnumerator Flash() {
        while (m_Timer < m_FlashMinTime) {
            m_Mesh.enabled = false;
            yield return new WaitForSeconds(0.15f);
            m_Mesh.enabled = true;
            yield return new WaitForSeconds(0.15f);
            if (m_Timer < 0f) break;
        }
        yield return null;
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
