using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitComboBehavior : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("AttackCombo", false);
        //animator.GetComponentInParent<PlayerController>().Attacking = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //var baseLayer = animator.GetCurrentAnimatorClipInfo(PlayerSettings.GetAnimatorLayers("base"));
        //if (!animator.GetBool("AttackCombo"))
        /*if(baseLayer[0].clip.name.Contains("Attack") && !animator.GetBool("AttackCombo"))
            animator.GetComponentInParent<PlayerController>().Attacking = false;*/
        //if (animator.GetBool("AttackCombo")) animator.SetTrigger("Attack");
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
