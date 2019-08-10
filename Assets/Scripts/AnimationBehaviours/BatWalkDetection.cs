using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatWalkDetection : StateMachineBehaviour
{
    protected GameObject m_CurrentAnimation;
    protected PlayerAnimationDatabase m_PlayerAnimationDB;
    protected PlayerState m_PlayerState;
    const int BAT_WALK = 5;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_PlayerAnimationDB = animator.GetComponent<PlayerAnimationDatabase>();
        m_PlayerState = animator.GetComponent<PlayerState>();
        if (m_PlayerState.m_CurrentAnimation != null) Destroy(m_PlayerState.m_CurrentAnimation);

        m_CurrentAnimation = m_PlayerAnimationDB.GetAnimationById(BAT_WALK);
        m_CurrentAnimation = Instantiate (m_CurrentAnimation, animator.rootPosition, animator.transform.rotation) as GameObject;
        m_CurrentAnimation.transform.parent = animator.transform;
        m_PlayerState.m_CurrentAnimation = m_CurrentAnimation;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(m_CurrentAnimation);
    }
}
