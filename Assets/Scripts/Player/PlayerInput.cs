using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] PlayerController motor = null;
    [SerializeField] PlayerCombat combat = null;
    [SerializeField] Player player = null;

    public bool is3D = true;

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


    private void FixedUpdate()
    {
        if (attack) KeepAttacking();    
    }
    public void OnMove(CallbackContext context)
    {
        if (motor == null) return;
        var move = context.ReadValue<Vector2>();
        motor.SetMove(move);
    }
    public void OnLook(CallbackContext context)
    {
        if (motor == null) return;
        if (!Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        var look = context.ReadValue<Vector2>();
        motor.SetLook(look);
    }
    public void OnMouseLook(CallbackContext context)
    {
        if (motor == null) return;
        if(Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        var look = (Vector3) context.ReadValue<Vector2>();
        look.z = 20f;
        var lookPos = Camera.main.ScreenToWorldPoint(look);
        var playerPos = Camera.main.ScreenToWorldPoint(Screen.width * 0.5f * Vector3.right + Screen.height * Vector3.up);

        playerPos = playerPos.x * Vector3.right + playerPos.z * Vector3.forward;
        lookPos = lookPos.x * Vector3.right + lookPos.z * Vector3.forward;

        var mousePlayerDelta = lookPos - playerPos;

        //var mousePlayerDelta = Camera.main.ScreenToWorldPoint(look);
        //var mousePlayerDelta = look;
        Debug.Log("OnMouseLook, mousePlayerDelta = " + mousePlayerDelta);
        if(is3D) mousePlayerDelta = mousePlayerDelta.x * Vector2.right + mousePlayerDelta.z * Vector2.up;
        motor.SetLook((Vector2)context.ReadValue<Vector2>());
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
        var isPerformed = context.performed;
        if (isPerformed) player.StartRolling();
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
}
