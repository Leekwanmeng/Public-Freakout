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

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        m_Animator.SetFloat("movementMagnitude", m_MovementMagnitude);
        m_Animator.SetInteger("holdItemId", m_HoldItemId);
        m_Animator.SetFloat("chargeShovePressure", m_ChargeShovePressure);
    }
    
    public void EnterKnockedState() {
        StartCoroutine(KnockedState());
    }

    IEnumerator KnockedState () {
        Debug.Log("Entered knock");
        m_IsKnocked = true;
        m_CanWalk = false;
        m_CanRotate = false;

        // SUper hacky!! FInd a way to tell when a force ends / how long it takes for friction to stop
        yield return new WaitForSeconds(1);

        m_IsKnocked = false;
        m_CanWalk = true;
        m_CanRotate = true;
        Debug.Log("Finish knock");
    }
}
