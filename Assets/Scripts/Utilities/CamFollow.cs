using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField] Transform follow;
    [SerializeField] Quaternion offsetLookAt;
    [SerializeField] Vector3 offsetPosition;
    //[SerializeField] Quaternion offsetRotation;
    [SerializeField] float positionClampSpeed;
    [SerializeField] float rotationClampSpeed;

    // Update is called once per frame
    void FixedUpdate()
    {
        //TOP VIEW
        //if (follow) transform.position = Vector3.Lerp(transform.position, follow.position + offsetPosition, positionClampSpeed);
        //THIRD VIEW
        if (follow)
        {
            transform.position = Vector3.Lerp(transform.position, 
                follow.position + offsetPosition.y * Vector3.up + offsetPosition.z * follow.forward, 
                positionClampSpeed);
        }
    }
}
