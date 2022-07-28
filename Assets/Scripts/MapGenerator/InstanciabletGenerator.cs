using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InstanciabletGenerator
{
    public static ObjectsData[] InstanciateObjects(InstanciableData objets, HeightMap heightMap, MeshSettings meshSetting, HeightMapSettings heightSettings, Vector2 sampleCenter)
    {
        if (objets == null) return null;
        ObjectsData[] objsData = new ObjectsData[objets.layers.Length];

        var usedPos = new List<Vector2>();

        for (int i = 0; i < objets.layers.Length; i++)
        {
            var objMap  = Noise.GenerateNoiseMap(meshSetting.numVertsPerLine, meshSetting.numVertsPerLine, objets.layers[i].noiseSettings, sampleCenter);

            var objData = new ObjectsData();
            objData.SetPrefab(objets.layers[i].visual);
            Vector2 topLeft = new Vector2(-1, 1) * meshSetting.meshWorldSize / 2f;

            for (int x = 1; x < meshSetting.numVertsPerLine - 1; x++)
            {
                for (int y = 1; y < meshSetting.numVertsPerLine - 1; y++)
                {
                    if (!CheckValueInArray(usedPos, new Vector2(x, y)))
                    {
                        //var factor = 1 / Mathf.Pow(objMap[x, y] + 0.0001f, 0.8f);
                        var factor = objets.layers[i].densityCurve.Evaluate(objMap[x, y]);
                        var objMapValue = Mathf.CeilToInt(factor - 1f);

                        Vector2 percent = new Vector2(x - 1, y - 1) / (meshSetting.numVertsPerLine - 3);
                        Vector2 vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y) * meshSetting.meshWorldSize;

                        if (heightMap.values[x, y] / heightSettings.heightMultiplier < objets.layers[i].endHeight
                            && heightMap.values[x, y] / heightSettings.heightMultiplier > objets.layers[i].startHeight
                            && x % objMapValue == 0
                            && y % objMapValue == 0)
                        {
                            objData.AddWorldPos(new Vector3(vertexPosition2D.x, heightMap.values[x, y], vertexPosition2D.y) / meshSetting.meshScale);
                            usedPos.Add(new Vector2(x, y));
                        }
                    }
                
                }
            }
            objsData[i] = objData;
        }
        return objsData;
    }
    static private bool CheckValueInArray(List<Vector2> list, Vector2 obj)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i] == obj)
                return true;
        }
        return false;
    }


}
public class ObjectsData
{
    GameObject prefab;
    List<Vector3> worldPositions = new List<Vector3>();

    public GameObject GetPrefab()
    {
        return prefab;
    }
    public void SetPrefab(GameObject prefab)
    {
        this.prefab = prefab;
    }
    public void AddWorldPos(Vector3 pos)
    {
        worldPositions.Add(pos);
    }
    public List<Vector3> GetWorldPositions()
    {
        return worldPositions;
    }
}
