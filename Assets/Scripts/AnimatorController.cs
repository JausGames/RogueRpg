using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    bool waitToBlock = false;
    bool waitToRoll = false;

    public Animator Animator { get => animator; set => animator = value; }

    private void Update()
    {
        if(waitToBlock)
        {

            var m_CurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            var m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
            if (m_CurrentClipInfo[0].clip.name.Contains("Idle") || m_CurrentClipInfo[0].clip.name.Contains("Walk"))
            {
                waitToBlock = false;

                animator.SetBool("Blocking", true);
                animator.SetLayerWeight(2, 1f);
            }
        }
        if (waitToRoll)
        {

            var m_CurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            var m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
            if (m_CurrentClipInfo[0].clip.name.Contains("Idle")
            || m_CurrentClipInfo[0].clip.name.Contains("Walk")
            || m_CurrentClipInfo[0].clip.name.Contains("GetHit")
            || m_CurrentClipInfo[0].clip.name.Contains("GetBlock"))
            {
                waitToRoll = false;

                animator.SetTrigger("Roll");
                animator.SetLayerWeight(3, 1f);
            }
        }
    }
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
        //Fetch the current Animation clip information for the base layer
        var m_CurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        //Access the current length of the clip
        var m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
        //Access the Animation clip name
        if (value && !(m_CurrentClipInfo[0].clip.name.Contains("Idle") 
            || m_CurrentClipInfo[0].clip.name.Contains("Walk") 
            || m_CurrentClipInfo[0].clip.name.Contains("GetHit") 
            || m_CurrentClipInfo[0].clip.name.Contains("GetBlock")))
        {
            waitToBlock = true;
            return;
        };

        Debug.Log("AnimatorController, SetBlocking : on ? " + value);
        animator.SetBool("Blocking", value);
        animator.SetLayerWeight(2, value ? 1f : 0f);
    }
    internal void Roll(bool value)
    {
        //Fetch the current Animation clip information for the base layer
        var m_CurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        //Access the current length of the clip
        var m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
        //Access the Animation clip name
        if (value && !(m_CurrentClipInfo[0].clip.name.Contains("Idle") || m_CurrentClipInfo[0].clip.name.Contains("Walk")))
        {
            waitToRoll = true;
            return;
        };

        Debug.Log("AnimatorController, SetBlocking : on ? " + value);
        if(value)animator.SetTrigger("Roll");
        animator.SetLayerWeight(3, value ? 1f : 0f);
    }
}
