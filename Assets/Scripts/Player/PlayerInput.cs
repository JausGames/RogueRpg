using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] PlayerController motor = null;
    [SerializeField] MoveHitable moveBody = null;
    [SerializeField] PlayerCombat combat = null;
    [SerializeField] Player player = null;
    [SerializeField] bool isAiming = false;
    [SerializeField] LayerMask Enemy = 7;
    [SerializeField] FolloMainCameraRotation freeCamera;
    [SerializeField] CinemachineFreeLook _freeLookComponent;
    [SerializeField] OrbitCamera orbitCamera = null;
    [SerializeField] Vector2 look;
    [Header("Rolling")]
    private float rollWait = 0.25f;


    private Controls controls;
    private bool attack;

    private Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    public Vector2 mouseLookSpeed = new Vector2(.5f, .25f);
    public Vector2 stickLookSpeed = new Vector2(7f, 4.5f);
    private float waitToSwitch;
    private float rollWaitTime;

    private void Update()
    {
        if (attack) KeepAttacking();

        if (_freeLookComponent != null)
        {
            if (look.magnitude > 0f)
            {
                _freeLookComponent.m_YAxisRecentering.m_enabled = false;
                _freeLookComponent.m_RecenterToTargetHeading.m_enabled = false;
                //Ajust axis values using look speed and Time.deltaTime so the look doesn't go faster if there is more FPS
                _freeLookComponent.m_XAxis.Value = Mathf.Lerp(_freeLookComponent.m_XAxis.Value, _freeLookComponent.m_XAxis.Value + look.x * 180f * Time.deltaTime, .2f);
                _freeLookComponent.m_YAxis.Value = Mathf.Lerp(_freeLookComponent.m_YAxis.Value, _freeLookComponent.m_YAxis.Value + look.y * Time.deltaTime, .2f);
            }
            else
            {
                _freeLookComponent.m_RecenterToTargetHeading.m_enabled = true;
                _freeLookComponent.m_YAxisRecentering.m_enabled = true;
            }
        }
    }
    private void Start()
    {

        player.TargetSetNullEvent.AddListener(
            delegate {
                TrySwitchTarget(out var aim, out var opp);
                player.isAiming(opp);

                if (aim == null) isAiming = false;
                SetCamera(aim);
            });
    }
    public void OnMove(CallbackContext context)
    {
        if (motor == null) return;
        var move = context.ReadValue<Vector2>();
        //motor.SetMove(move);
        moveBody.SetMovement(move.magnitude > .1f ? move : Vector2.zero);
    }
    public void OnLook(CallbackContext context)
    {
        if (!Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (isAiming) return;
        var input = context.ReadValue<Vector2>();
        look = new Vector2(stickLookSpeed.x * input.x, stickLookSpeed.y * input.y);
        //motor.SetLook(look);
        //freeCamera.SetLook(look);
        orbitCamera.SetLook(look.magnitude > .1f ? look : Vector2.zero);
    }


    public void OnMouseLook(CallbackContext context)
    {
        if (Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (isAiming) return;
        var input = context.ReadValue<Vector2>();
        //motor.SetLook(look);
        look = new Vector2(mouseLookSpeed.x * input.x, -mouseLookSpeed.y * input.y);
        orbitCamera.SetLook(look.magnitude > .1f ? look : Vector2.zero);
    }
    public void OnMouseSwitchAim(CallbackContext context)
    {
        var input = context.ReadValue<float>();
        //Debug.Log("PlayerInput, OnMouseSwitchAim : input = " + input);
        if (isAiming && input != 0)
        {
            SwitchTarget(out var aim, out var opp, input > 0f);
            player.isAiming(opp);

            if (aim == null) isAiming = false;
            SetCamera(aim);
        }
    }
    public void OnStickSwitchAim(CallbackContext context)
    {
        var input = context.ReadValue<Vector2>().x;
        //Debug.Log("PlayerInput, OnStickSwitchAim : input = " + input);

        if (isAiming && Mathf.Abs(input) > 0.9f)
        {
            SwitchTarget(out var aim, out var opp, input > 0f);
            player.isAiming(opp);

            if (aim == null) isAiming = false;
            SetCamera(aim);
        }
    }
    public void OnAttack(CallbackContext context)
    {
        if (player == null) return;
        attack = context.performed;
        if(attack) player.Attack(null);
    }
    public void KeepAttacking()
    {
        player.Attack(null);
    }
    public void OnBlocking(CallbackContext context)
    {
        if (player == null) return;
        var isPerformed = context.performed;
        var isCanc = context.canceled;
        if (isPerformed) player.StartBlocking(true);
        else if (isCanc) player.StartBlocking(false);
    }
    public void OnRoll(CallbackContext context)
    {
        if (player == null) return;
        if (context.performed) rollWaitTime = Time.time + rollWait;
        else if (context.canceled && Time.time < rollWaitTime)
        {
            player.StartRolling();
        }
    }
    public void OnAction(CallbackContext context)
    {
        if (player == null) return;
        var isPerformed = context.performed;
        if (isPerformed) player.TryAction();
    }
    public void OnMap(CallbackContext context)
    {
        if (player == null) return;
        var isPerformed = context.performed || !context.canceled;
        player.ShowMap(isPerformed);
    }
    public void OnAim(CallbackContext context)
    {
        if (player == null) return;
        var isPerformed = context.performed;
        if (isPerformed) isAiming = !isAiming;

        GetAim(out var aim, out var minion, isAiming);
        player.isAiming(minion);

        if (aim == null) isAiming = false;
        SetCamera(aim);
    }
    public void OnRun(CallbackContext context)
    {
        if (player == null) return;
        var isPerformed = !context.canceled;

        player.SetRunning(isPerformed);
    }

    private void SetCamera(Transform aim)
    {
        if (_freeLookComponent)
        {
            _freeLookComponent.LookAt = aim == null ? player.transform : aim;
            _freeLookComponent.m_RecenterToTargetHeading.m_WaitTime = isAiming ? 0f : 2f;
            _freeLookComponent.m_RecenterToTargetHeading.m_RecenteringTime = isAiming ? 0f : 2f;
            _freeLookComponent.m_YAxisRecentering.m_WaitTime = isAiming ? 0f : 2f;
            _freeLookComponent.m_YAxisRecentering.m_RecenteringTime = isAiming ? 0f : 2f;
            //_freeLookComponent.m_Orbits[1].m_Height = isAiming ? 1.8f : 2.4f;
            //_freeLookComponent.m_Orbits[1].m_Radius = isAiming ? 3f : 5f;
        }
    }

    private void GetAim(out Transform aim, out Minion opp, bool enable)
    {
        aim = null;
        opp = null;
        if(enable)
        {
            var cols = Physics.OverlapSphere(transform.position, 8f, Enemy);
            var angleList = new List<float>();
            var ennemies = new List<Minion>();

            for(int i = 0; i < cols.Length; i++)
            {
                var ennemy = cols[i].GetComponent<Minion>() ? cols[i].GetComponent<Minion>() : cols[i].GetComponentInParent<Minion>();
                if(ennemy.Moving)
                {
                    var currentDir = Camera.main.transform.forward;
                    currentDir -= currentDir.y * Vector3.up;

                    var targetDir = ennemy.transform.position - transform.position;
                    targetDir -= targetDir.y * Vector3.up;

                    var angle = Vector3.SignedAngle(currentDir, targetDir.normalized, transform.up);
                    if(angle < 75f && angle > -75f)
                    {
                        angleList.Add(Mathf.Abs(angle));
                        ennemies.Add(ennemy);
                    }

                }
            }

            if(angleList.Count > 0)
            {

                var id = FindSmallestAngle(angleList);
                aim = ennemies[id].transform;
                opp = ennemies[id];
            }
            else
                ResetCamera();
        }
        else
            ResetCamera();
    }

    private static int FindSmallestAngle(List<float> angleList)
    {
        Transform aim;
        var minIndex = 0;
        for (int i = 0; i < angleList.Count; i++)
        {
            if (angleList[i] < angleList[minIndex]) minIndex = i;
        }
        return minIndex;
    }

    private void SwitchTarget(out Transform aim, out Minion opp, bool checkRight)
    {
        aim = player.Target;
        opp = player.Target == null ? null : player.Target.GetComponent<Minion>() ? player.Target.GetComponent<Minion>() : player.Target.GetComponentInParent<Minion>(); ;
        if (Time.time < waitToSwitch + .3f) return;
        //Debug.Log("PlayerInput, SwitchTarget : Start");
        var cols = Physics.OverlapSphere(transform.position, 8f, Enemy);
        var angleList = new List<float>();
        var ennemies = new List<Minion>();

        for (int i = 0; i < cols.Length; i++)
        {
            var ennemy = cols[i].GetComponent<Minion>() ? cols[i].GetComponent<Minion>() : cols[i].GetComponentInParent<Minion>();
            if (ennemy.Moving && ennemy.transform != player.Target)
            {
                var currentDir = Camera.main.transform.forward;
                currentDir -= currentDir.y * Vector3.up;

                var targetDir = ennemy.transform.position - transform.position;
                targetDir -= targetDir.y * Vector3.up;

                var angle = Vector3.SignedAngle(currentDir, targetDir.normalized, transform.up);
                if ((checkRight ? (angle > 0f && angle < 90f) : (angle < 0f && angle > -90f)))
                {
                    Debug.Log("PlayerInput, SwitchTarget : add angle - ennemy = " + ennemy + ", angle = " + angle);
                    angle = Mathf.Abs(angle);
                    ennemies.Add(ennemy);
                    angleList.Add(angle);
                }
            }
        }
        if (angleList.Count > 0)
        {
            var id = FindSmallestAngle(angleList);
            aim = ennemies[id].transform;
            opp = ennemies[id];
            waitToSwitch = Time.time;
        }
    }
    private void TrySwitchTarget(out Transform aim, out Minion opp)
    {
        aim = null;
        opp = null;
        if (Time.time < waitToSwitch + .3f) return;
        //Debug.Log("PlayerInput, SwitchTarget : Start");
        var cols = Physics.OverlapSphere(transform.position, 8f, Enemy);
        var angleList = new List<float>();
        var ennemies = new List<Minion>();

        for (int i = 0; i < cols.Length; i++)
        {
            var ennemy = cols[i].GetComponent<Minion>() ? cols[i].GetComponent<Minion>() : cols[i].GetComponentInParent<Minion>();
            if (ennemy.Moving && ennemy.transform != player.Target)
            {
                var currentDir = Camera.main.transform.forward;
                currentDir -= currentDir.y * Vector3.up;

                var targetDir = ennemy.transform.position - transform.position;
                targetDir -= targetDir.y * Vector3.up;

                var angle = Vector3.SignedAngle(currentDir, targetDir.normalized, transform.up);
                if (angle < 90f && angle > -90f)
                {
                    Debug.Log("PlayerInput, SwitchTarget : add angle - ennemy = " + ennemy + ", angle = " + angle);
                    angle = Mathf.Abs(angle);
                    ennemies.Add(ennemy);
                    angleList.Add(angle);
                }
            }
        }
        if (angleList.Count > 0)
        {
            var id = FindSmallestAngle(angleList);
            aim = ennemies[id].transform;
            opp = ennemies[id];
            waitToSwitch = Time.time;
        }
    }

    private void ResetCamera()
    {
        var currentDir = Camera.main.transform.forward;
        currentDir -= currentDir.y * Vector3.up;

        var targetDir = player.transform.forward;
        targetDir -= targetDir.y * Vector3.up;

        var angle = Vector3.SignedAngle(currentDir, targetDir.normalized, transform.up);
        _freeLookComponent.m_XAxis.Value += angle / 2f;
        _freeLookComponent.m_YAxis.Value = 0.5f;
    }
}
