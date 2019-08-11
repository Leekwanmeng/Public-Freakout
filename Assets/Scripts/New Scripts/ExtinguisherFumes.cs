using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguisherFumes : MonoBehaviour
{
    private PlayerState m_PlayerState;
    private ParticleSystem m_ParticleSystem;
    void Awake()
    {
        m_PlayerState = GetComponentInParent<PlayerState>();
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        Blow();
    }

    void Blow() {
        if (m_PlayerState.m_HoldItemId == 2 && m_PlayerState.m_IsUsingStationaryItem) {
            m_ParticleSystem.Play();
        }
        else {
            m_ParticleSystem.Clear();
            m_ParticleSystem.Stop();
        }
    }
}
