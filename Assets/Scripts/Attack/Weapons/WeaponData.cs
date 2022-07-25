using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class WeaponData : ScriptableObject
{
    [Header("Stats - Damage")]
    [SerializeField] protected float damage;
    [SerializeField] protected float mass;
    //[SerializeField] protected float hitRange;
    [SerializeField] protected float neededStrength;
    [SerializeField] protected KnockbackData knockBack;
    [SerializeField] protected List<AttackData> attackSet = new List<AttackData>();
    [SerializeField] protected AttackData currentAttack;
    protected float nextHit = 0f;
    [Header("Combo")]
    [SerializeField] protected bool comboable = true;
    [SerializeField] protected float comboTime = .3f;
    internal float NextHitDamageRatio = 1f;

    public float Damage { get => damage; set => damage = value; }
    public List<float> HitRange { 
        get  {
            var range = new List<float>();
            foreach(AttackData data in attackSet)
            {
                range.Add(data.range);
            }
            return range;
        } 
    }
    public float Mass { get => mass; set => mass = value; }
    public KnockbackData KnockBack { get => knockBack; set => knockBack = value; }
    public float CoolDown { get => comboTime; set => comboTime = value; }
    public bool Comboable { get => comboable; set => comboable = value; }
    public float NeededStrength { get => neededStrength; set => neededStrength = value; }
    public AttackData CurrentAttack { get => currentAttack; set => currentAttack = value; }
    public List<AttackData> AttackSet { get => attackSet; set => attackSet = value; }

    virtual public void TriggerWeapon(Transform owner, Transform hitPoint, LayerMask enemyLayer, LayerMask friendLayer, AnimatorController animator, int animTriggerId = 1080829965)
    {
        if (!Comboable && nextHit > Time.time) return;
        else if (nextHit > Time.time)
            animator.TryCombo();
        else
        {
            if (animator) animator.AttackAnimation(animTriggerId);
            if(Comboable) ResetCombo();
        }
    }
    public void ResetCombo()
    {
        nextHit = Time.time + comboTime;
    }
}
