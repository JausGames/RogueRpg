using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableContainer : MonoBehaviour
{
    [SerializeField] Pickable item;
    [SerializeField] ParticleSystem particle;

    public Pickable Item
    {
        get => item;
        set
        {
            item = value;
            item.Container = gameObject;
        }
    }

    public ParticleSystem Particle { get => particle; set => particle = value; }
}
