using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableContainer : MonoBehaviour
{
    [SerializeField] Pickable item;

    public Pickable Item
    {
        get => item;
        set
        {
            item = value;
            item.Container = gameObject;
        }
    }
}
