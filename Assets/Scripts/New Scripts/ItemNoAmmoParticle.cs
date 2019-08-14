using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemNoAmmoParticle : MonoBehaviour
{
    private Item m_Item;
    private ParticleSystem m_ParticleSystem;
    private bool m_NoAmmo;

    void Awake()
    {
        m_Item = GetComponentInParent<Item>();
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        m_NoAmmo = false;
    }

    void Update()
    {
        if (!m_NoAmmo) {
            CheckEmptyAmmoSmoke();
        }
    }

    void CheckEmptyAmmoSmoke() {
        if (m_Item.m_Ammo <= 0f) {
            m_ParticleSystem.Play();
            m_NoAmmo = true;
        }
    
    }   
}
