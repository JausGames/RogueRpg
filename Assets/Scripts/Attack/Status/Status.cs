using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Status", menuName = "Bonus/Status", order = 3)]
public class Status : ScriptableObject
{
    public UnityEvent onStatusEnd = new UnityEvent();
    [SerializeField] Type type;
    [SerializeField] float length;
    [SerializeField] float frequency;

    [SerializeField] GameObject particle;

    float startTime = 0f;
    float lastApplication = 0f;


    Vector3 force = Vector3.zero;

    public Status(Status status)
    {
        
        this.type = status.type;
        this.length = status.length;
        this.frequency = status.frequency;
        this.particle = status.particle;
    }
    public Status(Type type, float length)
    {

        this.type = type;
        this.length = length;
    }

    public Type StatusType { get => type; set => type = value; }
    public Vector3 Force { get => force; set => force = value; }

    internal void ApplyStatus(Hitable hitable)
    {
        if (startTime == 0) { startTime = Time.time; lastApplication = type == Type.Freeze ? 0f : startTime - frequency; }
        else if (startTime + length < Time.time)
        {
            if (type == Type.Freeze) hitable.StopMotion(true);
            if (type == Type.Knock)
            {
                hitable.StopMotion(true);
                //hitable.GetComponent<Rigidbody>().isKinematic = true;
                //hitable.GetComponent<NavMeshAgent>().enabled = true;
            }
            onStatusEnd.Invoke();
            Destroy(this);
        }

        switch (type)
        {
            case Type.Fire:
                if (Time.time - lastApplication > frequency)
                {
                    hitable.GetHit(hitable.CombatData.PhysicArmor + 3f);
                    lastApplication += frequency;
                    var objParticle = Instantiate(particle, hitable.transform.Find("visual"));
                    Destroy(objParticle, objParticle.GetComponent<ParticleSystem>().main.duration);
                }
                break;
            case Type.Freeze:
                if (lastApplication == 0f)
                {
                    lastApplication = -1f;
                    hitable.StopMotion(false);
                    var objParticle = Instantiate(particle, hitable.transform.Find("visual"));
                    Destroy(objParticle, length);
                }
                break;
            case Type.Bleed:
                if (Time.time - lastApplication > frequency)
                {
                    hitable.GetHit(hitable.CombatData.PhysicArmor + 2f);
                    lastApplication += frequency;
                    var objParticle = Instantiate(particle, hitable.transform.Find("visual"));
                    Destroy(objParticle, objParticle.GetComponent<ParticleSystem>().main.duration);
                }
                break;
            case Type.Knock:
                hitable.Push(force);
                break;
            default:
                break;
        }
    }
    public enum Type
    {
        Fire,
        Freeze,
        Bleed,
        Knock,
        unknown
    }
}
