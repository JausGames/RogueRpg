using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinionSpawner", menuName = "Spawner/Minion", order = 2)]
public class MinionSpawner : ScriptableObject
{
    [SerializeField] List<SpawnData> spawnDataList = new List<SpawnData>();
    Transform roomTransform;

    public Transform RoomTransform { get => roomTransform; set => roomTransform = value; }
    internal List<SpawnData> SpawnDataList { get => spawnDataList; set => spawnDataList = value; }

    static void SpawnMinion(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        Instantiate(prefab, position, rotation, parent);
    }
    public List<Minion> SpawnMinion()
    {
        var minions = new List<Minion>();
        var right = GridSettings.gridSize.x * .5f * Vector3.right;
        var up = GridSettings.gridSize.y * .5f * Vector3.forward;
        foreach (SpawnData data in SpawnDataList)
        {
            minions.Add(
                Instantiate(data.prefab, RoomTransform.position + data.position, data.rotation, RoomTransform)
                    .GetComponent<Minion>()
                );
            minions[minions.Count - 1].Motor.Bounding = new Vector3[4]
                {
                    roomTransform.position + right + up,
                    roomTransform.position + right - up,
                    roomTransform.position - right - up,
                    roomTransform.position - right + up
                };
        }
        return minions;
    }
    public List<Minion> SpawnMinion(Transform parent)
    {
        var minions = new List<Minion>();
        foreach (SpawnData data in SpawnDataList)
            minions.Add(
                Instantiate(data.prefab, RoomTransform.position + data.position, data.rotation, parent)
                    .GetComponent<Minion>()
                );
        return minions;
    }

}
