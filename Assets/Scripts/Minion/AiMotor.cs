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

    private void Start()
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
            var dir = (destination - transform.position).normalized;
            if ((destination - transform.position).magnitude < toleranceRadius)
            { 
                body.velocity /= breakForce; 
                if(body.velocity.magnitude < .05f)
                {
                    body.velocity = Vector3.zero;
                    destinationReachedOrUnreachable.Invoke();
                }
                //var ag = GetComponent<NavMeshAgent>().path.
            }
            else if((!(Vector3.Angle(dir, body.velocity) < 30f) && Vector3.Angle(dir, body.velocity) > 45f) && body.velocity.magnitude > .2f)
                body.velocity /= breakForce;
            else if (body.velocity.magnitude >= speed || Vector3.Angle(dir, body.velocity) > 15f)
                body.velocity = dir * speed;
            else
                body.AddForce(dir * acceleration * Time.deltaTime, ForceMode.VelocityChange);
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
}
