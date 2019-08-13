using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateParticle : MonoBehaviour
{
    private Item m_Item;
    private ParticleSystem m_PS;

    void Awake()
    {
        m_Item = GetComponentInParent<Item>();
        m_PS = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        EmptyAmmoSmoke();
    }

    void EmptyAmmoSmoke() {
        // if (m_ItemId == 2 || m_ItemId == 3) {
        if (m_Item.m_ItemId == 2) {
            if (m_Item.m_Ammo <= 0f) {
                m_PS.Play();
            }
        }
    }
}
