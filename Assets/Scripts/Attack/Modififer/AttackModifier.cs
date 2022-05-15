using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "Bonus/Attack Modifier", order = 2)]
public class AttackModifier : ScriptableObject
{
    [SerializeField] Status status;

    internal void ApplyModifier(AttackData data)
    {
        if (status != null) data.AddStatus(new Status(status));
    }
}


