using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockLauncher : MonoBehaviour
{
    private PlayerState m_PlayerState;
    private ParticleSystem m_ParticleSystem;
    private bool m_Played;
    void Awake()
    {
        m_PlayerState = GetComponentInParent<PlayerState>();
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_Played = false;
    }

    void Update()
    {
        if (!m_Played) {
            StartCoroutine(Shock());
        }
    }

    IEnumerator Shock() {
        if (m_PlayerState.m_HoldItemId == 1 && m_PlayerState.m_IsSingleUseItem) {
            m_Played = true;
            yield return new WaitForSeconds(1f);
            m_ParticleSystem.Play();
            m_Played = false;
        }
    }
}
