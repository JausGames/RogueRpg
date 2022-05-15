using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Direction
{
    North,
    Est,
    South,
    West,
    None
}

public enum Type
{
    Start,
    End,
    Special,
    Default
}


public class Room : MonoBehaviour
{
    [SerializeField] protected Type roomType = Type.Default;
    [SerializeField] List<Direction> doorsDirections = new List<Direction>();

    [SerializeField] List<GameObject> ui = new List<GameObject>();
    [SerializeField] GameObject hidindSprite;

    [SerializeField] bool open = false;
    [SerializeField] bool isEmpty = false;
    [SerializeField] int[,] position;
    [SerializeField] List<Vector3> doorsPostition = new List<Vector3>();
    [SerializeField] List<Door> doorObject = new List<Door>();

    [SerializeField] protected List<GameObject> roomFloorPrefabs = new List<GameObject>();
    [SerializeField] protected List<GameObject> doorPrefabs = new List<GameObject>();
    [SerializeField] protected List<GameObject> wallPrefabs = new List<GameObject>();

    [SerializeField] protected List<MinionSpawner> MinionSpawnerList = new List<MinionSpawner>();
    [SerializeField] protected MinionSpawner MinionSpawner;

    [SerializeField] protected List<Minion> minions = new List<Minion>();

    internal void AddDoor(Door door)
    {
        doorObject.Add(door);
    }
    public Door GetDoorByDirection(Direction direction)
    {
        foreach (Door door in doorObject)
        {
            if (door.Direction == direction) return door;
        }
        return null;
    }

    private void OnDestroy()
    {
        foreach(GameObject go in ui)
        {
            Destroy(go);
        }
    }
    public List<Direction> DoorsDirections { get => doorsDirections; set => doorsDirections = value; }
    public List<Vector3> DoorsPostition { get => doorsPostition; set => doorsPostition = value; }
    public Type RoomType { get => roomType; set => roomType = value; }
    public int[,] Position { get => position; set => position = value; }
    public bool Open { get => open; 
        set {
            open = value;
            SetEnnemyCanMove(value);
            foreach (GameObject gm in ui) if(gm) gm.SetActive(value);
            if (value) Destroy(hidindSprite);
            }
        }

    public List<GameObject> Ui { get => ui; set => ui = value; }
    public bool IsEmpty { get => isEmpty; set => isEmpty = value; }

    private void Awake()
    {
        DoorsDirections = new List<Direction>();
        DoorsPostition = new List<Vector3>();
    }
    public void SetEnnemyCanMove(bool canMove)
    {
        var minions = GetComponentsInChildren<Minion>();
        var sleepingOpponent = new List<Minion>();

        foreach (Minion min in minions)
            if (min.Owner == null) sleepingOpponent.Add(min);

        foreach (Minion min in sleepingOpponent)
        {
            min.Moving = canMove;
            min.Motor.enabled = true;
        }
    }
    virtual public void GenerateRoom()
    {

        if (!IsContainingDoor(Direction.North))
            Instantiate(wallPrefabs[0], transform.position, doorPrefabs[0].transform.rotation, transform);

        if (!IsContainingDoor(Direction.Est))
            Instantiate(wallPrefabs[1], transform.position, doorPrefabs[0].transform.rotation, transform);

        if (!IsContainingDoor(Direction.South))
            Instantiate(wallPrefabs[2], transform.position, doorPrefabs[0].transform.rotation, transform);

        if (!IsContainingDoor(Direction.West))
            Instantiate(wallPrefabs[3], transform.position, doorPrefabs[0].transform.rotation, transform);

        switch (roomType)
        {
            case Type.Start:
                Instantiate(roomFloorPrefabs[1], transform.position, doorPrefabs[0].transform.rotation, transform);
                isEmpty = true;
                open = true;
                break;
            case Type.End:
                Instantiate(roomFloorPrefabs[2], transform.position, doorPrefabs[0].transform.rotation, transform);
                isEmpty = true;
                break;
            case Type.Special:
                Instantiate(roomFloorPrefabs[3], transform.position, doorPrefabs[0].transform.rotation, transform);
                isEmpty = true;
                break;
            case Type.Default:
                Instantiate(roomFloorPrefabs[0], transform.position, doorPrefabs[0].transform.rotation, transform);
                var rndSpawn = Random.Range(0, MinionSpawnerList.Count);
                MinionSpawner = new MinionSpawner();
                MinionSpawner.SpawnDataList = new List<SpawnData>();
                MinionSpawner.SpawnDataList.AddRange(MinionSpawnerList[rndSpawn].SpawnDataList);
                MinionSpawner.RoomTransform = this.transform;
                minions = MinionSpawner.SpawnMinion();

                foreach (Minion min in minions)
                {
                    min.dieEvent.AddListener(delegate { minions.Remove(min); CheckIfEmpty(); });
                }
                break;
            default:
                break;
        }
    }

    private void CheckIfEmpty()
    {
        if (this.minions.Count == 0) 
            isEmpty = true;
    }

    protected bool IsContainingDoor(Direction dir)
    {
        if (doorsDirections.Count == 0) return false;
        foreach (Direction d in doorsDirections)
        {
            if (d == dir) return true;
        }
        return false;
    }

}
