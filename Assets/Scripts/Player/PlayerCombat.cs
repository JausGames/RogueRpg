using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] CombatData combatData;
    [SerializeField] CombatDataUi ui;
    [SerializeField] public Transform hitPoint;
    [SerializeField] private LayerMask ennemyLayer;
    [SerializeField] private LayerMask friendLayer;

    public CombatData CombatData { get => combatData; set { combatData = value; ui.SetUpStat(combatData); } }

    public LayerMask EnnemyLayer { get => ennemyLayer; set => ennemyLayer = value; }
    public LayerMask FriendLayer { get => friendLayer; set => friendLayer = value; }

    public void Attack()
    {
        combatData.Attack(transform, hitPoint, ennemyLayer, friendLayer);
    }

    internal void AddBonus(Bonus bonus)
    {
        combatData.AddBonus(bonus);
        ui.UpdateStatUi(combatData, bonus);
    }
}
