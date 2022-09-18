using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] List<Room> roomList = new List<Room>();
    [SerializeField] List<int[,]> spotList = new List<int[,]>();
    [SerializeField] Room[,] roomMatrix = new Room[50, 50];
    [SerializeField] GameObject startGO, endGO;
    [SerializeField] MapDungeonUI mapUi;

    private Room startRoom;
    private int[,] startPos = new int[1, 2];

    private Room endRoom;
    private int[,] endPos = new int[1, 2];

    public List<Room> RoomList { get => roomList; }
    public Room[,] RoomMatrix { get => roomMatrix; }

    public List<int[,]> SpotList { get => spotList; set => spotList = value; }

    public List<Room> SelectableRoomList
    {
        get
        {
            var selectable = new List<Room>();
            selectable.AddRange(roomList);
            selectable.Remove(startRoom);
            selectable.Remove(endRoom);
            return selectable;
        }
    }

    internal void DeleteStage()
    {
        spotList.Clear();
        roomMatrix = new Room[50, 50];
        roomList.Clear();
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        mapUi.DeleteMap();
    }

    private void OnDestroy()
    {
    }
    public float MaxX
    {
        get
        {
            var value = -Mathf.Infinity;
            for (int i = 0; i < spotList.Count; i++)
            {
                if (spotList[i][0, 0] > value) value = spotList[i][0, 0];
            }
            Debug.Log("Stage, MaxX = " + value * GridSettings.gridSize.x);
            return value * GridSettings.gridSize.y;
        }
    }
    public float MinX
    {
        get
        {
            var value = Mathf.Infinity;
            for (int i = 0; i < spotList.Count; i++)
            {
                if (spotList[i][0, 0] < value) value = spotList[i][0, 0];
            }
            Debug.Log("Stage, MinX = " + value * GridSettings.gridSize.x);
            return value * GridSettings.gridSize.y;
        }
    }
    public float MaxY
    {
        get
        {
            var value = -Mathf.Infinity;
            for (int i = 0; i < spotList.Count; i++)
            {
                if (spotList[i][0, 1] > value) value = spotList[i][0, 1];
            }
            Debug.Log("Stage, MaxY = " + value * GridSettings.gridSize.y); 
            return value * GridSettings.gridSize.y;
        }
    }
    public float MinY
    {
        get
        {
            var value = Mathf.Infinity;
            for (int i = 0; i < spotList.Count; i++)
            {
                if (spotList[i][0, 1] < value) value = spotList[i][0, 1];
            }
            Debug.Log("Stage, MinY = " + value * GridSettings.gridSize.y);
            return value * GridSettings.gridSize.y;
        }
    }

    public void AddRoom(Room room, int[,] place)
    {
        StageTrace.Trace("Stage, AddRoom : try add room[" + roomList.Count + "] = [" + (place[0, 0]) + "," + (place[0, 1]) + "]");
        if (roomList.Contains(room)) return;
        StageTrace.Trace("Stage, AddRoom : try add room[" + roomList.Count + "] = [" + (place[0, 0]) + "," + (place[0, 1]) + "]");
        roomList.Add(room);
        spotList.Add(new int[,] { { place[0, 0], place[0, 1] } });
        roomMatrix[place[0, 0] + 25, place[0, 1] + 25] = room;
    }

    public int[,] GetPositionByRoom(Room room)
    {
        for (int i = 0; i < roomList.Count; i++)
            if (roomList[i] == room) 
                return spotList[i];

        return new int[0, 0];
    }
    public Room GetRoomByPosition(int[,] pos)
    {
        for (int i = 0; i < spotList.Count; i++)
            if (pos[0,0] == spotList[i][0,0] && pos[0, 1] == spotList[i][0, 1]) 
                return roomList[i];

        return null;
    }
    public bool CheckIsPlaceFree(int[,] place)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (spotList[i][0, 0] == place[0, 0] && spotList[i][0, 1] == place[0, 1])
            {
                return false;
            }
        }
        return true;
    }
    public void SetStageStart(Room room, int[,] pos)
    {
        startRoom = room;
        startPos[0, 0] = pos[0, 0];
        startPos[0, 1] = pos[0, 1];
        room.RoomType = Type.Start;
    }
    public void SetStageEnd(Room room, int[,] pos)
    {
        endRoom = room;
        endPos[0,0] = pos[0, 0];
        endPos[0,1] = pos[0, 1];
        room.RoomType = Type.End;
    }

    public bool CheckNextDoorRoom(int[,] vs, List<Direction> doorsDirections)
    {
        var roomIsOk = true;

        var northMatrix = new int[1, 2] { { vs[0, 0], vs[0, 1] + 1 } };
        var southMatrix = new int[1, 2] { { vs[0, 0], vs[0, 1] - 1 } };
        var eastMatrix = new int[1, 2] { { vs[0, 0] + 1, vs[0, 1] } };
        var westMatrix = new int[1, 2] { { vs[0, 0] - 1, vs[0, 1] } };

        if (GetRoomByPosition(northMatrix))
            if (IsDirectionInList(Direction.North, doorsDirections)
            != IsDirectionInList(Direction.South, GetRoomByPosition(northMatrix).DoorsDirections))
                roomIsOk = false;
        if (GetRoomByPosition(southMatrix))
            if (IsDirectionInList(Direction.South, doorsDirections)
            != IsDirectionInList(Direction.North, GetRoomByPosition(southMatrix).DoorsDirections))
                roomIsOk = false;
        if (GetRoomByPosition(eastMatrix))
            if (IsDirectionInList(Direction.Est, doorsDirections)
            != IsDirectionInList(Direction.West, GetRoomByPosition(eastMatrix).DoorsDirections))
                roomIsOk = false;
        if (GetRoomByPosition(westMatrix))
            if (IsDirectionInList(Direction.West, doorsDirections)
            != IsDirectionInList(Direction.Est, GetRoomByPosition(westMatrix).DoorsDirections))
                roomIsOk = false;

        return roomIsOk;

    }
    bool IsDirectionInList(Direction dirChecked, List<Direction> Roomdirections)
    {
        foreach (Direction dir in Roomdirections)
        {
            if (dir == dirChecked) return true;
        }
        return false;
    }

    public void StartSettingUpStage(StageGenerator.DoorNeedData doorNeed)
    {
        for (int i = 0; i < RoomList.Count; i++)
        {
            roomList[i].GenerateRoom();
        }

        SetMapUi(doorNeed);

        startGO = new GameObject("start");
        startGO.transform.position = startRoom.transform.position;

        endGO = new GameObject("end");
        endGO.transform.position = endRoom.transform.position;

    }

    private void SetMapUi(StageGenerator.DoorNeedData doorNeed)
    {
        for (int i = 0; i < RoomList.Count; i++)
        {
            if (roomList[i].RoomType == Type.Special)
            {
                var specialRoom = (SpecialRoom)roomList[i];
                roomList[i].Ui.Add(mapUi.AddRoom(spotList[i], MapSettings.RoomTypeToColor(specialRoom.RoomType, specialRoom.SpecialType)));
            }
            else
                roomList[i].Ui.Add(mapUi.AddRoom(spotList[i], MapSettings.RoomTypeToColor(roomList[i].RoomType)));
        }
        GenerateDoorUi(doorNeed);

        mapUi.CompleteRoom(RoomList);
    }

    private void GenerateDoorUi(StageGenerator.DoorNeedData doorNeed)
    {
        for (int i = 0; i < doorNeed.roomList.Count; i++)
        {
            var doorRoom = GetRoomByPosition(doorNeed.roomList[i].placedRoomMatPos);
            var doorConnectedRoom = GetRoomByPosition(doorNeed.connectedRoomList[i].placedRoomMatPos);

            var uiDoorPos = (doorRoom.transform.position.x / GridSettings.gridSize.x) * Vector3.right +
                                (doorRoom.transform.position.z / GridSettings.gridSize.y) * Vector3.up;

            switch (doorNeed.dirList[i])
            {
                case Direction.North:
                    var doorN = mapUi.AddDoor(uiDoorPos, Vector3.up, doorNeed.dirList[i]);
                    doorConnectedRoom.GetDoorByDirection(doorNeed.dirList[i]).UiDoor = doorN;
                    GetRoomByPosition(doorNeed.roomList[i].placedRoomMatPos).Ui.Add(doorN.transform.parent.gameObject);
                    break;
                case Direction.Est:
                    var doorE = mapUi.AddDoor(uiDoorPos, Vector3.right, doorNeed.dirList[i]);
                    doorConnectedRoom.GetDoorByDirection(doorNeed.dirList[i]).UiDoor = doorE;
                    GetRoomByPosition(doorNeed.roomList[i].placedRoomMatPos).Ui.Add(doorE.transform.parent.gameObject);
                    break;
                case Direction.South:
                    var doorS = mapUi.AddDoor(uiDoorPos, -Vector3.up, doorNeed.dirList[i]);
                    doorConnectedRoom.GetDoorByDirection(doorNeed.dirList[i]).UiDoor = doorS;
                    GetRoomByPosition(doorNeed.roomList[i].placedRoomMatPos).Ui.Add(doorS.transform.parent.gameObject);
                    break;
                case Direction.West:
                    var doorW = mapUi.AddDoor(uiDoorPos, -Vector3.right, doorNeed.dirList[i]);
                    doorConnectedRoom.GetDoorByDirection(doorNeed.dirList[i]).UiDoor = doorW;
                    GetRoomByPosition(doorNeed.roomList[i].placedRoomMatPos).Ui.Add(doorW.transform.parent.gameObject);
                    break;
                case Direction.None:
                    break;
                default:
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        /*if (startGO == null || endGO == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(startGO.transform.position, 3f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endGO.transform.position, 3f);*/
    }
}
