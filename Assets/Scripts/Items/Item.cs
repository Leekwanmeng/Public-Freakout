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
    
    private float m_Timer;
    protected virtual void Start()
    {
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        Countdown();
        Rotate();
    }

    protected void ResetTimer() {
        m_Timer = m_MaxTime;
    }

    protected void Countdown() {
        m_Timer -= Time.deltaTime;
        if (m_Timer < 0f) {
            Destroy(gameObject);
        }
    }

    protected void Rotate() {
        transform.Rotate(Vector3.up * m_RotateSpeed * Time.deltaTime, Space.Self);
    }



}
