using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class WeaponData : ScriptableObject
{
    [Header("Stats - Damage")]
    [SerializeField] protected float damage;
    [SerializeField] protected float hitRange;
    [SerializeField] protected KnockbackData knockBack;
    [SerializeField] protected float coolDown;
    [SerializeField] protected float nextHit = 0f;
    [SerializeField] protected bool comboable = true;

    public float Damage { get => damage; set => damage = value; }
    public float HitRange { get => hitRange; set => hitRange = value; }
    public KnockbackData KnockBack { get => knockBack; set => knockBack = value; }
    public float CoolDown { get => coolDown; set => coolDown = value; }
    public bool Comboable { get => comboable; set => comboable = value; }
    public float NextHit { get => nextHit; set => nextHit = value; }

    virtual public void TriggerWeapon(Transform owner, Transform hitPoint, LayerMask enemyLayer, LayerMask friendLayer, AnimatorController animator)
    {
        if (!Comboable && NextHit > Time.time) return;
        else if (NextHit > Time.time)
            animator.TryCombo();
        else
        {
            if (animator) animator.AttackAnimation();
            //ResetCooldown();
        }
    }
    public void ResetCooldown()
    {
        nextHit = Time.time + coolDown;
    }
}
