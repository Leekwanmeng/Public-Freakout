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
    public float m_Speed = 4f;
    public float m_TurnSpeed = 10f;
    public bool m_CanWalk;
    public bool m_CanRotate;
    public bool m_IsKnocked;
    public bool m_IsShoving;
    public bool m_IsCharging;
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
    }

    void Update()
    {
        UpdateAnimations();
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
    }

    public void DoForce(Vector3 force) {
        if (force.magnitude > 3f) {
            // m_IsKnocked = true;
            Debug.Log("IM KNOCKED!");
            GetKnocked();
        }
        m_RigidBody.AddForce(force, ForceMode.VelocityChange);
    }
}
