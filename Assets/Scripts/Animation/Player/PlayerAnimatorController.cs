using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : AnimatorController
{
    bool blocking;
    public void SetControllerAnimator(float velocity, bool isMoving ,float forwardRatio,float sideRatio)
    {
        animator.SetBool("Moving", isMoving);
        animator.SetFloat("SpeedForward", (velocity * Mathf.Sign(forwardRatio)));
        //animator.SetFloat("SpeedForward", (velocity * forwardRatio));
        animator.SetFloat("SpeedSide", (velocity * Mathf.Sign(sideRatio)));
        animator.SetFloat("SpeedRatio", Mathf.Min(1f, Mathf.Abs(forwardRatio) / Mathf.Abs(sideRatio)));
        animator.SetLayerWeight(PlayerSettings.GetAnimatorLayers("walk"), isMoving || blocking ? 1f : 0f);
    }
    public override void SetBlocking(bool value)
    {
        blocking = value;
        base.SetBlocking(value);
        animator.SetLayerWeight(PlayerSettings.GetAnimatorLayers("walk"), value ? 1f : 0f);
    }
}
