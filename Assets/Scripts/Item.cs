using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // Start is called before the first frame update
    public int m_itemId;
    public string m_itemName;
    public float m_maxTime;
    public bool m_groundState;
    
    private float m_Timer;
    void Start()
    {
        m_Timer = m_maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        Countdown();
    }

    private void Countdown() {
        m_Timer -= Time.deltaTime;
        if (m_Timer < 0f) {
            Destroy(gameObject);
        }
    }
}
