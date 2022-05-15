using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WeaponTrigger : MonoBehaviour
{
    [SerializeField] Hitable owner;
    [SerializeField] List<Hitable> touchedList;
    [SerializeField] bool isActive = false;

    public bool IsActive { get => isActive; set { isActive = value; touchedList.Clear(); } }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        Debug.Log("WeaponTrigger, OnTriggerEnter : other = " + other.gameObject);
        if (other.GetComponent<Hitable>() || other.GetComponentInParent<Hitable>())
        {
            var minion = other.GetComponent<Hitable>() ? other.GetComponent<Hitable>() : other.GetComponentInParent<Hitable>();
            if (minion == owner && !touchedList.Contains(minion)) return;

            minion.GetHit(owner.CombatData.GetAttackData(owner.transform.position, owner));
           touchedList.Add(minion);

            Debug.Log("WeaponTrigger, OnTriggerEnter : " + minion.gameObject);
        }
    }

    public void ResetSwing()
    {
        touchedList.Clear();
    }
}
