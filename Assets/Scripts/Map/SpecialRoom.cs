
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialRoom : Room
{
    [SerializeField] SpecialRoom.Type specialType;

    public Type SpecialType { get => specialType; set => specialType = value; }

    public override void GenerateRoom()
    {
        //var rnd = Random.Range(0, System.Enum.GetNames(typeof(SpecialRoom.Type)).Length);

        //specialType = (Type) rnd;

    if (!IsContainingDoor(Direction.North))
        Instantiate(wallPrefabs[0], transform.position, doorPrefabs[0].transform.rotation, transform);

    if (!IsContainingDoor(Direction.Est))
        Instantiate(wallPrefabs[1], transform.position, doorPrefabs[0].transform.rotation, transform);

    if (!IsContainingDoor(Direction.South))
        Instantiate(wallPrefabs[2], transform.position, doorPrefabs[0].transform.rotation, transform);

    if (!IsContainingDoor(Direction.West))
        Instantiate(wallPrefabs[3], transform.position, doorPrefabs[0].transform.rotation, transform);

        switch (specialType)
        {
            case Type.Troup:
                Instantiate(roomFloorPrefabs[0], transform.position, doorPrefabs[0].transform.rotation, transform);
                break;
            case Type.Bonus:
                Instantiate(roomFloorPrefabs[1], transform.position, doorPrefabs[0].transform.rotation, transform);
                break;
            case Type.Shop:
                Instantiate(roomFloorPrefabs[2], transform.position, doorPrefabs[0].transform.rotation, transform);
                break;
            default:
                break;
        }

    }
    
    public enum Type
    {
        Troup,
        Bonus,
        Shop,
        Null
    }
}
