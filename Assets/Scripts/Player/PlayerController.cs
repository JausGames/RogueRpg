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
    [SerializeField] public bool attacking = false;
    [SerializeField] AnimationCurve accelerationCurve;
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
            var angle = Vector3.SignedAngle(transform.forward, targetDir.normalized , transform.up);
            var baseRot = transform.rotation;
            transform.Rotate(transform.up * angle * .2f);
            transform.rotation = Quaternion.Lerp(baseRot, transform.rotation, 0.2f);
        }
        else if (input.magnitude > 0.05f && !rotateWithLook)
        {
            var move = (input.x * orbitCamera.transform.right + input.y * orbitCamera.transform.forward).normalized;
            var angle = Vector3.SignedAngle(transform.forward, move, transform.up);
            var baseRot = transform.rotation;
            transform.Rotate(transform.up * angle * .2f);
            transform.rotation = Quaternion.Lerp(baseRot, transform.rotation, .7f);
        }
        else if(orbitCamera.Input.magnitude > 0.05f && rotateWithLook)
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

    internal void SetMaxSpeed(float speed)
    {
        /*maxSpeed = speed;
        maxRunSpeed = speed * 4f;*/
    }
    internal void SetAcceleration(float acceleration)
    {
        /*maxAcceleration = acceleration;
        maxAirAcceleration = acceleration * .1f;*/
    }

    public void RotateWithLook(bool value)
    {
        rotateWithLook = value;
    }

    internal void StopMotion(bool isMoving)
    {
        if(!isMoving)
            body.velocity = Vector3.zero;
        this.isMoving = isMoving;
    }

    private void UpdateAnimator()
    {
        var velocity = body.velocity.sqrMagnitude / PlayerSettings.MaxAnimationSpeed;
        /*var frontRatio = Vector3.Dot(body.velocity.normalized, transform.forward);
        var sideRatio = Vector3.Dot(body.velocity.normalized, transform.right);*/
        var frontRatio = body.velocity.sqrMagnitude / PlayerSettings.MaxAnimationSpeed;
        var sideRatio = 0f;

        /*Debug.Log("PlayerControlelr, UpdateAnimator : ratio front = " + frontRatio);
        Debug.Log("PlayerControlelr, UpdateAnimator : ratio side = " + sideRatio);*/

        animator.SetControllerAnimator(velocity, input.magnitude > .1f && isMoving, frontRatio, sideRatio);
    }
}
