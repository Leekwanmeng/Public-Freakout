using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private int m_CurrentAnimId;
    private int m_PreviousAnimId;
    private GameObject m_CurrentAnimation;
    private PlayerAnimationDatabase m_Database;
    private PlayerState m_PlayerState;

    const int EMPTY_IDLE = 0;
    const int EMPTY_WALK = 1;
    const int EMPTY_CHARGE = 2;
    const int EMPTY_SHOVE = 3;
    const int BAT_IDLE = 4;
    const int BAT_WALK = 5;
    const int BAT_ACTION = 6;
    const int AED_IDLE = 7;
    const int AED_WALK = 8;
    const int AED_CHARGE = 9;
    const int AED_DISCHARGE = 10;
    const int EXTINGUISHER_IDLE = 11;
    const int EXTINGUISHER_WALK = 12;
    // const int EXTINGUISHER_ACTION;
    const int JACKHAMMER_IDLE = 13;
    const int JACKHAMMER_WALK = 14;
    const int JACKHAMMER_ACTION = 15;

    private bool idle;
    
    void Awake()
    {
        m_Database = GameObject.Find("PlayerAnimationManager").GetComponent<PlayerAnimationDatabase>();
        m_PlayerState = GetComponent<PlayerState>();
    }
    void Start()
    {
        SetAnimation(EMPTY_IDLE);
        idle = true;
    }

    // TODO: Code out state machine
    void Update()
    {
        // if curr state != prev state
            // run state machine
        
        if (m_CurrentAnimId != m_PreviousAnimId) {
            StateMachine();
        }
        m_PreviousAnimId = m_CurrentAnimId;

        if (m_PlayerState.m_MovementMagnitude > 0.01f && idle) {
            SetAnimation(EMPTY_WALK);
            idle = false;
        } 
        else if (m_PlayerState.m_MovementMagnitude < 0.01f && !idle) {
            idle = true;
            SetAnimation(EMPTY_IDLE);
        }

    }

    void SetAnimation(int anim_id) {
        // Remove previous animation
        if (m_CurrentAnimation != null) {
            Destroy(m_CurrentAnimation);
        }

        // Set new animation
        m_CurrentAnimation = m_Database.GetAnimationById(anim_id);
        m_CurrentAnimation = Instantiate (m_CurrentAnimation, transform.position, transform.rotation) as GameObject;
        m_CurrentAnimation.transform.parent = transform;
    }

    void StateMachine() {
        switch (m_CurrentAnimId) {
            case EMPTY_IDLE:
                SetAnimation(EMPTY_IDLE);
                break;
        }
    }
}
