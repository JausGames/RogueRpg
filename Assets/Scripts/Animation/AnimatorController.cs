using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] bool waitToBlock = false;
    [SerializeField] bool waitToRoll = false;
    [SerializeField] private bool waitToAttack;
    protected int animAttackTrigger;

    public Animator Animator { get => animator; set => animator = value; }
    public bool WaitToAttack { get => waitToAttack; set => waitToAttack = value; }

    private void Update()
    {
        var baseLayer = animator.GetCurrentAnimatorClipInfo(PlayerSettings.GetAnimatorLayers("base"));
        var rollLayer = animator.GetCurrentAnimatorClipInfo(PlayerSettings.GetAnimatorLayers("roll"));
        if (waitToRoll)
        {
            if ((baseLayer[0].clip.name.Contains("Idle")
            || baseLayer[0].clip.name.Contains("Walk")
            || baseLayer[0].clip.name.Contains("Run")
            || baseLayer[0].clip.name.Contains("GetHit")
            ) && rollLayer[0].clip.name.Contains("Idle"))
            {
                waitToRoll = false;

                animator.SetTrigger("Roll");
                animator.SetLayerWeight(3, 1f);
                return;
            }
        }
        if (waitToBlock && baseLayer.Length >= 1)
        {
            if (baseLayer[0].clip.name.Contains("Idle") || baseLayer[0].clip.name.Contains("Walk") || baseLayer[0].clip.name.Contains("Run"))
            {
                waitToBlock = false;

                animator.SetBool("Blocking", true);
                animator.SetLayerWeight(2, 1f);
                return;
            }
        }
        if (waitToAttack) Debug.Log("AnimatorController, Wait to attack : " + baseLayer[0].clip.name);
        if(waitToAttack && baseLayer.Length >= 1 && rollLayer.Length >= 1)
        {
            if ((baseLayer[0].clip.name.Contains("Idle")
                  || baseLayer[0].clip.name.Contains("Walk")
                  || baseLayer[0].clip.name.Contains("Run")
                  ) && rollLayer[0].clip.name.Contains("Idle"))
            {
                waitToAttack = false;
                animator.SetTrigger(animAttackTrigger);
                return;
            }
            else if (baseLayer[0].clip.name.Contains("Attack"))
                waitToAttack = false;
        }
    }
    virtual public void AttackAnimation(int animAttackTriggerId = 1080829965)
    {
        animAttackTrigger = animAttackTriggerId;
        waitToAttack = true;
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
        var baseLayer = animator.GetCurrentAnimatorClipInfo(PlayerSettings.GetAnimatorLayers("base"));
        if (value &&
        // check if base layer is on an animation that accept transition (IDLE, WALK, GETHIT, GETBLOCK)
            !(baseLayer[0].clip.name.Contains("Idle")
            || baseLayer[0].clip.name.Contains("Walk")
            || baseLayer[0].clip.name.Contains("Run")
            || baseLayer[0].clip.name.Contains("GetHit")
            || baseLayer[0].clip.name.Contains("GetBlock")))
        {
            waitToBlock = true;
            return;
        };

        animator.SetBool("Blocking", value);
        animator.SetLayerWeight(PlayerSettings.GetAnimatorLayers("block"), value ? 1f : 0f);

        // waitToBlock = true;
    }
    internal void Roll(bool value)
    {
        var baseLayer = animator.GetCurrentAnimatorClipInfo(PlayerSettings.GetAnimatorLayers("base"));
        var rollLayer = animator.GetCurrentAnimatorClipInfo(PlayerSettings.GetAnimatorLayers("roll"));

        if (value && (
            // check if base layer is on an animation that accept transition (IDLE or WALK)
            !(baseLayer[0].clip.name.Contains("Idle")
            || baseLayer[0].clip.name.Contains("Walk")
            || baseLayer[0].clip.name.Contains("Run"))

            // check if roll layer is on an animation that accept transition (IDLE)
            || !rollLayer[0].clip.name.Contains("Idle"))
            )
        {
            waitToRoll = true;
            return;
        };
        if (value) animator.SetTrigger("Roll");
        animator.SetLayerWeight(PlayerSettings.GetAnimatorLayers("roll"), value || waitToRoll ? 1f : 0f);

        // waitToRoll = true;
    }

    internal bool CanRun()
    {
        var baseLayer = animator.GetCurrentAnimatorClipInfo(PlayerSettings.GetAnimatorLayers("base"));
        var rollLayer = animator.GetCurrentAnimatorClipInfo(PlayerSettings.GetAnimatorLayers("roll"));
        var blockLayer = animator.GetCurrentAnimatorClipInfo(PlayerSettings.GetAnimatorLayers("block"));

        return (baseLayer[0].clip.name.Contains("Idle")
            || baseLayer[0].clip.name.Contains("Walk")
            || baseLayer[0].clip.name.Contains("Run"))
            && rollLayer[0].clip.name.Contains("Idle")
            && blockLayer[0].clip.name.Contains("Idle");
    }
}
