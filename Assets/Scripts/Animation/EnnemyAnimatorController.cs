using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyAnimatorController : AnimatorController
{
    private void Awake()
    {
        Debug.Log("Attack hash = " + Animator.StringToHash("SmallAttack"));
    }
    public override void AttackAnimation(int animTrigger = 0)
    {
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
