using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Coin", menuName = "Bonus/Pickables/Coin", order = 1)]
public class Coin : Pickable
{
    int value = 0;
    DungeonSettings.CoinType type = DungeonSettings.CoinType.Bronze;

    public int Value { get => value; set => this.value = value; }
    public DungeonSettings.CoinType Type { get => type; set => type = value; }

    public override void AddToPlayer(Player player)
    {
        value = Random.Range(DungeonSettings.CoinValue[type][0], DungeonSettings.CoinValue[type][1] + 1);
        player.Wallet.AddMoney(value);
        base.AddToPlayer(player);
    }
}
