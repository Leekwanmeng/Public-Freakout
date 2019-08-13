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
    public float m_TurnSpeed;
    public bool m_CanWalk;
    public bool m_CanRotate;
    public bool m_IsKnocked;
    public bool m_IsShoving;
    public bool m_IsCharging;
    public bool m_IsUsingStationaryItem;
    public bool m_IsSingleUseItem;
    public float m_FrictionMagnitude;
    public float m_MovementMagnitude;
    public Vector3 m_CurrentMovement;
    public Vector3 m_PreviousMovement;
    public float m_ChargeShovePressure;
    public float m_MinChargeShovePressure;
    public float m_MaxChargeShovePressure;
    public float m_Cooldown;
    public Image m_selectImage;
    public Animator m_Animator;
    public Rigidbody m_RigidBody;
    public GameObject m_CurrentAnimation;
    public List<int> m_ShoveHitLog;
    public float m_AEDChargingTime;
    public float m_AEDCastDuration = 0.5f;
    public bool m_AEDIsCharging = false;
    public bool m_UseCoolDownUI = false;
    

    public float m_Ammo;
    

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_RigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        m_TurnSpeed = 10f;
        m_IsUsingStationaryItem = false;
        m_ShoveHitLog = new List<int>();
    }

    void Update()
    {   
        if (m_IsKnocked && m_RigidBody.velocity == Vector3.zero){
            m_IsKnocked = false;
            m_CanWalk = true;
            m_CanRotate = true;
            m_TurnSpeed = 10f;
        }
        UpdateAnimations();
        CheckStationaryItem();
        ReduceCooldown();
    }

    // void CheckStationaryItem() {
    //     float slowerTurn = 2.0f;

    //     if (m_IsUsingStationaryItem) {
        
    //         m_CanWalk = false;
    //         m_TurnSpeed = slowerTurn;
        
    //     } else {
    //         // if (!m_IsShoving && !m_IsKnocked){
    //         //     m_CanWalk = true;
    //         //     m_TurnSpeed = 10f;
    //         // }
            
    //     }
    // }

    void CheckStationaryItem() {
        float slowerTurn = 2.0f;
        bool holdingStationaryItem = m_HoldItemId == 2 || m_HoldItemId == 3;
        if (m_IsUsingStationaryItem) {
            m_CanWalk = false;
            m_TurnSpeed = slowerTurn;
        } else {
            // if (GetComponent<AudioSource>().loop == true){
            // GetComponent<AudioSource>().Stop();
            // GetComponent<AudioSource>().loop = false;
            // }
            if (holdingStationaryItem){
                GetComponent<AudioSource>().Stop();
                GetComponent<AudioSource>().loop = false;
                m_CanWalk = true;
                m_TurnSpeed = 10f;
            }
        }
    }

    void ReduceCooldown() {
        if (m_Cooldown > 0f) {
            m_Cooldown -= Time.deltaTime;
            if (m_Cooldown < 0f) m_Cooldown = 0f;
        }
    }

    void UpdateAnimations() {
        m_Animator.SetFloat("movementMagnitude", m_MovementMagnitude);
        m_Animator.SetInteger("holdItemId", m_HoldItemId);
        m_Animator.SetFloat("chargeShovePressure", m_ChargeShovePressure);
        m_Animator.SetBool("isCharged", m_IsCharging);
        m_Animator.SetBool("isKnocked", m_IsKnocked);
        m_Animator.SetBool("isUsingStationaryItem", m_IsUsingStationaryItem);
        m_Animator.SetBool("isSingleUseItem", m_IsSingleUseItem);
        m_Animator.SetBool("isShoving", m_IsShoving);
    }

    public void GetKnocked() {
        m_ChargeShovePressure = 0.0f;
        m_IsKnocked = true;
        m_CanWalk = false;
        m_CanRotate = false;
        m_IsShoving = false;
        m_IsCharging = false;
        m_IsSingleUseItem = false;
        m_IsUsingStationaryItem = false;
        m_AEDIsCharging = false;
    }

    public void DoForce(Vector3 force) {
        if (force.magnitude > 6f) {
            // m_IsKnocked = true;
            GetKnocked();
            GetComponent<PlayerAction>().KnockDropItem(force);
            // GetComponent<AudioSource>().PlayOneShot(GetComponent<PlayerAction>().sfx_PlayerCollision);
        }
        m_RigidBody.AddForce(force, ForceMode.VelocityChange);
    }

    public void DestroyCurrentAnimation() {
        Destroy(m_CurrentAnimation);
    }
}
