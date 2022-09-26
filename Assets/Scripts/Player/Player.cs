using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class Player : Hitable
{


    [Header("Player - Component")]
    Army army;
    PlayerCombat combat;
    PlayerController motor;
    [SerializeField] MoveHitable moveBody;
    [SerializeField] PlayerWallet wallet;

    [Header("Player - Masks")]
    [SerializeField] LayerMask minionMask;
    [SerializeField] LayerMask doorLayer;
    [SerializeField] LayerMask interactLayer;
    [SerializeField] LayerMask pickableLayer;

    [Header("Player - UI")]
    [SerializeField] MapUi mapUi;
    [SerializeField] HealthBar healthUI;
    [SerializeField] List<GameObject> hidableGo;
    [SerializeField] Text walletTxt;

    [Header("Player - VFX")]
    [SerializeField] ParticleSystem actionParticle;
    [HideInInspector]
    public UnityEvent TargetSetNullEvent;

    //[SerializeField] new private PlayerAnimatorController animator;

    public LayerMask MinionMask { get => minionMask; set => minionMask = value; }
    public PlayerWallet Wallet { get => wallet;}
    public PlayerController Motor { get => motor; set => motor = value; }
    public Transform Target { get => motor.Target; }
    [HideInInspector]
    override public bool CanRotate { 
        get => canRotate; 
        set
            {
                canRotate = value;
                //motor.IsMoving = value;
            } 
    }
    [HideInInspector]
    override public bool Attacking { 
        get => Attacking; 
        set
        {
            attacking = value;
            motor.attacking = value;
            CanRotate = value;
            if (!value && motor.Running) motor.Running = false;
        } 
    }

    public void ResetForNewStage()
    {
        transform.position = Vector3.zero;
        army.ResetToOrigin();
    }
    override public void AddBonus(Bonus bonus)
    {
        combat.AddBonus(bonus);
    }
    public LayerMask GetFriendLayer()
    {
        return combat.FriendLayer;
    }
    public LayerMask GetEnemyLayer()
    {
        return combat.EnnemyLayer;
    }

    // Start is called before the first frame update
    private void Awake()
    {
        ragdoll.SetRagdollActive(false);
        CopyWeapon(combatData.Weapon);
        wallet = new PlayerWallet(PlayerSettings.StartMoney);
        UpdateWalletUi();
    }
    private void OnEnable()
    {
        wallet.onAmountChange += UpdateWalletUi;
    }

    private void UpdateWalletUi()
    {
        walletTxt.text = wallet.Amount.ToString();
    }

    private void OnDisable()
    {
        wallet.onAmountChange -= UpdateWalletUi;
    }
    public override void SetIsMoving(bool v)
    {
        motor.IsMoving = v;
    }
    private void Update()
    {
        var gridGen = FindObjectOfType<GridGenerator.GridGenerator>();
        if (gridGen)
            mapUi.SetPlayerPosition(transform.position.x , transform.position.z, gridGen.minimapCamera.orthographicSize);
        else
            mapUi.SetPlayerPosition(transform.position.x / GridSettings.gridSize.x, transform.position.z / GridSettings.gridSize.y);

        //Check for pickable
        var cols = Physics.OverlapSphere(transform.position, PlayerSettings.PickableRadiusCheck, pickableLayer);
        if(cols.Length > 0)
        {
            List<Pickable> pickables = new List<Pickable>();
            for(int i = 0; i < cols.Length; i++)
            {
                var pick = cols[i].GetComponent<PickableContainer>().Item;
                if (!pickables.Contains(pick))
                {
                    pickables.Add(pick);
                    pick.AddToPlayer(this);
                }
            }
        }
        /*if(Target != null)
        {

            SetTargetNull();
        }*/

        //HideElementsBetween();
    }

    public void StartBlocking(bool value)
    {
        Debug.Log("Player, SetBlocking : shield on ? " + value);
        animator.SetBlocking(value);
    }
    override public void SetBlocking(bool value)
    {
        Debug.Log("Player, SetBlocking : shield on ? " + value);
        if(!value)
            animator.SetBlocking(value);
        Blocking = value;
        motor.RotateWithLook(value);
        blockingShield.IsActive = value;
    }

    public void StartRolling()
    {
        if(Blocking)SetBlocking(false);
        if (motor.Running) motor.Running = false;
        Debug.Log("Player, StartRolling");
        animator.Roll(true);
    }
    public void StartRollingMovement()
    {
        AddStatus(new Status(Status.Type.Rolling, .4f, transform.forward * combatData.Speed * (combatData.Agility / 10f)));
        Debug.Log("Player, StartRollingMovement");
        SetIsMoving(false);
    }
    override public void SetRolling(bool value)
    {
        Debug.Log("Player, SetRolling : value = " + value);
        rolling = value;
    }

    void Start()
    {
        combat = GetComponent<PlayerCombat>();
        combat.CombatData = combatData;
        motor = GetComponent<PlayerController>();
        moveBody = GetComponent<MoveHitable>();
        army = GetComponent<Army>();
        // to set roll speed
        combatData.Agility = combatData.Agility;

        motor.SetAcceleration(combatData.Acceleration);
        motor.SetMaxSpeed(combatData.Speed);

        healthUI.SetMaxHealth(combatData.MAX_HEALTH);
        healthUI.SetHealth(combatData.Health);
    }
    internal void TryAction()
    {
        var cols = Physics.OverlapCapsule(transform.position, transform.position + 2f * transform.transform.forward, 0.5f,  interactLayer | doorLayer);
        
        if (cols.Length > 0)
        {
            var closest = cols[0];
            for (int i = 1; i < cols.Length; i++)
            {
                if ((transform.position - cols[i].transform.position).sqrMagnitude < (transform.position - closest.transform.position).sqrMagnitude) closest = cols[i];
            }

            var interactable = closest.GetComponent<Interactable>();
            interactable.OnInteract(this);
        }
    }

    internal void isAiming(Minion target)
    {
        if (Target && target) 
            if(target.transform == Target.transform) 
                return;

        if (target != null) target.dieEvent.RemoveListener(SetTargetNull);
        motor.Target = target == null ? null : target.transform;
        if (target != null) target.dieEvent.AddListener(SetTargetNull);
    }

    internal void SetRunning(bool isPerformed)
    {

        if (isPerformed && animator.CanRun()) motor.Running = isPerformed;
        else if (isPerformed) motor.WaitToRun = isPerformed;
        else
        {
            motor.WaitToRun = isPerformed;
            motor.Running = isPerformed;
        }
    }

    private void SetTargetNull()
    {
        motor.Target = null;
        TargetSetNullEvent.Invoke();
    }

    public override void Attack(Hitable victim)
    {
        if (Blocking) SetBlocking(false);
        if (motor.Running) motor.Running = false;
        combat.Attack();
    }

    public void AddMinionToArmy(Minion minion)
    {
        minion.Owner = this;
        army.AddMinion(minion);
        army.SetMinionsPosition(transform.position, transform.forward);
    }

    internal void ShowMap(bool isPressed)
    {
        mapUi.SetWholeScreenMap(isPressed);
        foreach(GameObject go in hidableGo)
        {
            go.SetActive(isPressed);
        }
    }
    public override void GetHit(float damage)
    {
        base.GetHit(damage);
        healthUI.SetHealth(combatData.Health);
    }
    public override void GetHit(AttackData attackData)
    {
        base.GetHit(attackData);
        if(!Blocking && !Countering)animator.GetHit();
        healthUI.SetHealth(combatData.Health);
    }
    public override void StopMotion(bool isMoving)
    {
        motor.StopMotion(isMoving);
    }
    protected override void ApplyStatus()
    {
        base.ApplyStatus();
        /*motor.IsMoving = true;
        for (int i = 0; i < currentStatusList.Count; i++)
        {
            if (currentStatusList[i].StatusType == Status.Type.Knock)
                motor.IsMoving = false;
        }*/

    }
}

public class PlayerWallet
{
    int amount = 0;
    public delegate void Amountchange();
    public event Amountchange onAmountChange;

    public PlayerWallet(int amount)
    {
        this.amount = amount;
    }

    public int Amount { get => amount;}
    public bool RemoveMoney(int amount)
    {
        if (this.amount < amount) return false;

        this.amount -= amount;
        onAmountChange();
        return true;
    }
    public void AddMoney(int amount)
    {
        this.amount += amount;
        onAmountChange();
    }
}


