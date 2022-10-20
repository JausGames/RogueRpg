using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class PlayerController : MoveHitable
{
    [SerializeField] bool isMoving = true;
    [SerializeField] bool canStop = true;
    [SerializeField] private bool attacking = false;
    [SerializeField] OrbitCamera orbitCamera;
    [SerializeField] PlayerAnimatorController animator;
    private bool rotateWithLook;

    [SerializeField] Transform target;
    [SerializeField] private bool running;
    private bool waitToRun;
    private float maxRunSpeed;

    public bool CanStop { get => canStop; set => canStop = value; }
    public bool IsMoving { get => isMoving; set => isMoving = value; }
    public Transform Target { get => target; set => target = value; }
    public bool Running { get => running; set => running = value; }
    public bool WaitToRun { get => waitToRun; set => waitToRun = value; }
    public bool Attacking { get => attacking; set => attacking = value; }
    public Vector2 Input { get => input; set => input = value; }

    protected override void UpdateState()
    {
        base.UpdateState();
    }

    private void FixedUpdate()
    {
        if (!isMoving)
        {
            //if(canStop) body.velocity = new Vector3(body.velocity.x / 1.1f, body.velocity.y, body.velocity.z / 1.1f );
            return;
        }
        // BASE
        UpdateState();
        AdjustVelocity();

        body.velocity = velocity;

        //because it is the first thing done in physics 
        ClearState();

        // BASE END
        if (waitToRun)
        {
            if (animator.CanRun()) running = true;
        }

        if (target != null)
        {
            var targetDir = target.position - transform.position;
            targetDir -= targetDir.y * Vector3.up;
            var angle = Vector3.SignedAngle(transform.forward, targetDir.normalized, transform.up);
            var baseRot = transform.rotation;
            transform.Rotate(transform.up * angle * .2f);
            transform.rotation = Quaternion.Lerp(baseRot, transform.rotation, 0.2f);
        }
        else if (input.magnitude > 0.05f && !rotateWithLook)
        {
            var right = orbitCamera.transform.right;
            right.y = 0f;
            right.Normalize();
            var forward = orbitCamera.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            var move = (input.x * right + input.y * forward).normalized;
            var angle = Vector3.SignedAngle(transform.forward, move, transform.up);
            var baseRot = transform.rotation;
            transform.Rotate(transform.up * angle * .2f);
            transform.rotation = Quaternion.Lerp(baseRot, transform.rotation, .7f);
        }
        else if (orbitCamera.Input.magnitude > 0.05f && rotateWithLook)
        {
            var angle = Vector3.SignedAngle(transform.forward, Camera.main.transform.forward, transform.up);
            var baseRot = transform.rotation;
            transform.Rotate(transform.up * angle * .2f);
            transform.rotation = Quaternion.Lerp(baseRot, transform.rotation, .7f);
        }
        else
        {
            body.angularVelocity /= 10f;
        }

        UpdateAnimator();
    }
    protected override float DetermineMaxSpeed()
    {
        return OnGround ? !attacking ? running ? maxRunSpeed : maxSpeed : maxSpeed * .3f : maxSpeed * .1f;
    }
    protected override float DetermineMaxSpeedChange()
    {
        float acceleration = OnGround ? !attacking ? running ? maxAcceleration * 2f : maxAcceleration : maxAcceleration * 1f : maxAirAcceleration;
        var currMaxSpeed = DetermineMaxSpeed();
        float maxSpeedChange = acceleration * accelerationCurve.Evaluate(velocity.magnitude / currMaxSpeed) * Time.deltaTime;
        return maxSpeedChange;
    }
    internal void SetMaxSpeed(float speed)
    {
        maxSpeed = speed;
        maxRunSpeed = speed * 2f;
    }
    internal void SetAcceleration(float acceleration)
    {
        maxAcceleration = acceleration;
        maxAirAcceleration = acceleration * .1f;
    }

    public void RotateWithLook(bool value)
    {
        rotateWithLook = value;
    }

    internal void StopMotion(bool isMoving)
    {
        if (!isMoving)
            body.velocity = Vector3.zero;
        this.isMoving = isMoving;
    }

    private void UpdateAnimator()
    {
        var velocity = body.velocity.sqrMagnitude / PlayerSettings.MaxAnimationSpeed;
        var horizontalvelocity = new Vector3(body.velocity.x, 0f, body.velocity.z).normalized;
        var horizontalForward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        var horizontalRight = new Vector3(transform.right.x, 0f, transform.right.z).normalized;
        var frontDot = Vector3.Dot(horizontalvelocity, horizontalForward);
        var sideDot = Vector3.Dot(horizontalvelocity, horizontalRight);

        Debug.Log("Front dot = " + frontDot + ", side dot = " + sideDot);
        var sideForwardRatio = Mathf.Abs(frontDot);

        animator.SetControllerAnimator(velocity, input.magnitude > .1f && isMoving, frontDot, sideDot, sideForwardRatio);
    }
}
