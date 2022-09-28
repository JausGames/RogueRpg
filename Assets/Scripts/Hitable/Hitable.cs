using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using UnityEngine.Events;

abstract public class Hitable : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent dieEvent;
    [Header("Base - Status")]
    [SerializeField] protected bool moving = false;
    [SerializeField] protected bool attacking = false;
    [SerializeField] protected bool frozen = false;
    [SerializeField] protected bool rolling = false;
    [SerializeField] protected bool canRotate = true;
    [Space]
    [Header("Base - Combat")]
    [SerializeField] protected CombatData combatData;
    [SerializeField] protected List<Status> currentStatusList = new List<Status>();
    [HideInInspector] private List<Renderer> ObjsBetweenThisAndCamera;
    [Space]
    [Header("Base - Component")]
    [SerializeField] protected Shield blockingShield;
    [SerializeField] protected RagdollManager ragdoll;
    [SerializeField] protected AnimatorController animator;
    [Space]
    [Header("Base - VFX")]
    [SerializeField] protected GameObject hitVfx;

    public bool Moving { get => moving; set => moving = value; }
    public CombatData CombatData { get => combatData; set => combatData = value; }
    public bool Frozen { get => frozen; set => frozen = value; }
    public List<Status> CurrentStatusList { get => currentStatusList; set => currentStatusList = value; }
    protected bool Blocking { get => blockingShield.IsActive; set => blockingShield.IsActive = value; }
    protected bool Countering { get => blockingShield.IsCounter; set => blockingShield.IsCounter = value; }
    virtual public bool CanRotate { get => canRotate; set => canRotate = value; }
    virtual public bool Attacking { get => attacking; set => attacking = value; }

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
        Debug.Log("Hitable, GetHit : trigger");
        var damageWithArmor = Mathf.Min(damage - combatData.PhysicArmor);
        combatData.Health = Mathf.Max(combatData.Health - damageWithArmor, 0f);

        if (combatData.Health == 0f)
            Die(Vector3.zero);
    }

    virtual public void SetIsMoving(bool v)
    {
        
    }

    virtual public void SetRolling(bool v)
    {
        rolling = v;
    }

    virtual public void GetHit(AttackData attackData)
    {
        if (Countering && CheckIsBlocked(attackData.origin.transform.position - transform.position))
        {
            attackData.origin.GetBlock();
            return;
        }
        else if(Blocking && CheckIsBlocked(attackData.origin.transform.position - transform.position))
        {
            attackData.damage = attackData.damage * (1f - blockingShield.ShieldPower * 0.01f);
        }

        if (rolling)
            return;

        ThrowBloodVfx(attackData);

        if (combatData.Health == 0f) return;

        Vector3 force = ApplyAttackData(attackData);

        if (combatData.Health == 0f)
            Die(force);
    }

    private bool CheckIsBlocked(Vector3 origin)
    {
        var angle = Vector3.Angle(transform.forward, origin);
        var isBlocked = false;

        Debug.Log("Hitable, GetHit : Try Block, angle =  " + angle);
        if (angle < 45f)
        {
            isBlocked = true;
        }

        return isBlocked;
    }

    private Vector3 ApplyAttackData(AttackData attackData)
    {
        var damageWithArmor = attackData.damage * (100f - combatData.PhysicArmor) * 0.01f;
        combatData.Health = Mathf.Max(combatData.Health - damageWithArmor, 0f);


        for (int i = 0; i < attackData.statusList.Count; i++)
        {
            if (!ContainStatus(attackData.statusList[i]))
            {
                AddStatus(attackData.statusList[i]);
            }
        }
        var force = Vector3.zero;
        if (attackData.knockback.force > 0f)
        {
            force = (this.transform.position - attackData.knockback.origin).normalized * attackData.knockback.force * combatData.KnockBackRatio;
            //this.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);

            AddStatus(new Status(Status.Type.Knock, attackData.knockback.time * combatData.KnockBackRatio, force));
        }

        return force;
    }

    private void ThrowBloodVfx(AttackData attackData)
    {
        var vfxGo = Instantiate(hitVfx, transform.position + Vector3.up, Quaternion.Euler(transform.position.x - attackData.knockback.origin.x, transform.position.y - attackData.knockback.origin.y, transform.position.z - attackData.knockback.origin.z), null);
        Destroy(vfxGo, 6f);
    }

    public void AddStatus(Status it)
    {
        var newStatus = new Status(it);

        currentStatusList.Add(newStatus);
        newStatus.onStatusEnd.AddListener(delegate
        {
            currentStatusList.Remove(newStatus);
        });
    }

    public void GetBlock()
    {
        Debug.Log("Hitable, GetBlock : trigger");
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
    virtual public void SetBlocking(bool value)
    {
        Blocking = value;
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
    protected bool ContainStatus(Status status)
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

    public virtual void StopMotion(bool isMoving) {}
         
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
