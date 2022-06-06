using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyAnimatorController : AnimatorController
{
    internal void SetController(float velocity)
    {
        animator.SetFloat("Speed", velocity);
    }

    internal void PlayAnimation(bool v)
    {
        animator.enabled = v;
    }
}
