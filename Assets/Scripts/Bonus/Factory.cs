using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : Interactable
{
    [SerializeField] protected Shop shop = null;

    public override void OnInteract(Hitable player)
    {
    }
}
