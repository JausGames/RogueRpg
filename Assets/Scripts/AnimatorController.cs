using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] protected Animator animator;

    public Animator Animator { get => animator; set => animator = value; }

    virtual public void AttackAnimation()
    {
        animator.SetTrigger("Attack");
    }

    virtual public void TryCombo()
    {
        animator.SetBool("AttackCombo", true);
    }
    virtual public void GetHit()
    {
        animator.SetTrigger("GetHit");
    }
    virtual public void GetBlock()
    {
        animator.SetTrigger("GetBlock");
    }
    virtual public void SetBlocking(bool value)
    {
        animator.SetBool("Blocking", value);
    }
    internal void Roll()
    {
        animator.SetTrigger("Roll");
    }
}
