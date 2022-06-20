using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WeaponTrigger : MonoBehaviour
{
    [SerializeField] Hitable owner;
    [SerializeField] List<Hitable> touchedList;
    [SerializeField] List<ParticleSystem> particles;
    [SerializeField] bool isActive = false;

    public bool IsActive { get => isActive;
        set
        {
            isActive = value;
            touchedList.Clear();
            if (value)
                foreach (ParticleSystem prtcl in particles)
                    prtcl.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;


        if (other.GetComponent<Hitable>() || other.GetComponentInParent<Hitable>())
        {
            var minion = other.GetComponent<Hitable>() ? other.GetComponent<Hitable>() : other.GetComponentInParent<Hitable>();
            if (minion == owner || touchedList.Contains(minion)) return;

            minion.GetHit(owner.CombatData.GetAttackData(owner.transform.position, owner));
           touchedList.Add(minion);

            Debug.Log("WeaponTrigger, OnTriggerEnter : hitable " + minion.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!isActive) return;


        if (other.GetComponent<Hitable>() || other.GetComponentInParent<Hitable>())
        {
            var minion = other.GetComponent<Hitable>() ? other.GetComponent<Hitable>() : other.GetComponentInParent<Hitable>();
            if (minion == owner || touchedList.Contains(minion)) return;

            minion.GetHit(owner.CombatData.GetAttackData(owner.transform.position, owner));
            touchedList.Add(minion);

            Debug.Log("WeaponTrigger, OnTriggerEnter : hitable " + minion.gameObject);
        }
    }

    public void ResetSwing()
    {
        touchedList.Clear();
    }
}
