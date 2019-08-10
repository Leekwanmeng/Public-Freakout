using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDirectionalControl : MonoBehaviour
{
    public bool m_UseRelativeRotation = true;  

    private Quaternion m_RelativeRotation;     


    private void Start()
    {
        m_RelativeRotation = transform.parent.localRotation;
    }

    private void Update()
    {
        if (m_UseRelativeRotation)
            transform.rotation = m_RelativeRotation;
    }
}
