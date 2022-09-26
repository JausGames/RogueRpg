using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static UnityEngine.InputSystem.InputAction;

public class BasicInput : MonoBehaviour
{
    [SerializeField] MoveHitable motor = null;
    [SerializeField] OrbitCamera orbitCamera = null;


    private Controls controls;

    private Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    public void OnMove(CallbackContext context)
    {
        if (motor == null) return;
        var move = context.ReadValue<Vector2>();
        motor.SetMovement(move);
    }
    public void OnLook(CallbackContext context)
    {
        if (orbitCamera == null) return;
        var look = context.ReadValue<Vector2>();
        orbitCamera.SetLook(look);
    }

}
