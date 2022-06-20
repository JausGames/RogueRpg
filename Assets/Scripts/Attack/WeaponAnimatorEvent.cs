using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimatorEvent : MonoBehaviour
{
    [SerializeField] List<WeaponTrigger> triggers;
    [SerializeField] Hitable owner;
    [SerializeField] Animator animator;

    public void SetTriggerActive(int attackId)
    {
        Debug.Log("WeaponAnimatorEvent, SetTriggerActive : attackid " + attackId);
        owner.CombatData.Weapon.CurrentAttack = owner.CombatData.Weapon.AttackSet[attackId];
        triggers[owner.CombatData.Weapon.CurrentAttack.triggerId].IsActive = true;
        owner.CombatData.Weapon.ResetCombo();
    }
    public void SetTriggerInactive()
    {
        foreach(WeaponTrigger tr in triggers)
        {
            tr.IsActive = false;
        }
        //owner.CombatData.Weapon.ResetCombo();
    }
}
