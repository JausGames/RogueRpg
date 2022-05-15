using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatLayer : MonoBehaviour
{
    static public LayerMask GetPlayerLayer()
    {
        return LayerMask.NameToLayer("Player");
    }

    static public LayerMask GetEnnemyLayer()
    {
        return LayerMask.NameToLayer("Enemy");
    }
    static public LayerMask GetMinionLayer()
    {
        return LayerMask.NameToLayer("Minion");
    }
}
