using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopRollingBehavior : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("StopRollingBehavior, OnStateEnter : stop rolling movement");
        var player = animator.GetComponentInParent<Player>();
        var body = player.GetComponent<Rigidbody>();
        var currentList = new List<Status>();
        currentList.AddRange(player.CurrentStatusList);
        foreach (Status status in currentList)
            if (status.StatusType == Status.Type.Rolling) status.RemoveStatusFromHitable(player);
        body.velocity = body.velocity * (Mathf.Sqrt(animator.GetFloat("Agility") + 5) * (1 / 6));
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /*override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }*/

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("StopRollingBehavior, OnStateExit : stop rolling");
        animator.GetComponentInParent<AnimatorController>().Roll(false);
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
