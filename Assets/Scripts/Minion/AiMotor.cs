using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AiMotor : MonoBehaviour
{
    UnityEvent destinationReachedOrUnreachable = new UnityEvent();
    [SerializeField] Rigidbody body;
    [SerializeField] Vector3 destination = Vector3.zero;
    [SerializeField] Vector3[] bounding = new Vector3[4];
    [SerializeField] float acceleration;
    [SerializeField] float speed;
    [SerializeField] float breakForce;
    [SerializeField] float toleranceRadius;
    [SerializeField] float maxTravelTime = 2f;
    [SerializeField] float currentTravelTime = 0f;
    [SerializeField] bool isActive = true;
    private Vector3 directionInAxis;
    private Vector3 velocityInAxis;
    private Vector3 moveAxisY;
    private Vector3 moveAxisX;
    private Vector3 moveAxisZ;

    public float Acceleration { get => acceleration; set => acceleration = value; }
    public float Speed { get => speed; set => speed = value; }
    public Vector3 Destination
    {
        get => destination; 
        set
        {
            currentTravelTime = Time.time;
            destination = ClampDestination(value);
        }
    }
    public Rigidbody Body { get => body; set => body = value; }
    public Vector3[] Bounding { get => bounding; set => bounding = value; }
    public UnityEvent DestinationReached { get => destinationReachedOrUnreachable; set => destinationReachedOrUnreachable = value; }
    public bool IsActive { get => isActive; set => isActive = value; }

    private void Awake()
    {
        if(bounding[0] == Vector3.zero
            && bounding[1] == Vector3.zero
            && bounding[2] == Vector3.zero
            && bounding[3] == Vector3.zero)
        {
            bounding[0] = new Vector3(Mathf.Infinity, 0, Mathf.Infinity);
            bounding[1] = new Vector3(Mathf.Infinity, 0, -Mathf.Infinity);
            bounding[2] = new Vector3(-Mathf.Infinity, 0, -Mathf.Infinity);
            bounding[3] = new Vector3(-Mathf.Infinity, 0, Mathf.Infinity);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isActive) return;
        if (destination != Vector3.zero)
        {
            if (Time.time - currentTravelTime > maxTravelTime) destinationReachedOrUnreachable.Invoke();
            if ((destination - transform.position).magnitude < toleranceRadius)
            {
                body.velocity /= breakForce;
                if (body.velocity.magnitude < .05f)
                {
                    body.velocity = Vector3.zero;
                    destinationReachedOrUnreachable.Invoke();
                }
                //var ag = GetComponent<NavMeshAgent>().path.
            }
            else
            {
                var origin = transform.position + Vector3.up;
                var dir = Vector3.down;
                var onFloor = Physics.Raycast(origin, dir, out var hit, 1.2f, (1 << 19) | (1 << 12) | (1 << 11), QueryTriggerInteraction.UseGlobal);
                if (onFloor)
                {
                    var horizontalDir = (destination - transform.position).normalized;
                    horizontalDir -= horizontalDir.y * Vector3.up;
                    horizontalDir = horizontalDir.normalized;

                    moveAxisY = hit.normal;
                    var changeAxisAngle = Quaternion.FromToRotation(Vector3.up, moveAxisY);
                    moveAxisX = changeAxisAngle * Vector3.right;
                    moveAxisZ = changeAxisAngle * Vector3.forward;
                    var horizontalSpeed = body.velocity.x * Vector3.right + body.velocity.z * Vector3.forward;
                    directionInAxis = (horizontalDir.x * moveAxisX + horizontalDir.z * moveAxisZ).normalized;
                    velocityInAxis = (body.velocity.x * moveAxisX + body.velocity.y * moveAxisY + body.velocity.z * moveAxisZ);


                    body.velocity = Vector3.Lerp(body.velocity, directionInAxis * horizontalSpeed.magnitude + body.velocity.y * Vector3.up, 0.2f);
                    var gravityCounter = body.useGravity ? -Physics.gravity : Vector3.zero;
                    body.AddForce(directionInAxis * acceleration * Time.deltaTime + gravityCounter, ForceMode.Acceleration);
                }
            }
        }
    }
    public Vector3 ClampDestination(Vector3 destination)
    {
        Debug.Log("ClampDestination : dest = " + destination);/*
        destination.x = Mathf.Max(destination.x, bounding[2].x + 1f);
        destination.x = Mathf.Min(destination.x, bounding[0].x - 1f);
        destination.z = Mathf.Max(destination.z, bounding[2].z + 1f);
        destination.z = Mathf.Min(destination.z, bounding[0].z - 1f);*/
        Debug.Log("ClampDestination : dest = " + destination);
        /*destination.x = Mathf.Max(Mathf.Min(destination.x, bounding[0].x), bounding[3].x);
        destination.z = Mathf.Max(Mathf.Min(destination.z, bounding[0].z), bounding[3].z);*/
        return destination;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + moveAxisY);
        Gizmos.color = Color.Lerp(Color.green, Color.white, .3f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + moveAxisZ);
        Gizmos.color = Color.Lerp(Color.blue, Color.white, .3f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.forward);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + moveAxisX);
        Gizmos.color = Color.Lerp(Color.red, Color.white, .3f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + velocityInAxis);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + directionInAxis * 1.2f);
    }
    }
