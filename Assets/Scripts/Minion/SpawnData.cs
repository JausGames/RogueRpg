using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SpawnData", menuName = "Spawner/Data", order = 1)]
class SpawnData : ScriptableObject
{
    public GameObject prefab;
    public Vector3 position;
    public Quaternion rotation;
}
