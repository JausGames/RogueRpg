using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gold Coin", menuName = "Bonus/Pickables/Gold Coin", order = 3)]
public class GoldCoin : Coin
{
    private void Awake()
    {
        Type = DungeonSettings.CoinType.Gold;
    }

}
