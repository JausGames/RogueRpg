using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEvent : MonoBehaviour
{
    [SerializeField] Player player;

    public void StartRolling()
    {
        player.SetRolling(true);
    }
    public void StopRolling()
    {
        player.SetRolling(false);
    }
}
