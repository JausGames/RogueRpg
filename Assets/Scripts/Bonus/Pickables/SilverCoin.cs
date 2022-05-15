using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Silver Coin", menuName = "Bonus/Pickables/Silver Coin", order = 2)]
public class SilverCoin : Coin
{
    private void Awake()
    {
        Type = DungeonSettings.CoinType.Silver;
    }

}
