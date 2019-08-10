﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public int m_PlayerMask = 1 << 9;    // layer 9: Player
    public int m_ItemMask = 1 << 10; // layer 10: Items
    public int m_HoldItemId = -1;
    public float m_Speed = 4f;
    public float m_TurnSpeed;
    public bool m_CanWalk;
    public bool m_CanRotate;
    public bool m_IsKnocked;
    public bool m_IsShoving;
    public bool m_IsCharging;
    public bool m_IsUsingStationaryItem;
    public float m_FrictionMagnitude;
    public float m_MovementMagnitude;
    public Vector3 m_CurrentMovement;
    public Vector3 m_PreviousMovement;
    public float m_ChargeShovePressure;
    public float m_MinChargeShovePressure;
    public float m_MaxChargeShovePressure;
    public Image m_selectImage;
    public Animator m_Animator;
    public Rigidbody m_RigidBody;

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_RigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        m_TurnSpeed = 10f;
        m_IsUsingStationaryItem = false;
    }

    void Update()
    {   
        if (m_IsKnocked && m_RigidBody.velocity == Vector3.zero){
            Debug.Log("Stoped knocked");
            m_IsKnocked = false;
            m_CanWalk = true;
            m_CanRotate = true;
        }
        if (m_PlayerNumber == 1){
        // Debug.Log("Knocked: " + m_IsKnocked);
        // Debug.Log("Charging: " + m_IsCharging);
        // Debug.Log("Shoving: " + m_IsShoving);
        // Debug.Log("Walkable: " + m_CanWalk);
        // Debug.Log("Can Rotate: " + m_CanRotate);
        }
        UpdateAnimations();
        CheckStationaryItem();
    }

    void CheckStationaryItem() {
        float slowerTurn = 2.0f;
        bool holdingStationaryItem = m_HoldItemId == 2 || m_HoldItemId == 3;
        if (m_IsUsingStationaryItem) {
            m_CanWalk = false;
            m_TurnSpeed = slowerTurn;
        } else {
            if (holdingStationaryItem){
                m_CanWalk = true;
                m_TurnSpeed = 10f;
            }
        }
    }

    void UpdateAnimations() {
        m_Animator.SetFloat("movementMagnitude", m_MovementMagnitude);
        m_Animator.SetInteger("holdItemId", m_HoldItemId);
        m_Animator.SetFloat("chargeShovePressure", m_ChargeShovePressure);
        m_Animator.SetBool("isCharged", m_IsCharging);
    }

    public void GetKnocked() {
        m_IsKnocked = true;
        m_CanWalk = false;
        m_CanRotate = false;
        m_IsShoving = false;
        m_IsCharging = false;
        m_ChargeShovePressure = 0.0f;
        GetComponent<PlayerAction>().DropItem();
    }

    public void DoForce(Vector3 force) {
        if (force.magnitude > 5f) {
            // m_IsKnocked = true;
            Debug.Log("IM KNOCKED!");
            GetKnocked();
        }
        m_RigidBody.AddForce(force, ForceMode.VelocityChange);
    }
}
