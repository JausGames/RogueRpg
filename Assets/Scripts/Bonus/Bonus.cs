using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public class StatBonus
{
    public float damage;
    public float speed;
    public float physicArmor;
    public float hitRadius;
    public float hitRange;
    public float coolDown;
    public float health;

    public StatBonus(float damage, float speed, float physicArmor, float hitRadius, float hitRange, float coolDown, float health)
    {
        this.damage = damage;
        this.speed = speed;
        this.physicArmor = physicArmor;
        this.hitRadius = hitRadius;
        this.hitRange = hitRange;
        this.coolDown = coolDown;
        this.health = health;
    }
}

[CreateAssetMenu(fileName = "Bonus", menuName = "Bonus/Basic Bonus", order = 1)]
public class Bonus : ScriptableObject
{
    [SerializeField] List<AttackModifier> modifiers = new List<AttackModifier>();
    [SerializeField] StatBonus bonus;
    [SerializeField] Sprite sprite;
    [SerializeField] Texture texture;
    [SerializeField] int basePrice;

    public Sprite Sprite { get => sprite; }
    public StatBonus StatBonus { get => bonus;}
    public List<AttackModifier> Modifiers { get => modifiers;}
    public int Price { get => basePrice;}
    public Texture Texture { get => texture; set => texture = value; }
}
