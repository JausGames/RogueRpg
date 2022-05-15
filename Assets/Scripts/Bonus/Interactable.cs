using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Interactable : MonoBehaviour
{
    abstract public void OnInteract(Hitable player);
}
