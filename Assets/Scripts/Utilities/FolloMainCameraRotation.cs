using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolloMainCameraRotation : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector2 look;
    [SerializeField] float rotationSpeed = 0.01f;
    [SerializeField] Vector3 offset;
        // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, 0.8f);
        if(look.x != 0) transform.rotation *= Quaternion.AngleAxis(look.x + rotationSpeed * Time.deltaTime, Vector3.up);
    }

    internal void SetLook(Vector2 look)
    {
        this.look = look;
    }
}
