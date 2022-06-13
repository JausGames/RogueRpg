using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : Settings
{
    private static int startMoney = 10;
    private static float pickableRadius = .2f;
    private static Dictionary<string, int> animatorLayers = new Dictionary<string, int>() { { "base", 0 }, { "walk", 1 }, { "block", 2 }, { "roll", 3 } };

    public static int StartMoney { get => startMoney; }
    public static float PickableRadiusCheck { get => pickableRadius; }

    public static int GetAnimatorLayers(string layerName)
    {
        if (!animatorLayers.ContainsKey(layerName)) throw new System.Exception("Player Settings - No animator layer for this key (key = " + layerName);
        var value = -1;
        animatorLayers.TryGetValue(layerName, out value);
        return value;
    }
}
