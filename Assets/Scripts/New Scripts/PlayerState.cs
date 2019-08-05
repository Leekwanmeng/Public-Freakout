using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public int m_PlayerMask = 1 << 9;    // layer 9: Player
    public int m_ItemMask = 1 << 10; // layer 10: Items
    public int m_HoldItemId = -1;
    public bool m_CanWalk;
    public bool m_CanRotate;
    public bool m_IsKnocked;
    public bool m_IsShoving;
    public bool m_IsCharging;
    public float m_FrictionMagnitude;
    public float m_Movement;
    public float m_ChargeShovePressure;
    public float m_MinChargeShovePressure;
    public float m_MaxChargeShovePressure;
    public Image m_selectImage;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
