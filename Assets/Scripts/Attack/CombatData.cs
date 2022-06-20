using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class CombatData
{
    [Header("Stats - Health")]
    [SerializeField] protected float maxHealth = 10f;
    [SerializeField] protected float health;
    [SerializeField] protected float knockBackRatio = 1f;
    [Header("Stats - Speed")]
    [SerializeField] protected float strength;
    [SerializeField] protected float speed;
    //@PlayerVsEnemy
    [SerializeField] protected float agility;
    [SerializeField] protected float acceleration;
    [SerializeField] protected float physicArmor;
    [SerializeField] protected WeaponData weapon;
    [Header("Stats - Minion?")]
    [SerializeField] protected float dropRate = 0.1f;
    [Header("Bonuses")]
    [SerializeField] List<Bonus> bonusList = new List<Bonus>();
    [SerializeField] List<String> bonusName = new List<String>();
    [SerializeField] List<Sprite> bonusSprite = new List<Sprite>();
    [Header("Components")]
    [SerializeField] public ParticleSystem attackParticles;
    [SerializeField] List<AttackModifier> modifiers = new List<AttackModifier>();
    [SerializeField] protected AnimatorController animator;

    public float HitRange { get => Weapon.HitRange; set => Weapon.HitRange = value; }
    public float MAX_HEALTH { get => maxHealth; set => maxHealth = value; }
    public float Health { get => health; set => health = value; }
    public float Speed { 
        get => speed; 
        set
        {
            speed = value;

            //@PlayerVsEnemy
            if (animator.GetType() == typeof(PlayerAnimatorController))
            {
                animator.GetComponentInParent<Player>().Motor.SetSpeed(speed);
            }
        }
    }
    public float PhysicArmor { get => physicArmor; set => physicArmor = value; }
    public float Strength { get => strength; set => strength = value; }
    public float CurrentDamage {
        get 
        { 
            return Weapon.Damage * Mathf.Min(Strength / Weapon.NeededStrength, PlayerSettings.MaxWeaponDamageBoostByStrength) * Weapon.NextHitDamageRatio; 
        }
    }
    //public float Cooldown { get => Weapon.CoolDown; set => Weapon.CoolDown = value; }
    public List<Bonus> BonusList { get => bonusList; set => bonusList = value; }
    public float DropRate { get => dropRate; set => dropRate = value; }
    public AnimatorController Animator { get => animator; set => animator = value; }
    public KnockbackData KnockBack { get => Weapon.KnockBack; set => Weapon.KnockBack = value; }
    public float Acceleration { get => acceleration; set => acceleration = value; }
    public WeaponData Weapon { get => weapon; set => weapon = value; }
    public float KnockBackRatio { get => knockBackRatio; set => knockBackRatio = value; }
    public float Agility
    {
        get => agility;
        set
        {
            agility = value;
            //@PlayerVsEnemy
            if (animator.GetType() == typeof(PlayerAnimatorController))
            {
                var playerController = (PlayerAnimatorController)animator;
                playerController.SetAgility((agility - weapon.Mass * 0.1f) / PlayerSettings.FastRollAgilityValue);
            }
        }     
    }

    public AttackData GetAttackData(Vector3 hitorigin, Hitable owner)
    {
        //var data = new AttackData(CurrentDamage, new KnockbackData(KnockBack, hitorigin), owner, modifiers);
        var data = Weapon.CurrentAttack;
        data.knockback.origin = hitorigin;
        data.origin = owner;
        return data;
    }

    virtual public void Attack(Transform owner, Transform hitPoint, LayerMask enemyLayer, LayerMask friendLayer, string animTrigger = "")
    {
        Weapon.TriggerWeapon(owner, hitPoint, enemyLayer, friendLayer, animator, animTrigger);
    }
    virtual public void HitTarget(Hitable victim, Vector3 hitOrigin)
    {
        victim.GetHit(GetAttackData(hitOrigin, null));
    }

    virtual public Dictionary<string, float> AddBonus(Bonus bonus)
    {
        Dictionary<string, float> bonusDisplay = new Dictionary<string, float>();
        if (bonus.StatBonus.strength != 0)
        {
            this.Strength += bonus.StatBonus.strength;
            bonusDisplay.Add("Strength", bonus.StatBonus.strength);
        }
        if (bonus.StatBonus.speed != 0)
        {
            this.Speed += bonus.StatBonus.speed;
            bonusDisplay.Add("Speed", bonus.StatBonus.speed);
        }
        if (bonus.StatBonus.agility != 0)
        {
            this.Agility += bonus.StatBonus.agility;
            bonusDisplay.Add("Agility", bonus.StatBonus.agility);
        }
        if (bonus.StatBonus.physicArmor != 0)
        {
            this.PhysicArmor += bonus.StatBonus.physicArmor;
            bonusDisplay.Add("Armor", bonus.StatBonus.physicArmor);
        }
        if (bonus.StatBonus.health != 0)
        {
            this.MAX_HEALTH += bonus.StatBonus.health;
            bonusDisplay.Add("Health", bonus.StatBonus.health);
        }

        modifiers.AddRange(bonus.Modifiers);

        bonusName.Add(bonus.name);
        bonusSprite.Add(bonus.Sprite);
        bonusList.Add(bonus);
        return bonusDisplay;
    }

}


[Serializable]
public class KnockbackData
{
    public float force;
    public float time;
    [NonSerialized] public Vector3 origin;

    public KnockbackData(KnockbackData data, Vector3 origin)
    {
        this.force = data.force;
        this.time = data.time;
        this.origin = origin;
    }
}
[Serializable]
public class AttackData
{
    public float damage;
    public KnockbackData knockback;
    public Hitable origin;
    public List<Status> statusList = new List<Status>();
    public int triggerId;

    public AttackData(float strength, KnockbackData knockback, Hitable origin, List<AttackModifier> modifiers, int triggerId)
    {
        this.damage = strength;
        this.knockback = knockback;
        this.origin = origin;
        this.triggerId = triggerId;
        foreach (AttackModifier mod in modifiers)
        {
            mod.ApplyModifier(this);
        }
    }

    internal void AddStatus(Status status)
    {
        statusList.Add(status);
    }
}
