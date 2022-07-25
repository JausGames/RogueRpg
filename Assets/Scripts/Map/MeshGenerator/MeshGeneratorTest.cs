using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneratorTest : MonoBehaviour
{
	//public Renderer textureRender;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;

	//public TextureData textureData;
	public Material terrainMaterial;
	public BitMap bitmap;
	HeightMap heightMap;


    private void Start()
    {
		DrawMapInEditor();
    }


    public void DrawMapInEditor()
	{
		//textureData.ApplyToMaterial(terrainMaterial);
		//textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
		heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, bitmap, Vector2.zero);
		
		for(int x = 0; x < 3; x++)
			for(int y = 0; y < 3; y++)
            {
				HeightMap heights = KeepNeededBitmapValue(heightMap, x, y);
				DrawMesh(MeshGenerator.MeshGenerator.GenerateTerrainMesh(heights.values, meshSettings, 4), new Vector3(meshSettings.meshWorldSize * (x - 1),0, meshSettings.meshWorldSize * (y - 1)));

            }
	}

    private HeightMap KeepNeededBitmapValue(HeightMap heightMap, int x, int y)
    {
		float[,] newValue = new float[(heightMap.values.GetUpperBound(0) + 1) / 9, (heightMap.values.GetUpperBound(1) + 1) / 9];
		Debug.Log("heightMap.values.GetUpperBound(0) + 1) " + (heightMap.values.GetUpperBound(0) + 1));

        try
        {
			for (int xIt = x; xIt < xIt + heightMap.values.GetUpperBound(0) / 9; xIt++)
				for(int yIt = y; yIt < yIt + heightMap.values.GetUpperBound(0) / 9; yIt++)
				{
					newValue[xIt - x, yIt - y] = heightMap.values[xIt, yIt];
				}

			return new HeightMap(newValue, heightMap.minValue, heightMap.maxValue);
        }
		catch(Exception ex)
        {
			throw new Exception(ex.Message);
        }

    }

    public void DrawMesh(MeshGenerator.MeshData meshData, Vector3 offset)
	{
		var go = new GameObject();
		go.transform.position = offset;
		var meshFilter = go.AddComponent<MeshFilter>();
		var meshRenderer =  go.AddComponent<MeshRenderer>();
		meshRenderer.material = terrainMaterial;
		go.AddComponent<MeshRenderer>();
		meshFilter.sharedMesh = meshData.CreateMesh();

		//textureRender.gameObject.SetActive(false);
		meshFilter.gameObject.SetActive(true);

	}
}

