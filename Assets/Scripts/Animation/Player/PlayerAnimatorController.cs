using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : AnimatorController
{
    bool blocking;
    public void SetControllerAnimator(float velocity, bool isMoving)
    {
        animator.SetBool("Moving", isMoving);
        animator.SetLayerWeight(PlayerSettings.GetAnimatorLayers("walk"), isMoving || blocking ? 1f : 0f);
        animator.SetFloat("Speed", velocity);
    }
    public override void SetBlocking(bool value)
    {
        blocking = value;
        base.SetBlocking(value);
        animator.SetLayerWeight(PlayerSettings.GetAnimatorLayers("walk"), value ? 1f : 0f);
    }
}
