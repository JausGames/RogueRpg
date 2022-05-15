using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class DungeonSettings
{
    public enum CoinType
    {
        Bronze,
        Silver,
        Gold
    }

    static public int[] bronzeCoinRange = { 1, 4 };
    static public int[] silverCoinRange = { 5, 10 };
    static public int[] goldCoinRange = { 10, 20 };

    static Dictionary<CoinType, int[]> coinValue = new Dictionary<CoinType, int[]>()
    { 
        {CoinType.Bronze, bronzeCoinRange},
        {CoinType.Silver, silverCoinRange},
        {CoinType.Gold, goldCoinRange}
    };

    public static Dictionary<CoinType, int[]> CoinValue { get => coinValue; }
}
