using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : AnimatorController
{
    bool blocking;
    public void SetControllerAnimator(float velocity, bool isMoving ,float forwardDot,float sideDot, float sideForwardRatio)
    {
        animator.SetBool("Moving", isMoving);
        animator.SetFloat("SpeedForward", Math.Abs(forwardDot) > 0.3f ? Mathf.Sign(forwardDot) : 0f);
        animator.SetFloat("SpeedSide", Math.Abs(sideDot) > 0.3f ? Mathf.Sign(sideDot) : 0f);
        animator.SetFloat("SpeedRatio", sideForwardRatio);
        animator.SetFloat("Speed", velocity);
        animator.SetFloat("WalkAnimationSpeed", velocity < 1.5f ? 1f : .75f * velocity);
        animator.SetLayerWeight(PlayerSettings.GetAnimatorLayers("walk"), isMoving || blocking ? 1f : 0f);
    }
    public override void SetBlocking(bool value)
    {
        blocking = value;
        base.SetBlocking(value);
        animator.SetLayerWeight(PlayerSettings.GetAnimatorLayers("walk"), value ? 1f : 0f);
    }

    public void SetAgility(float agility)
    {
        animator.SetFloat("Agility", agility);
    }
}
