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
        animator.SetFloat("SpeedRatio", Mathf.MoveTowards(animator.GetFloat("SpeedRatio"), sideForwardRatio, .05f));
        animator.SetFloat("Speed", velocity);
        animator.SetFloat("WalkAnimationSpeed", velocity < 1.5f ? 1f : .75f * velocity);


        var currentLayerWeight = animator.GetLayerWeight(PlayerSettings.GetAnimatorLayers("walk"));
        var baseLayer = animator.GetCurrentAnimatorClipInfo(PlayerSettings.GetAnimatorLayers("base"));
        var additiveClipNeeded = true;
        if(baseLayer[0].clip) additiveClipNeeded = !(baseLayer[0].clip.name.Contains("Idle") || baseLayer[0].clip.name.Contains("Walk") || baseLayer[0].clip.name.Contains("Run"));
        var desiredValue = (isMoving && additiveClipNeeded) ? 1f : 0f;

        animator.SetLayerWeight(PlayerSettings.GetAnimatorLayers("walk"), Mathf.MoveTowards(currentLayerWeight, desiredValue, .1f));
    }
    public override void SetBlocking(bool value)
    {
        blocking = value;
        base.SetBlocking(value);
        var currentLayerWeight = animator.GetLayerWeight(PlayerSettings.GetAnimatorLayers("walk"));
        var desiredValue = value ? 1f : 0f;
        //animator.SetLayerWeight(PlayerSettings.GetAnimatorLayers("walk"), desiredValue);
    }

    public void SetAgility(float agility)
    {
        animator.SetFloat("Agility", agility);
    }
}
