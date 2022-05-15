using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimatorEvent : MonoBehaviour
{
    [SerializeField] WeaponTrigger trigger;
    [SerializeField] Hitable owner;

    public void SetTriggerActive()
    {
        Debug.Log("SetTrigger active");
        trigger.IsActive = true;
        owner.CombatData.Weapon.ResetCooldown();
    }
    public void SetTriggerInactive()
    {
        trigger.IsActive = false;
        owner.CombatData.Weapon.ResetCooldown();
    }
}
