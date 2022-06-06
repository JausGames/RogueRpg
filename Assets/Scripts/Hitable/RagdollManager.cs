using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    [SerializeField] private List<Collider> ragdollColliders;
    [SerializeField] private List<Collider> inGameColliders;
    [SerializeField] private List<Rigidbody> ragdollRigidbodies;
    [SerializeField] private Rigidbody body;
    [SerializeField] private Animator animator;

    public void SetRagdollActive(bool value)
    {
        if (ragdollColliders.Count > 1)
        {
            foreach (Collider col in ragdollColliders) col.enabled = value;
        }
        if (ragdollRigidbodies.Count > 1)
        {
            foreach (Rigidbody bod in ragdollRigidbodies)
            {
                bod.isKinematic = !value;
                if (value) bod.velocity = body.velocity;
            }
        }
        if (inGameColliders.Count > 0)
        {

            foreach (Collider col in inGameColliders)
            {
                col.enabled = !value;
            }
            body.isKinematic = value;
        }

        animator.enabled = !value;

    }
    public void SetRagdollActive(bool value, Vector3 force)
    {
        if (ragdollColliders.Count > 1)
        {
            foreach (Collider col in ragdollColliders) col.enabled = value;
        }
        if (ragdollRigidbodies.Count > 1)
        {
            foreach (Rigidbody bod in ragdollRigidbodies)
            {
                bod.isKinematic = !value;
            }
        }
        if (inGameColliders.Count > 0)
        {

            foreach (Collider col in inGameColliders)
            {
                col.enabled = !value;
            }
            body.isKinematic = value;
        }

        ragdollRigidbodies[0].AddForce(force, ForceMode.Impulse);
        ragdollRigidbodies[2].AddForce(force, ForceMode.Impulse);
        ragdollRigidbodies[1].AddForce(force, ForceMode.Impulse);
        animator.enabled = !value;


    }
}
