using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionFactory : Factory
{
    [SerializeField] List<SpawnData> spawnList = new List<SpawnData>();
    [SerializeField] List<Minion> minions = new List<Minion>();
    [SerializeField] bool open = false;
    private MinionSpawner minionSpawner;

    public bool Open { get => open; set => open = value; }

    override public void OnInteract(Hitable player)
    {
        if (!open) OpenChest(player);
    }
    private void OpenChest(Hitable player)
    {
        open = true;
        var rndI = Random.Range(1, 5);
        var rnd = Random.Range(1, spawnList.Count);

        minionSpawner = new MinionSpawner();
        minionSpawner.SpawnDataList = new List<SpawnData>();
        for (int i = 0; i < rndI; i++)
        {
            minionSpawner.SpawnDataList.Add(spawnList[rnd]);
            minionSpawner.RoomTransform = this.transform;
        }
        minions = minionSpawner.SpawnMinion(player.transform.parent.Find("Army"));
        var army = FindObjectOfType<Army>();
        foreach (Minion min in minions)
        {
            min.Owner = (Player) player;
            min.Owner.AddMinionToArmy(min);
            min.gameObject.layer = 1 >> min.Owner.MinionMask.value;
            min.SetLayers(min.Owner.GetFriendLayer(), min.Owner.GetEnemyLayer());
            min.Moving = true;
        }
        
        Destroy(this.gameObject);
    }

}
