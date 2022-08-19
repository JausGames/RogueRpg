using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolloMainCameraRotation : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
        // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
    }
}
