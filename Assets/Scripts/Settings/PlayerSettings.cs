using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : Settings
{
    private static int startMoney = 10;
    private static float pickableRadius = .2f;
    private static float fastRollAgilityValue = 5f;
    private static float maxAnimationSpeed = 25f;
    private static float maxWeaponDamageBoostByStrength = 1.5f;
    private static Dictionary<string, int> animatorLayers = new Dictionary<string, int>() { { "base", 0 }, { "walk", 1 }, { "block", 2 }, { "roll", 3 } };

    public static int StartMoney { get => startMoney; }
    public static float PickableRadiusCheck { get => pickableRadius; }
    public static float FastRollAgilityValue { get => fastRollAgilityValue; set => fastRollAgilityValue = value; }
    public static float MaxWeaponDamageBoostByStrength { get => maxWeaponDamageBoostByStrength; set => maxWeaponDamageBoostByStrength = value; }
    public static float MaxAnimationSpeed { get => maxAnimationSpeed; set => maxAnimationSpeed = value; }

    public static int GetAnimatorLayers(string layerName)
    {
        if (!animatorLayers.ContainsKey(layerName)) throw new System.Exception("Player Settings - No animator layer for this key (key = " + layerName);
        var value = -1;
        animatorLayers.TryGetValue(layerName, out value);
        return value;
    }
}
