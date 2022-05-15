using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetlingAnimatorController : AnimatorController
{
    internal void SetController(float velocity)
    {
        animator.SetFloat("Speed", velocity);
    }
}
