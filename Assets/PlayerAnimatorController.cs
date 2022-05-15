using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : AnimatorController
{
    public void SetControllerAnimator(float velocity, bool isMoving)
    {
        animator.SetBool("Moving", isMoving);
        animator.SetFloat("Speed", velocity);
    }
}
