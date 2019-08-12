using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackhammerEffects : MonoBehaviour
{
    private PlayerState m_PlayerState;
    private ParticleSystem[] m_ParticleSystems;
    void Awake()
    {
        m_PlayerState = GetComponentInParent<PlayerState>();
        m_ParticleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        Dust();
    }

    void Dust() {
        if (m_PlayerState.m_HoldItemId == 3 && m_PlayerState.m_IsUsingStationaryItem) {
            
            foreach (ParticleSystem ps in m_ParticleSystems) {
                ps.Play();
            }
        }
        else {
            foreach (ParticleSystem ps in m_ParticleSystems) {
                ps.Clear();
                ps.Stop();
            }
        }
    }
}
