using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRotation : MonoBehaviour
{
    private Quaternion m_RelativeRotation; 

    void Start()
    {
        m_RelativeRotation = transform.localRotation;
    }

    void Update()
    {
        transform.rotation = m_RelativeRotation;
    }
}
