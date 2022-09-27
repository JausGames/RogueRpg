using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHitable : MonoBehaviour
{
    //can finish jump if you want
    [SerializeField]
    Transform inputSpace = default;
    protected Vector2 input;
    protected Vector3 velocity, desiredVelocity;
    [SerializeField, Range(0f,100f)]
    protected float maxAcceleration = 10f, maxAirAcceleration = 1f;
    [SerializeField] AnimationCurve accelerationCurve;
    [SerializeField, Range(0f, 100f)]
    protected float maxSpeed = 10f;
    protected Rigidbody body;
    bool OnGround => groundContactCount > 0;
    bool ApplyCounterGravity => true;

    public Rigidbody Body { get => body; set => body = value; }

    [Header("Ground detection")]
    int groundContactCount, steepContactCount;

    [SerializeField, Range(0f, 90f)]
    float maxGroundAngle = 25f, maxStairsAngle = 50f;
    float minGroundDotProduct, minStairsDotProduct;
    Vector3 contactNormal, steepNormal;

    int stepsSinceLastGrounded, stepsSinceLastJump;
    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;

    [SerializeField, Min(0f)]
    float probeDistance = 1f;
    [SerializeField]
    LayerMask probeMask = -1, stairsMask = -1;

    [Header("Speed reduction by angle")]
    [SerializeField, Range(0f, 1f)]
    protected float minDotProductToReduceSpeed = .5f;
    [SerializeField, Range(0f, 1f)]
    protected float speedReductionImpact = .5f;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        OnValidate();
    }
    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
    }


    private void Update()
    {
        if (inputSpace)
        {
            {
                Vector3 forward = inputSpace.forward;
                forward.y = 0f;
                forward.Normalize();
                Vector3 right = inputSpace.right;
                right.y = 0f;
                right.Normalize();
                desiredVelocity = (forward * input.y + right * input.x) * maxSpeed;
            }
        }
        else
        {
            desiredVelocity = new Vector3(input.x, 0f, input.y) * maxSpeed;
        }
    }

    private void FixedUpdate()
    {
        UpdateState();
        AdjustVelocity();

        body.velocity = velocity;

        //because it is the first thing done in physics 
        ClearState();
    }
    virtual protected void UpdateState()
    {
        stepsSinceLastGrounded += 1;
        //stepsSinceLastJump += 1;
        velocity = body.velocity;
        if (OnGround || SnapToGround() || CheckSteepContacts())
        {
            stepsSinceLastGrounded = 0;
            //jumpPhase = 0;
            if (groundContactCount > 1)
                contactNormal.Normalize();
        }
		else 
        {
			contactNormal = Vector3.up;
		}
    }
    bool SnapToGround()
    {
        if (stepsSinceLastGrounded > 1)
        {
            return false;
        }
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed)
        {
            return false;
        }
        if (!Physics.Raycast(body.position, Vector3.down, out RaycastHit hit, probeDistance, probeMask))
        {
            return false;
        }
        if (hit.normal.y < minGroundDotProduct)
        {
            return false;
        }
        groundContactCount = 1;
        contactNormal = hit.normal;
        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f)
        {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        return false;
    }
    internal void SetMovement(Vector2 move)
    {
        input = move.normalized;
    }
    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }
    protected void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        Debug.DrawLine(transform.position + Vector3.up * 1f, transform.position + Vector3.up * 1f + xAxis, Color.white);
        Debug.DrawLine(transform.position + Vector3.up * 1f, transform.position + Vector3.up * 1f + zAxis, Color.white);

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        var currenctVect = new Vector2(velocity.x, velocity.z);
        Debug.DrawLine(transform.position + Vector3.up * 2f, transform.position + Vector3.up * 2f + new Vector3(currenctVect.x, 0,currenctVect.y), Color.red);
        Debug.DrawLine(transform.position + Vector3.up * 3f, transform.position + Vector3.up * 3f + desiredVelocity, Color.blue);

        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        Vector3 counterGravity = OnGround && ApplyCounterGravity ? -Physics.gravity * Time.deltaTime : Vector3.zero;
        float maxSpeedChange = acceleration * accelerationCurve.Evaluate(velocity.magnitude) * Time.deltaTime;

        var curentDotProduct = Vector2.Dot(currenctVect.normalized, new Vector2(desiredVelocity.x, desiredVelocity.z).normalized);
        var angle = Vector2.SignedAngle(currenctVect.normalized, new Vector2(desiredVelocity.x, desiredVelocity.z).normalized);
        var counterDir = Vector2.Perpendicular(currenctVect * Mathf.Sign(angle));
        var counterDirV3 = new Vector3(counterDir.x, 0, counterDir.y).normalized;
        Debug.DrawLine(transform.position + Vector3.up * 2f, transform.position + Vector3.up * 2f + counterDirV3, Color.yellow);
        curentDotProduct = curentDotProduct < minDotProductToReduceSpeed ? (curentDotProduct - minDotProductToReduceSpeed) / -(1 + minDotProductToReduceSpeed) : 0f;

        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);
        Debug.Log("Dot product = " + curentDotProduct);
        var perfectVelocity = new Vector3(velocity.x, 0, velocity.z).magnitude * desiredVelocity.normalized + velocity.y * Vector3.up;

        velocity = (speedReductionImpact * (1f-curentDotProduct )* perfectVelocity) + ((1f-speedReductionImpact) * velocity);
        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ) + counterGravity;
    }
    protected void ClearState()
    {
        groundContactCount = steepContactCount = 0;
        contactNormal = steepNormal = Vector3.zero;
    }
    bool CheckSteepContacts()
    {
        if (steepContactCount > 1)
        {
            steepNormal.Normalize();
            if (steepNormal.y >= minGroundDotProduct)
            {
                groundContactCount = 1;
                contactNormal = steepNormal;
                return true;
            }
        }
        return false;
    }
    /*void Jump()
    {
        if (OnGround || jumpPhase < maxAirJumps)
        {
            stepsSinceLastJump = 0;
            jumpPhase += 1;
		}
    }*/

    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }
    void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }
    void EvaluateCollision(Collision collision)
    {
        float minDot = GetMinDot(collision.gameObject.layer);
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            //onGround |= normal.y >= minGroundDotProduct;
            if (normal.y >= minDot)
            {
                groundContactCount += 1;
                contactNormal += normal;
            }
            else if (normal.y > -0.01f)
            {
                steepContactCount += 1;
                steepNormal += normal;
            }
        }
    }
    float GetMinDot(int layer)
    {
        return (stairsMask & (1 << layer)) == 0 ?
            minGroundDotProduct : minStairsDotProduct;
    }
}
