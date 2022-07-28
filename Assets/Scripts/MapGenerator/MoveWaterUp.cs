using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWaterUp : MonoBehaviour
{
    [SerializeField] float upwardOffset;

    public void MoveUpward()
    {
        transform.position = transform.position + upwardOffset * Vector3.up;
    }
}
