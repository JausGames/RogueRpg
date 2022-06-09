using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRollingBehavior : StateMachineBehaviour
{
    bool hasRoll;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("StartRollingBehavior, OnStateEnter : set hasRool = false");
        hasRoll = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    if (animator.GetLayerWeight(3) == 1f && animator.GetComponentInParent<Player>() && stateInfo.normalizedTime > .2f && !hasRoll)
        {
            Debug.Log("StartRollingBehavior, OnStateUpdate : start roll movement");
            animator.GetComponentInParent<Player>().StartRollingMovement();
            hasRoll = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
