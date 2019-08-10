using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleDetection : StateMachineBehaviour
{
    protected GameObject m_CurrentAnimation;
    protected PlayerAnimationDatabase m_PlayerAnimationDB;
    protected PlayerState m_PlayerState;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_PlayerAnimationDB = animator.GetComponent<PlayerAnimationDatabase>();
        m_PlayerState = animator.GetComponent<PlayerState>();

        m_CurrentAnimation = m_PlayerAnimationDB.GetAnimationById(0);
        m_CurrentAnimation = Instantiate (m_CurrentAnimation, animator.rootPosition, animator.transform.rotation) as GameObject;
        m_CurrentAnimation.transform.parent = animator.transform;
        m_PlayerState.m_CurrentAnimation = m_CurrentAnimation;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(m_CurrentAnimation);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
