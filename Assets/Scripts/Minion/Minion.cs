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
    [Header("Status")]
    [SerializeField] private bool fighting = false;
    [SerializeField] private bool attacking = false;
    [Header("Minion characteristic")]
    [SerializeField] Type type;
    [Header("Component")]
    [SerializeField] public Transform hitPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask friendLayer;
    [SerializeField] new public BeetlingAnimatorController animator;
    [SerializeField] Player owner;
    [SerializeField] Vector3 randomDestination;
    [SerializeField] AiMotor motor;
    [Header("Timers")]
    [SerializeField] bool waitToAttack = false;
    [SerializeField] float attackTimer;
    [SerializeField] bool waitToMove = false;
    [SerializeField] float moveTimer;

    [HideInInspector]

    public Player Owner { get => owner; set => owner = value; }
    public bool Fighting { get => fighting; set => fighting = value; }
    public Type MinionType { get => type; set => type = value; }
    public LayerMask EnemyLayer { get => enemyLayer; set => enemyLayer = value; }
    public LayerMask FriendLayer { get => friendLayer; set => friendLayer = value; }
    public AiMotor Motor { get => motor; set => motor = value; }
    public bool Attacking { get => attacking; set => attacking = value; }

    private void Awake()
    {

        CopyWeapon(combatData.Weapon);
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
        waitToAttack = false;
        waitToMove = false;
    }
    private void Update()
    {
        if(waitToAttack && attackTimer + .5f < Time.time && !frozen)
        {
            Attack(null);
            moveTimer = Time.time;
            waitToMove = true;
            waitToAttack = false;
        }
        if (waitToMove && moveTimer + .3f < Time.time && !frozen)
        {
            Moving = true;
            waitToMove = false;
        }

        if (!Moving || waitToAttack || frozen) return;

        var cols = Physics.OverlapSphere(transform.position, combatData.HitRange, enemyLayer);

        if (cols.Length > 0)
        {
            Fighting = true;

            var offset = owner ? (owner.transform.position - cols[0].transform.position).normalized * combatData.HitRange : Vector3.zero;
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
            waitToAttack = true;
        }
        else if (cols.Length == 0)
        {
            Fighting = false;
            var player = FindObjectOfType<Player>();

            if (owner == null && player != null)
            {
                cols = Physics.OverlapSphere(transform.position, 3f, enemyLayer);

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
            animator.SetController(speed.magnitude);
        }
        else if(Mathf.Abs(speed.x) < Mathf.Abs(speed.z) && Mathf.Abs(speed.z) > 0.2f)
        {
            var Direction = speed.z < 0 ? Minion.Direction.Down : Minion.Direction.Up;
            animator.SetController(speed.magnitude);
        }
        else
            animator.SetController(0f);
    }

    public void SetPosition(Vector3 position)
    {
        if (!moving || !motor.enabled) return;
        motor.Destination = position;
    }

    public void SetRotation(Vector3 direction)
    {
        if (!moving) return;
        var angle = Vector3.SignedAngle(transform.forward, direction, transform.up);
        transform.Rotate(transform.up * angle);
    }
    public override void Attack(Hitable victim)
    {
        if (!moving) return;
        Attacking = false;
        combatData.Attack(transform, hitPoint, enemyLayer, friendLayer);
    }
    public Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 direction, Vector2 point)
    {
        direction.Normalize();
        Vector2 lhs = point - origin;

        float dotP = Vector2.Dot(lhs, direction);
        return origin + direction * dotP;
    }
    protected override void Die(Vector3 force)
    {
        if (!owner)
        {
            var dropRateRND = UnityEngine.Random.Range(0f, 1f);

            if (dropRateRND < CombatData.DropRate + 0.9f)
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
        motor.enabled = false;
        Moving = false;
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
            animator.PlayAnimation(true);
            Moving = true;
            frozen = false;
            motor.IsActive = true;
        }
        else
        {
            animator.PlayAnimation(false);
            Moving = false;
            motor.Body.velocity = Vector3.zero;
            motor.IsActive = false;
        }
    }
}
