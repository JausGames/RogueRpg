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
    [Header("Stats - Speed")]
    [SerializeField] protected float speed;
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
    public float Speed { get => speed; set => speed = value; }
    public float PhysicArmor { get => physicArmor; set => physicArmor = value; }
    public float Damage { get => Weapon.Damage; set => Weapon.Damage = value; }
    public float Cooldown { get => Weapon.CoolDown; set => Weapon.CoolDown = value; }
    public List<Bonus> BonusList { get => bonusList; set => bonusList = value; }
    public float DropRate { get => dropRate; set => dropRate = value; }
    public AnimatorController Animator { get => animator; set => animator = value; }
    public KnockbackData KnockBack { get => Weapon.KnockBack; set => Weapon.KnockBack = value; }
    public float Acceleration { get => acceleration; set => acceleration = value; }
    public WeaponData Weapon { get => weapon; set => weapon = value; }

    /*private void Awake()
    {
        health = MAX_HEALTH;
        weapon = CopyWeapon(weapon);
    }
    private WeaponData CopyWeapon(WeaponData data)
    {
        return Instantiate(data);
    }*/

    public AttackData GetAttackData(Vector3 hitorigin, Hitable owner)
    {
        var data = new AttackData(Weapon.Damage, new KnockbackData(KnockBack, hitorigin), owner, modifiers);
        return data;
    }


    virtual public void Attack(Transform owner, Transform hitPoint, LayerMask enemyLayer, LayerMask friendLayer)
    {
        Weapon.TriggerWeapon(owner, hitPoint, enemyLayer, friendLayer, animator);
    }
    virtual public void HitTarget(Hitable victim, Vector3 hitOrigin)
    {
        victim.GetHit(GetAttackData(hitOrigin, null));
    }

    virtual public void AddBonus(Bonus bonus)
    {
        this.Weapon.Damage += bonus.StatBonus.damage;
        this.speed += bonus.StatBonus.speed;
        this.physicArmor += bonus.StatBonus.physicArmor;
        this.Weapon.HitRange += bonus.StatBonus.hitRange;
        this.Weapon.CoolDown += bonus.StatBonus.coolDown;
        this.health += bonus.StatBonus.health;

        modifiers.AddRange(bonus.Modifiers);

        bonusName.Add(bonus.name);
        bonusSprite.Add(bonus.Sprite);
        bonusList.Add(bonus);
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
public class AttackData
{
    public float damage;
    public KnockbackData knockback;
    public Hitable origin;
    public List<Status> statusList = new List<Status>();

    public AttackData(float damage, KnockbackData knockback, Hitable origin, List<AttackModifier> modifiers)
    {
        this.damage = damage;
        this.knockback = knockback;
        this.origin = origin;
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
