using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : Hitable
{
    Army army;
    PlayerCombat combat;
    PlayerController motor;
    [SerializeField] MapUI mapUi;

    [SerializeField] LayerMask minionMask;
    [SerializeField] LayerMask doorLayer;
    [SerializeField] LayerMask interactLayer;
    [SerializeField] LayerMask pickableLayer;

    [SerializeField] PlayerWallet wallet;

    [SerializeField] HealthBar healthUI;
    [SerializeField] ParticleSystem actionParticle;
    [SerializeField] List<GameObject> hidableGo;
    [SerializeField] Text walletTxt;
    //[SerializeField] new private PlayerAnimatorController animator;

    public LayerMask MinionMask { get => minionMask; set => minionMask = value; }
    public PlayerWallet Wallet { get => wallet;}
    public PlayerController Motor { get => motor; set => motor = value; }

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
        wallet = new PlayerWallet(10);
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
    private void Update()
    {
        mapUi.SetPlayerPosition(transform.position.x / GridSettings.gridSize.x, transform.position.z / GridSettings.gridSize.y);
        //Check for pickable
        var cols = Physics.OverlapSphere(transform.position, .3f, pickableLayer);
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
        HideElementsBetween();
    }

    public void SetBlocking(bool value)
    {
        animator.SetBlocking(value);
        blocking = value;
    }

    public void StartRolling()
    {
        animator.Roll();
    }
    public void SetRolling(bool value)
    {
        Debug.Log("Player, SetRolling : value = " + value);
        rolling = value;
        if (value) Push(transform.forward * 5f);
    }

    void Start()
    {
        combat = GetComponent<PlayerCombat>();
        combat.CombatData = combatData;
        motor = GetComponent<PlayerController>();
        army = GetComponent<Army>();

        motor.SetSpeed(combatData.Speed);
        motor.updateArmyEvent.AddListener(delegate { army.SetMinionsPosition(transform.position, transform.forward); });

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

    public override void Attack(Hitable victim)
    {
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
        healthUI.SetHealth(combatData.Health);
    }
    public override void StopMotion(bool isMoving)
    {
        motor.StopMotion(isMoving);
    }
    protected override void ApplyStatus()
    {
        base.ApplyStatus();
        motor.IsMoving = true;
        for (int i = 0; i < currentStatusList.Count; i++)
        {
            if (currentStatusList[i].StatusType == Status.Type.Knock)
                motor.IsMoving = false;
        }

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


