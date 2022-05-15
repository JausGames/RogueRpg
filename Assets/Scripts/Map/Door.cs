using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : Interactable
{
    [SerializeField] List<Room> connectedRooms = new List<Room>();
    [SerializeField] GameObject uiDoor;
    [SerializeField] Direction direction;

    public GameObject UiDoor { get => uiDoor; set => uiDoor = value; }
    public Direction Direction { get => direction; set => direction = value; }


    override public void OnInteract(Hitable player)
    {
        foreach(Room room in connectedRooms)
        {
            if(room.Open && room.IsEmpty)
                Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        /*var room = GetComponentInParent<Room>();
        room.CalculateNavMesh();*/

        foreach(Room room in connectedRooms)
        {
            if(room != null) room.Open = true;
        }
        if(uiDoor) Destroy(uiDoor);
        var stageGen = FindObjectOfType<StageGenerator>();
        Debug.Log("Door broken");
        if(stageGen) stageGen.UpdateNavMesh();

    }
    public void SetConnectedRoom(Room room1, Room room2)
    {
        connectedRooms.Add(room1);
        connectedRooms.Add(room2);
        room1.AddDoor(this);
        room2.AddDoor(this);
    }

    internal void ChangeConnectedRoom(SpecialRoom specialRoom)
    {
        for(int i = 0; i < connectedRooms.Count; i++)
        {
            if (connectedRooms[i] == null) connectedRooms[i] = specialRoom;
        }
    }
}

