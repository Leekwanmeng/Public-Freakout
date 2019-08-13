using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEmptyAmmo : MonoBehaviour
{
    private PlayerState m_PlayerState;
    private ParticleSystem m_ParticleSystem;

    void Awake()
    {
        m_PlayerState = GetComponentInParent<PlayerState>();
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckAmmoSmoke();
    }

    void CheckAmmoSmoke() {
        if (m_PlayerState.m_HoldItemId == 2 || m_PlayerState.m_HoldItemId == 3) {
            if (m_PlayerState.m_Ammo <= 0f) {
                m_ParticleSystem.Play();
            }
        }
        else {
            m_ParticleSystem.Clear();
            m_ParticleSystem.Stop();
        }
    
    }   
}
