using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float shieldPower = 70f; // %
    [SerializeField] float counterTime = .1f;
    [Header("Status")]
    [SerializeField] bool isActive = false;
    [SerializeField] bool isCounter = false;
    [SerializeField] float counterTimer = 0f;

    private void Update()
    {
        if (isCounter && counterTimer < Time.time) isCounter = false;
    }
    public bool IsActive
    {
        get => isActive; 
        set
        {
            isActive = value;
            isCounter = value;
            if (value) counterTimer = Time.time + counterTime;
        }
    }

    public bool IsCounter { get => isCounter; set => isCounter = value; }
    public float ShieldPower { get => shieldPower; set => shieldPower = value; }
}
