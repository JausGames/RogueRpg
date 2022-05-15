using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent updateArmyEvent;


    [SerializeField] Vector2 move = Vector2.zero;
    [SerializeField] Vector2 look = Vector2.zero;
    [SerializeField] float speed = 50f;
    [SerializeField] float maxSpeed = 15f;
    [SerializeField] bool isMoving = true;
    [SerializeField] bool canStop = true;
    [SerializeField] Rigidbody body;
    [SerializeField] AnimationCurve accelerationCurve;
    [SerializeField] PlayerAnimatorController animator;
    private float lastTimeNoLook;
    [SerializeField] private Transform cameraContainer;
    private float lastTimeLook;

    public bool CanStop { get => canStop; set => canStop = value; }
    public Rigidbody Body { get => body; set => body = value; }
    public bool IsMoving { get => isMoving; set => isMoving = value; }

    internal void SetSpeed(float speed)
    {
        maxSpeed = 3f * speed;
        this.speed = maxSpeed * 3.333f;
    }

    

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        if (updateArmyEvent == null)
            updateArmyEvent = new UnityEvent();
    }

    private void Update()
    {
        if (!isMoving) return;
        var currSpeed = body.velocity.sqrMagnitude;

        Debug.Log("Playercontroller, Update : currSpeed = " + currSpeed);

        var v3Look = look.x * transform.right + look.y * transform.forward;

        var lookAngle = Vector3.SignedAngle(transform.forward, v3Look, transform.up);
        if (look.magnitude > 0.05f)
        {
            var baseRot = cameraContainer.rotation;
            cameraContainer.Rotate(transform.up * lookAngle * .2f);
            cameraContainer.rotation = Quaternion.Lerp(baseRot, cameraContainer.rotation, Mathf.Min(Mathf.Pow(Time.time - lastTimeNoLook, 1f), 0.1f));
            lastTimeLook = Time.time;
        }
        else
        {
            lastTimeNoLook = Time.time;
            //body.angularVelocity /= 10f;
        }

        var v3Move = move.x * cameraContainer.right + move.y * cameraContainer.forward;

        if (move.magnitude > 0.1f && currSpeed < maxSpeed)
        {
            body.velocity = Vector3.Lerp(body.velocity, body.velocity.magnitude * v3Move, 0.1f);
            var speedFactor = accelerationCurve.Evaluate(currSpeed / maxSpeed);
            body.AddForce(v3Move * speed * speedFactor * Time.deltaTime, ForceMode.Impulse);

            Debug.Log("Playercontroller, Update : normal acc = " + v3Move * speed * speedFactor * Time.deltaTime);
            Debug.Log("Playercontroller, Update : normal acc speedFactor = " + speedFactor);
        }
        else if (move.magnitude > 0.1f && body.velocity.sqrMagnitude >= maxSpeed)
        {
            body.velocity = Vector3.Lerp(body.velocity, Mathf.Sqrt(maxSpeed) * v3Move, 0.1f);
            Debug.Log("Playercontroller, Update : max speed move = " + Mathf.Sqrt(maxSpeed) * v3Move);
        }
        else if(canStop)
        {
            body.velocity /= 10f;
        }

        var angle = Vector3.SignedAngle(transform.forward, v3Move, transform.up);
        if (move.magnitude > 0.05f)
        {
            var baseRot = transform.rotation;
            transform.Rotate(transform.up * angle * .2f);
            // transform.rotation = Quaternion.Lerp(baseRot, transform.rotation, Mathf.Min(Mathf.Pow(Time.time - lastTimeNoLook, 1f), 0.1f));
            transform.rotation = Quaternion.Lerp(baseRot, transform.rotation, 0.3f);
        }
        else
        {
            //lastTimeNoLook = Time.time;
            body.angularVelocity /= 10f;
        }
        if(Time.time - lastTimeLook > .5f)
        {
            var resetAngle = Vector3.SignedAngle(cameraContainer.forward, transform.forward, transform.up);
            var baseRot = cameraContainer.rotation;
            cameraContainer.Rotate(transform.up * resetAngle * .2f);
            cameraContainer.rotation = Quaternion.Lerp(baseRot, cameraContainer.rotation, Mathf.Min(Time.time - lastTimeLook - .5f, 1f) * 0.1f);
        }

        updateArmyEvent.Invoke();

        UpdateAnimator();
    }

    internal void StopMotion(bool isMoving)
    {
        if(!isMoving)
            body.velocity = Vector3.zero;
        this.isMoving = isMoving;
    }

    private void UpdateAnimator()
    {
        Debug.Log("PlayerControlelr, UpdateAnimator : speed clamped = " + (body.velocity.sqrMagnitude / maxSpeed));
        animator.SetControllerAnimator(body.velocity.sqrMagnitude / maxSpeed, move.magnitude > .1f);
    }

    public void SetMove(Vector2 move)
    {
        this.move = move;
    }

    public void SetLook(Vector2 look)
    {
        this.look = look;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)move);
    }
}
