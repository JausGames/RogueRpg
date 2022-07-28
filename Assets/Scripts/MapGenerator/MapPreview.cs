using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapPreview : MonoBehaviour {

	public Renderer textureRender;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;


	public enum DrawMode {NoiseMap, Mesh, FalloffMap};
	public DrawMode drawMode;

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureData;
	public InstanciableData instanciablesData;

	public Material terrainMaterial;
	HeightMap heightMap;
	List<GameObject> instances = new List<GameObject>();



	[Range(0,MeshSettings.numSupportedLODs-1)]
	public int editorPreviewLOD;
	public bool autoUpdate;




	public void DrawMapInEditor() {
		textureData.ApplyToMaterial (terrainMaterial);
		textureData.UpdateMeshHeights (terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
		heightMap = HeightMapGenerator.GenerateHeightMap (meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);

		if (drawMode == DrawMode.NoiseMap) {
			DrawTexture (TextureGenerator.TextureFromHeightMap (heightMap));
		} else if (drawMode == DrawMode.Mesh) {
			DrawMesh (MeshGenerator.GenerateTerrainMesh (heightMap.values,meshSettings, editorPreviewLOD));
		} else if (drawMode == DrawMode.FalloffMap) {
			DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine),0,1)));
		}
	}





	public void DrawTexture(Texture2D texture) {
		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height) /10f;

		textureRender.gameObject.SetActive (true);
		meshFilter.gameObject.SetActive (false);
	}

	public void DrawMesh(MeshData meshData) {
		meshFilter.sharedMesh = meshData.CreateMesh ();

		textureRender.gameObject.SetActive (false);
		meshFilter.gameObject.SetActive (true);

		InstanciateObjects(InstanciabletGenerator.InstanciateObjects(instanciablesData, heightMap, meshSettings, heightMapSettings, Vector2.zero));
	}

	void InstanciateObjects(ObjectsData[] data)
    {
		var go = meshFilter.gameObject;
		if (go.GetComponentsInChildren<ObjectSetter>().Length > 0)
			foreach (ObjectSetter obj in go.GetComponentsInChildren<ObjectSetter>()) DestroyImmediate(obj.gameObject);
		//for (int i = 0; i < go.transform.childCount; i++) DestroyImmediate(go.transform.GetChild(i).gameObject);
		for(int i = 0; i < data.Length; i++)
        {
			var pos = data[i].GetWorldPositions();
			for(int j = 0; j < pos.Count; j++)
			{
				var instance = Instantiate(data[i].GetPrefab(), pos[j] * meshSettings.meshScale, Quaternion.identity);
				instance.transform.SetParent(meshFilter.transform);
				if (instance.GetComponent<ObjectSetter>() != null) instance.GetComponent<ObjectSetter>().SetUpObject();
				instances.Add(instance);
			}
        }
    }


	void OnValuesUpdated() {
		if (!Application.isPlaying) {
			DrawMapInEditor ();
		}
	}

	void OnTextureValuesUpdated() {
		textureData.ApplyToMaterial (terrainMaterial);
	}

	void OnValidate() {

		if (meshSettings != null) {
			meshSettings.OnValuesUpdated -= OnValuesUpdated;
			meshSettings.OnValuesUpdated += OnValuesUpdated;
		}
		if (heightMapSettings != null) {
			heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
			heightMapSettings.OnValuesUpdated += OnValuesUpdated;
		}
		if (textureData != null) {
			textureData.OnValuesUpdated -= OnTextureValuesUpdated;
			textureData.OnValuesUpdated += OnTextureValuesUpdated;
		}

	}

}
