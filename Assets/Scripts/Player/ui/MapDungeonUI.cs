using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDungeonUI : MapUi
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] List<Image> roomUi;
    float imageSize = 0;
    [SerializeField] List<GameObject> doorPrefab;
    [SerializeField] List<GameObject> wallPrefabs;

    private void Awake()
    {
        imageSize = roomPrefab.GetComponent<RectTransform>().rect.width + 7f;
        width = GetComponent<RectTransform>().rect.width / 2f;

        var canvasTrans = canvas.GetComponent<RectTransform>();
        canvasTrans.sizeDelta = 2f* width * Vector3.right + 2f *width * Vector3.up;
    }
    public GameObject AddRoom(int[,] roomPos, Color color)
    {
        var room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, canvas.transform);
        room.GetComponent<RectTransform>().localPosition = Vector3.right * roomPos[0, 0] * imageSize + Vector3.up * roomPos[0, 1] * imageSize;
        room.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        roomUi.Add(room.GetComponent<Image>());
        roomUi[roomUi.Count - 1].color = new Color(color.r, color.g, color.b, miniMapFade);
        return room;

    }

    internal void DeleteMap()
    {
        roomUi.Clear();
        /*foreach(Image img in roomUi)
        {
            Destroy(img.gameObject);
        }*/
    }

    public void AddRoom(int[,] roomPos)
    {
        var room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, canvas.transform);
        room.GetComponent<RectTransform>().localPosition = Vector3.right * roomPos[0, 0] * imageSize + Vector3.up * roomPos[0, 1] * imageSize;
        room.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        roomUi.Add(room.GetComponent<Image>());
    }
    public GameObject AddDoor(Vector3 roomPos, Vector3 offset, Direction direction)
    {
        var prefab = direction == Direction.North || direction == Direction.South ? doorPrefab[1] : doorPrefab[0];
        var door = Instantiate(prefab, Vector3.zero, Quaternion.identity, canvas.transform);
        door.GetComponent<RectTransform>().localPosition = (roomPos + offset * 0.5f).x * imageSize * Vector3.right + (roomPos + offset * 0.5f).y * imageSize * Vector3.up;
        door.GetComponent<RectTransform>().localRotation = Quaternion.identity;
        return door.transform.Find("door").gameObject;
    }
    public void CompleteRoom(List<Room> roomList)
    {
        foreach(Room rm in roomList)
        {
            var uiDoorPos = (rm.transform.position.x / GridSettings.gridSize.x) * Vector3.right +
                                (rm.transform.position.z / GridSettings.gridSize.y) * Vector3.up;

            GameObject prefab = null;
            //List<GameObject> doorObjs = new List<GameObject>();
            Vector3 offset = Vector3.zero;
            Debug.Log("AddDoor, MapUI : uiDoorPos + " + uiDoorPos);
            Debug.Log("AddDoor, MapUI : offset + " + offset);
            if (!IsContainingDoor(rm, Direction.North))
            {
                prefab = Instantiate(wallPrefabs[0], Vector3.zero, Quaternion.identity, canvas.transform);
                offset = Vector3.up;
                prefab.GetComponent<RectTransform>().localPosition = (uiDoorPos + offset * 0.5f).x * imageSize * Vector3.right + 
                                                                    (uiDoorPos + offset * 0.5f).y * imageSize * Vector3.up;
                prefab.GetComponent<RectTransform>().localRotation = Quaternion.identity;
                rm.Ui.Add(prefab);
            }
            if (!IsContainingDoor(rm, Direction.Est))
            {
                prefab = Instantiate(wallPrefabs[1], Vector3.zero, Quaternion.identity, canvas.transform);
                offset = Vector3.right;
                prefab.GetComponent<RectTransform>().localPosition = (uiDoorPos + offset * 0.5f).x * imageSize * Vector3.right + 
                                                                    (uiDoorPos + offset * 0.5f).y * imageSize * Vector3.up;
                prefab.GetComponent<RectTransform>().localRotation = Quaternion.identity;
                rm.Ui.Add(prefab);
            }

            if (!IsContainingDoor(rm, Direction.South))
            {
                prefab = Instantiate(wallPrefabs[0], Vector3.zero, Quaternion.identity, canvas.transform);
                offset = Vector3.down;
                prefab.GetComponent<RectTransform>().localPosition = (uiDoorPos + offset * 0.5f).x * imageSize * Vector3.right + 
                                                                    (uiDoorPos + offset * 0.5f).y * imageSize * Vector3.up;
                prefab.GetComponent<RectTransform>().localRotation = Quaternion.identity;
                rm.Ui.Add(prefab);
            }
            if (!IsContainingDoor(rm, Direction.West))
            {
                prefab = Instantiate(wallPrefabs[1], Vector3.zero, Quaternion.identity, canvas.transform);
                offset = Vector3.left;
                prefab.GetComponent<RectTransform>().localPosition = (uiDoorPos + offset * 0.5f).x * imageSize * Vector3.right + 
                                                                    (uiDoorPos + offset * 0.5f).y * imageSize * Vector3.up;
                prefab.GetComponent<RectTransform>().localRotation = Quaternion.identity;
                rm.Ui.Add(prefab);
            }


        }

        List<Image> images = new List<Image>();
        images.Add(transform.Find("playerMark").GetComponent<Image>());
        images.AddRange(transform.Find("Canvas").GetComponentsInChildren<Image>());

        foreach (Image img in images)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, miniMapFade);
        }
    }

    bool IsContainingDoor(Room room, Direction dir)
    {
        if (room.DoorsDirections.Count == 0) return false;
        foreach (Direction d in room.DoorsDirections)
        {
            if (d == dir) return true;
        }
        return false;
    }
    public override void SetPlayerPosition(float x, float y, float cameraSize = 1)
    {
        /*Debug.Log("MapUI, SetPlayerPosition : Update pos x : " + x);
        Debug.Log("MapUI, SetPlayerPosition : Update pos y : " + y);*/
        if (!zoom) canvas.GetComponent<RectTransform>().localPosition = new Vector3(-x * imageSize - width, -y * imageSize - width);
        else canvas.GetComponent<RectTransform>().localPosition = new Vector3(-x * imageSize - Screen.width * 0.5f, -y * imageSize - Screen.height * 0.5f);

    }

    
}
