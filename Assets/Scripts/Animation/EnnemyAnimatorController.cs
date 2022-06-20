using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyAnimatorController : AnimatorController
{
    public override void AttackAnimation(string animTrigger = "")
    {
        if (animTrigger == "") animTrigger = "Attack";
        animator.SetTrigger(animTrigger);
    }
    internal void SetController(float velocity)
    {
        animator.SetFloat("Speed", velocity);
    }

    internal void PlayAnimation(bool v)
    {
        animator.enabled = v;
    }
}
