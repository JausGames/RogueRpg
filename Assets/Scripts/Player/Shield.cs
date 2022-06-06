using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] bool isActive = false;

    public bool IsActive { get => isActive; set => isActive = value; }
}
