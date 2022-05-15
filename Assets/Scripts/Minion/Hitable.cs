using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

abstract public class Hitable : MonoBehaviour
{
    public UnityEvent dieEvent;
    [SerializeField] protected bool moving = false;
    [SerializeField] protected bool blocking = false;
    [SerializeField] protected bool rolling = false;
    [SerializeField] protected CombatData combatData;
    [SerializeField] protected List<Status> currentStatusList = new List<Status>();
    [SerializeField] private List<Renderer> ObjsBetweenThisAndCamera;
    [SerializeField] protected RagdollManager ragdoll;
    [SerializeField] protected AnimatorController animator;

    [SerializeField] protected GameObject hitVfx;

    public bool Moving { get => moving; set => moving = value; }
    public CombatData CombatData { get => combatData; set => combatData = value; }

    abstract public void Attack(Hitable victim);

    protected void CopyWeapon(WeaponData data)
    {
        combatData.Weapon = Instantiate(data);
    }

    virtual protected void Die()
    {
        Moving = false;
        dieEvent.Invoke();
        ragdoll.SetRagdollActive(true);
        this.enabled = false;
        Destroy(gameObject, 1.5f);
    }
    virtual protected void Die(Vector3 force)
    {
        Moving = false;
        dieEvent.Invoke();
        ragdoll.SetRagdollActive(true, force);
        this.enabled = false;
        Destroy(gameObject, 1.5f);
    }
    virtual public void GetHit(float damage)
    {
        if (combatData.Health == 0f) return;
        var damageWithArmor = Mathf.Min(damage - combatData.PhysicArmor);
        combatData.Health = Mathf.Max(combatData.Health - damageWithArmor, 0f);

        if (combatData.Health == 0f)
            Die(Vector3.zero);
    }

    virtual public void GetHit(AttackData attackData)
    {
        if (blocking)
        {
            attackData.origin.GetBlock();
            return;
        }
        if (rolling)
            return;
        animator.GetHit();
        Instantiate(hitVfx, transform.position + Vector3.up, Quaternion.Euler(transform.position.x - attackData.knockback.origin.x, transform.position.y - attackData.knockback.origin.y, transform.position.z - attackData.knockback.origin.z), null);
        if (combatData.Health == 0f) return;
        var damageWithArmor = Mathf.Min(attackData.damage - combatData.PhysicArmor);
        combatData.Health = Mathf.Max(combatData.Health - damageWithArmor, 0f);


        for (int i = 0; i < attackData.statusList.Count; i++)
        {
            if (!ContainStatus(attackData.statusList[i]))
            {
                var newStatus = new Status(attackData.statusList[i]);

                currentStatusList.Add(newStatus);
                newStatus.onStatusEnd.AddListener(delegate {
                    currentStatusList.Remove(newStatus);
                });
            }
        }
        var force = Vector3.zero;
        if(attackData.knockback.force > 0f)
        {
            force = (this.transform.position - attackData.knockback.origin).normalized * attackData.knockback.force;
            //this.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);

            var newStatus = new Status(Status.Type.Knock, attackData.knockback.time);
            newStatus.Force = force;
            currentStatusList.Add(newStatus);
            newStatus.onStatusEnd.AddListener(delegate {
                currentStatusList.Remove(newStatus);
            });
        }

        if (combatData.Health == 0f)
            Die(force);
    }

    private void GetBlock()
    {
        animator.GetBlock();
    }

    protected void HideElementsBetween()
    {
        var newObjs = CheckObjectBetween();
        var a = .3f;
        foreach (Renderer rd in newObjs)
        {
            if (!ObjsBetweenThisAndCamera.Contains(rd))
            {
                ChangeTransparency(rd, a);
            }
        }
        a = 1f;
        foreach (Renderer rd in ObjsBetweenThisAndCamera)
        {
            if (!newObjs.Contains(rd))
            {
                ChangeTransparency(rd, a);
            }
        }

        ObjsBetweenThisAndCamera = newObjs;
    }

    public void Push(Vector3 force)
    {
        if(this is Minion)
        {
            var selfMin = (Minion)this;
            //selfMin.Motor.Body.AddForce(force, ForceMode.Force);
            selfMin.Motor.Body.velocity = force;
        }    
        else if(this is Player)
        {
            var selfPlayer = (Player)this;
            //selfPlayer.Motor.Body.AddForce(force, ForceMode.Force);
            selfPlayer.Motor.Body.velocity = force;
        }
    }

    private static void ChangeTransparency(Renderer rd, float a)
    {
        Color tempColor = rd.material.color;
        tempColor.a = a;
        rd.material.color = tempColor;
    }

    protected List<Renderer> CheckObjectBetween()
    {
        var camPos = Camera.main.transform.position;

        RaycastHit[] hits;
        hits = Physics.RaycastAll(camPos, (transform.position - camPos), (transform.position - camPos).magnitude, 3);
        Debug.DrawRay(camPos, (transform.position - camPos), Color.red, 1f, true);
        var returnList = new List<Renderer>();

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            Renderer rend = hit.transform.GetComponent<Renderer>();

            if(rend) returnList.Add(rend);
        }

        return returnList;
    }
    bool ContainStatus(Status status)
    {
        foreach(Status state in currentStatusList)
            if (state.StatusType == status.StatusType) return true;
        
        return false;
    }
    Status FindStatusByType(Status.Type statusType)
    {
        foreach (Status state in currentStatusList)
            if (state.StatusType == statusType) return state;

        return null;
    }
    private void LateUpdate()
    {
        ApplyStatus();
    }

    public virtual void StopMotion(bool isMoving) { }
         
    virtual public void AddBonus(Bonus bonus)
    {
        combatData.AddBonus(bonus);
    }

    virtual protected void ApplyStatus()
    {
        for(int i = 0; i < currentStatusList.Count; i++)
        {
            currentStatusList[i].ApplyStatus(this);
        }
    }
}
