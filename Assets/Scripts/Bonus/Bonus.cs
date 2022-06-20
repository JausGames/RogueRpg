using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public class StatBonus
{
    public float strength;
    public float speed;
    public float agility;
    public float physicArmor;
    public float health;

    public StatBonus(float strength, float speed, float agility, float physicArmor, float hitRadius, float hitRange, float coolDown, float health)
    {
        this.strength = strength;
        this.speed = speed;
        this.agility = agility;
        this.physicArmor = physicArmor;
        this.health = health;
    }
}

[CreateAssetMenu(fileName = "Power Up", menuName = "Bonus/Basic Power Up", order = 1)]
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
