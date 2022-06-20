using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Minion : Hitable
{
    public enum Type{
        Warrior,
        Tank,
        Range,
        Support
    }
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Null
    }


    //[SerializeField] public float nextHit = 0f;
    [Header("Minion - Status")]
    [SerializeField] private bool fighting = false;
    [SerializeField] private bool attacking = false;
    [SerializeField] private bool canRotate = true;
    [Space]
    [Header("Minion - Ai")]
    [SerializeField] private float DetectionRadius = 8f;
    [Header("Minion - Combat")]
    [SerializeField] public Transform hitPoint;
    [Space]
    [Header("Minion - Masks")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask friendLayer;
    //[SerializeField] new public EnnemyAnimatorController animator;
    [Space]
    [Header("Minion - Component")]
    [SerializeField] Player owner;
    [SerializeField] KnockdownModule koModule;
    [HideInInspector] Vector3 randomDestination;
    [SerializeField] AiMotor motor;
    [Space]
    [Header("Minion - Timers")]
    [SerializeField] bool waitToAttack = false;
    [SerializeField] float attackTimer;
    [SerializeField] bool waitToSmallAttack;
    [SerializeField] float endOfAttack;

    [SerializeField] bool waitToMove = false;
    [SerializeField] float moveTimer;
    //private float attackDelay = .1f;
    private float attackDelay = 0;
    private float animationDelay = 1f;
    private Hitable target;

    public Player Owner { get => owner; set => owner = value; }
    public bool Fighting { get => fighting; set => fighting = value; }
    public LayerMask EnemyLayer { get => enemyLayer; set => enemyLayer = value; }
    public LayerMask FriendLayer { get => friendLayer; set => friendLayer = value; }
    public AiMotor Motor { get => motor; set => motor = value; }
    public bool Attacking { get => attacking; set => attacking = value; }
    public Hitable Target
    {
        get => target; 
        set
        {
            target = value;
            if (target!= null) target.dieEvent.AddListener(delegate { target = null; });
        }
    }
    public bool CanRotate { get => canRotate; set => canRotate = value; }

    private void Awake()
    {

        CopyWeapon(combatData.Weapon);
        ragdoll = GetComponentInChildren<RagdollManager>();
        ragdoll.SetRagdollActive(false); 
        if (motor)
        {
            motor.Speed = combatData.Speed;
            motor.Acceleration = combatData.Acceleration;
            motor.DestinationReached.AddListener(delegate { randomDestination = transform.position; });
        }
        randomDestination = transform.position;
        //StopMotion(false);
    }
    public override void GetHit(AttackData attackData)
    {
        base.GetHit(attackData);
        koModule.knockdownTolerance -= attackData.damage;

        if(koModule.knockdownTolerance <= 0)
        {
            koModule.startRecoveryTimer = Time.time + koModule.koRecoveryDelay;
            koModule.knockdownTolerance = koModule.MAX_KO_TOLERANCE;
            animator.GetHit();
        }

        waitToAttack = false;
        waitToMove = false;
    }
    private void Update()
    {
        if (koModule.knockdownTolerance < koModule.MAX_KO_TOLERANCE && Time.time >= koModule.startRecoveryTimer)
            koModule.knockdownTolerance = Mathf.Min(koModule.MAX_KO_TOLERANCE, koModule.knockdownTolerance + Time.deltaTime * koModule.koRecoverySpeed * 0.0001f);

        if((waitToAttack || waitToSmallAttack) && attackTimer + attackDelay < Time.time && !frozen)
        {
            if (Vector3.Dot(transform.forward, (target.transform.position - transform.position).normalized) < 0.95f)
                TryRotatingToFuturTargetPosition();
            else
            {
                if (waitToAttack) 
                    Attack(null);
                if (waitToSmallAttack)
                    Attack(null, "SmallAttack");
                moveTimer = Time.time;
                endOfAttack = Time.time + animationDelay;
                SetMotorEnable(false);
                motor.Body.velocity = Vector3.zero;
                waitToMove = true;
                waitToAttack = false;
                waitToSmallAttack = false;
                return;
            }
        }
        else if(waitToAttack && Target)
        {
            TryRotatingToFuturTargetPosition();
        }
        if (!waitToAttack && waitToMove && moveTimer + 1.5f < Time.time && !frozen)
        {
            SetMotorEnable(true);
            waitToMove = false;
        }
        else if (waitToMove && Target)
        {
            TryRotatingToFuturTargetPosition();
        }

        if (!Moving || waitToAttack || waitToMove || frozen) return;

        var cols = Physics.OverlapSphere(transform.position, combatData.HitRange, enemyLayer);
        var player = FindObjectOfType<Player>();
        

        if (cols.Length > 0)
        {
            Fighting = true;

            var offset = owner ? (owner.transform.position - cols[0].transform.position).normalized * combatData.HitRange : Vector3.zero;
            Target = cols[0].GetComponent<Hitable>();
            SetPosition(cols[0].transform.position + offset);
            if (combatData.Weapon.GetType() == typeof(RangeWeapon))
            {
                var rangeWeapon = (RangeWeapon)combatData.Weapon;
                var opponentVelocity = cols[0].GetComponent<Minion>() ? cols[0].GetComponent<NavMeshAgent>().velocity : (Vector3)cols[0].GetComponent<Rigidbody>().velocity;
                var opponentPosition = cols[0].transform.position;
                var opponentLastPosition = (cols[0].transform.position + opponentVelocity.normalized * opponentVelocity.magnitude * combatData.HitRange * (1f / rangeWeapon.ProjectileSpeed));
                //var opponentFuturePosition = FindNearestPointOnLine(cols[0].transform.position, opponentVelocity, hitPoint.position);
                var opponentFuturePosition = opponentPosition;

                var oppTravelTime = (opponentPosition - (Vector3)opponentFuturePosition).magnitude / opponentVelocity.magnitude;
                var bulletTravelTime = (hitPoint.position - (Vector3)opponentFuturePosition).magnitude / rangeWeapon.ProjectileSpeed;

                var it = 0;
                var delta = 50f;
                while (oppTravelTime < bulletTravelTime && opponentFuturePosition != opponentLastPosition && it < 80)
                {
                    if ((opponentFuturePosition - opponentLastPosition).magnitude < Time.deltaTime) opponentFuturePosition = opponentLastPosition;
                    opponentFuturePosition += (opponentLastPosition - opponentFuturePosition).normalized * Time.deltaTime * delta;
                    oppTravelTime = (opponentPosition - opponentFuturePosition).magnitude / opponentVelocity.magnitude;
                    bulletTravelTime = (hitPoint.position - opponentFuturePosition).magnitude / rangeWeapon.ProjectileSpeed;
                    it++;
                }
                Debug.Log("it = " + it);



                Debug.DrawLine(transform.position, opponentFuturePosition, Color.red);
                Debug.DrawLine(transform.position, opponentLastPosition, Color.black);
                Debug.DrawLine(opponentPosition, opponentFuturePosition, Color.cyan);
                //Debug.DrawLine(opponentPosition, opponentLastPosition, Color.blue);
                var rot = ((Vector3)opponentFuturePosition - transform.position).x * Vector3.right + ((Vector3)opponentFuturePosition - transform.position).z * Vector3.forward;
                SetRotation(rot.normalized);
            }
            else
            {
                var rot = (cols[0].transform.position - transform.position).x * Vector3.right + (cols[0].transform.position - transform.position).z * Vector3.forward;
                SetRotation(rot.normalized);
            }
            motor.Destination = transform.position;
            attackTimer = Time.time;
            waitToSmallAttack = true;
        }
        else if (cols.Length == 0)
        {
            if (combatData.Weapon.GetType() == typeof(PhysicalWeapon) && player && !waitToSmallAttack)
            {
                Vector3 rot = IsOpponentTouchableWithDelay(player);
                if (rot != Vector3.zero)
                {
                    Fighting = true;
                    SetRotation(rot.normalized);
                    motor.Destination = transform.position;
                    attackTimer = Time.time;
                    waitToAttack = true;
                    Debug.Log("Minion, Player : in thefuture");
                    return;
                }
            }
            Fighting = false;

            if (owner == null && player != null)
            {
                cols = Physics.OverlapSphere(transform.position, DetectionRadius, enemyLayer);

                if (cols.Length > 0)
                {
                    SetPosition(cols[0].transform.position);
                    var rot = (cols[0].transform.position - transform.position).x * Vector3.right + (cols[0].transform.position - transform.position).z * Vector3.forward;
                    SetRotation(rot.normalized);
                }
                else
                {
                    Debug.DrawLine(transform.position, randomDestination);
                    if ((transform.position - randomDestination).sqrMagnitude < 0.2f)
                    {
                        var rndX = UnityEngine.Random.Range(-1f, 1f);
                        var rndY = UnityEngine.Random.Range(-1f, 1f);
                        var distance = 5f;
                        randomDestination = (rndX * Vector3.right + rndY * Vector3.forward).normalized * distance + transform.position;
                        
                        SetPosition(randomDestination);
                        var rot = (randomDestination - transform.position).x * Vector3.right + (randomDestination - transform.position).z * Vector3.forward;
                        SetRotation(rot.normalized);
                    }
                }
            }
        }
        UpdateAnimator();
    }

    private void TryRotatingToFuturTargetPosition()
    {
        var futurPos = (Vector3)target.transform.position - transform.position + target.GetComponent<Rigidbody>().velocity * (Time.time - endOfAttack);
        var rot = futurPos.x * Vector3.right + futurPos.z * Vector3.forward;
        SetRotation(rot.normalized);
    }

    private void SetMotorEnable(bool value)
    {
        motor.IsActive = value;
        Moving = value;
    }

    private Vector3 IsOpponentTouchableWithDelay(Hitable target)
    {
        this.Target = target;
        var targetVelocity = target.GetComponent<Minion>() ? target.GetComponent<NavMeshAgent>().velocity : (Vector3)target.GetComponent<Rigidbody>().velocity;
        var targetPosition = target.transform.position;
        var hitPoint = transform.position;
        //Last possible position of the target that we could touched
        //based on target speed, and attack animation delay
        //var targetLastPosition = (target.transform.position + targetVelocity.normalized * targetVelocity.magnitude * combatData.HitRange * (1f / animationDelay));
        var targetFuturePosition = FindNearestPointOnLine(targetPosition, targetVelocity, hitPoint);
        var futurDistance = (targetFuturePosition - hitPoint).magnitude;
        //var targetFuturePosition = targetPosition;

        var oppTravelTime = (targetPosition - (Vector3)targetFuturePosition).magnitude / targetVelocity.magnitude;
        Debug.Log("Minion, IsOpponentTouchableWithDelay : futureDistance = " + futurDistance);
        Debug.Log("Minion, IsOpponentTouchableWithDelay : oppTravelTime = " + oppTravelTime);
        if (futurDistance > CombatData.HitRange || oppTravelTime > animationDelay || oppTravelTime <= animationDelay * 0.7f) return Vector3.zero;

        Debug.DrawLine(transform.position, targetFuturePosition, Color.red, 1f);
        //Debug.DrawLine(transform.position, targetLastPosition, Color.black, 1f);
        Debug.DrawLine(targetPosition, targetFuturePosition, Color.cyan, 1f);
        //Debug.DrawLine(targetPosition, targetLastPosition, Color.blue);
        var rot = ((Vector3)targetFuturePosition - transform.position).x * Vector3.right + ((Vector3)targetFuturePosition - transform.position).z * Vector3.forward;
        return rot;
    }

    public void SetLayers(LayerMask friend, LayerMask enemy)
    {
        enemyLayer = enemy;
        friendLayer = friend;
    }
    private void UpdateAnimator()
    {
        var speed = motor.Body.velocity;
        if(Mathf.Abs(speed.x) > Mathf.Abs(speed.z) && Mathf.Abs(speed.x) > 0.2f)
        {
            var Direction = speed.x < 0 ? Minion.Direction.Left : Minion.Direction.Right;
            var animator = (EnnemyAnimatorController) base.animator;
            animator.SetController(speed.magnitude);
        }
        else if(Mathf.Abs(speed.x) < Mathf.Abs(speed.z) && Mathf.Abs(speed.z) > 0.2f)
        {
            var Direction = speed.z < 0 ? Minion.Direction.Down : Minion.Direction.Up;
            var animator = (EnnemyAnimatorController)base.animator;
            animator.SetController(speed.magnitude);
        }
        else
        {
            var animator = (EnnemyAnimatorController)base.animator;
            animator.SetController(0f);
        }
    }

    public void SetPosition(Vector3 position)
    {
        if (!moving || !motor.enabled) return;
        motor.Destination = position;
    }

    public void SetRotation(Vector3 direction)
    {
        if (!canRotate) return;
        var angle = Vector3.SignedAngle(transform.forward, direction, transform.up);
        var currentRot = transform.rotation;
        transform.Rotate(transform.up * angle);
        var futurRot = transform.rotation;
        transform.rotation = currentRot;
        transform.rotation = Quaternion.Lerp(currentRot, futurRot, .2f);
    }
    public override void Attack(Hitable victim)
    {
        if (!moving) return;
        Attacking = false;
        combatData.Attack(transform, hitPoint, enemyLayer, friendLayer);
    }
    public void Attack(Hitable victim, string animTrigger = "")
    {
        if (!moving) return;
        Attacking = false;
        combatData.Attack(transform, hitPoint, enemyLayer, friendLayer, animTrigger);
    }
    public Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 direction, Vector3 point)
    {
        direction.Normalize();
        Vector3 lhs = point - origin;

        float dotP = Vector3.Dot(lhs, direction);
        return origin + direction * dotP;
    }
    protected override void Die(Vector3 force)
    {
        if (!owner)
        {
            var dropRateRND = UnityEngine.Random.Range(0f, 1f);

            if (dropRateRND < CombatData.DropRate)
            {
                var GO = Instantiate(Resources.Load<GameObject>("Prefabs/Pickables/PickableContainer"), transform.position, Quaternion.identity);
                var pickables = Resources.LoadAll<Pickable>("Prefabs/Pickables/Presets");
                var rates = new List<float>();
                var totalRate = 0f;
                for (int i = 0; i < pickables.Length; i++)
                {
                    rates.Add(pickables[i].Rate);
                    totalRate += pickables[i].Rate;
                }
                var rnd = UnityEngine.Random.Range(0f, totalRate);
                var result = 0f;
                var container = GO.GetComponent<PickableContainer>();

                for (int i = 0; i < rates.Count; i++)
                {
                    var sumRate = 0f;
                    for (int j = 0; j <= i; j++)
                    {
                        sumRate += rates[j];
                    }
                    if (rnd <= sumRate)
                    {
                        container.Item = Instantiate(pickables[i], transform.position, GO.transform.rotation, GO.transform);
                        break;
                    }
                }

                var prtcRenderer = container.Particle.GetComponent<ParticleSystemRenderer>();
                var mat = new Material(prtcRenderer.material);
                mat.mainTexture = container.Item.Texture;
                prtcRenderer.material = mat;
                //GO.GetComponentInChildren<SpriteRenderer>().sprite = GO.GetComponentInChildren<PickableContainer>().Item.Sprite;

            }
        }
        SetMotorEnable(false);
        base.Die(force);

    }
    void SetLayerToChildrens(Transform transform, LayerMask layer)
    {
        transform.gameObject.layer = layer;
        for (int i = 0; i < transform.childCount; i++)
        {
            SetLayerToChildrens(transform.GetChild(i), layer);
        }
    }
    public override void StopMotion(bool isMoving)
    {
        if (frozen) return;
        if(isMoving)
        {
            var animator = (EnnemyAnimatorController)base.animator;
            animator.PlayAnimation(true);
            SetMotorEnable(true);
            frozen = false;
        }
        else
        {
            var animator = (EnnemyAnimatorController)base.animator;
            animator.PlayAnimation(false);
            SetMotorEnable(false);
            motor.Body.velocity = Vector3.zero;
        }
    }
}
[Serializable]
public class KnockdownModule
{
    public float knockdownTolerance = 20f;
    public float MAX_KO_TOLERANCE = 20f;
    public float koRecoverySpeed = 1f;
    public float koRecoveryDelay = .5f;
    public float startRecoveryTimer;
}