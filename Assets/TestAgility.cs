using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAgility : MonoBehaviour
{
    public Animator animator;

    public void UpdateAgility(float value)
    {
        animator.SetFloat("Agility", value);
    }
}
