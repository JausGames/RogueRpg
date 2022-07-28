using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk {
	
	const float colliderGenerationDistanceThreshold = 5;
	public event System.Action<TerrainChunk, bool> onVisibilityChanged;
	public Vector2 coord;
	 
	GameObject meshObject;
	Vector2 sampleCentre;
	Bounds bounds;

	MeshRenderer meshRenderer;
	MeshFilter meshFilter;
	MeshCollider meshCollider;

	LODInfo[] detailLevels;
	LODMesh[] lodMeshes;
	int colliderLODIndex;

	HeightMap heightMap;
	bool heightMapReceived;

	List<GameObject> objectInstances = new List<GameObject>();
	ObjectsData[] objectsData;
	bool objectsDataReceived;
	int previousLODIndex = -1;
	bool hasSetCollider;
	float maxViewDst;

	HeightMapSettings heightMapSettings;
	MeshSettings meshSettings;
	InstanciableData instanciablesData;
	Transform viewer;

	public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, InstanciableData instanciablesData, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material) {
		Debug.Log("Chunk coord : " + coord);
		this.coord = coord;
		this.instanciablesData = instanciablesData;
		this.detailLevels = detailLevels;
		this.colliderLODIndex = colliderLODIndex;
		this.heightMapSettings = heightMapSettings;
		this.meshSettings = meshSettings;
		this.viewer = viewer;

		sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
		Vector2 position = coord * meshSettings.meshWorldSize ;
		bounds = new Bounds(position,Vector2.one * meshSettings.meshWorldSize );


		meshObject = new GameObject("Terrain Chunk");
		meshRenderer = meshObject.AddComponent<MeshRenderer>();
		meshFilter = meshObject.AddComponent<MeshFilter>();
		meshCollider = meshObject.AddComponent<MeshCollider>();
		meshRenderer.material = material;

		meshObject.transform.position = new Vector3(position.x,0,position.y);
		meshObject.transform.parent = parent;
		SetVisible(false);

		lodMeshes = new LODMesh[detailLevels.Length];
		for (int i = 0; i < detailLevels.Length; i++) {
			lodMeshes[i] = new LODMesh(detailLevels[i].lod);
			lodMeshes[i].updateCallback += UpdateTerrainChunk;
			if (i == colliderLODIndex) {
				lodMeshes[i].updateCallback += UpdateCollisionMesh;
			}
		}

		maxViewDst = detailLevels [detailLevels.Length - 1].visibleDstThreshold;

	}

	public void Load() {
		//ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap (meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapReceived);
		ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMapFromBitmap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapReceived);
		
	}

	public MeshFilter GetMeshFilter()
    {
		return meshFilter;
    }

	void OnHeightMapReceived(object heightMapObject) {
		Debug.Log("OnHeightMapReceived, coord = " + coord.x + ", " + coord.y);
		this.heightMap = KeepNeededBitmapValue((HeightMap)heightMapObject, (int)coord.x, (int)coord.y);
		//this.heightMap = (HeightMap)heightMapObject;
		heightMapReceived = true;
		//@BJN instanciable data
		//ThreadedDataRequester.RequestData(() => InstanciabletGenerator.InstanciateObjects(instanciablesData, heightMap, meshSettings, heightMapSettings, sampleCentre), OnObjectDataReceived);
		UpdateTerrainChunk ();
	}
	private HeightMap KeepNeededBitmapValue(HeightMap heightMap, int x, int y)
	{
		var zoom = 10;

		float[,] newValue = new float[(heightMap.values.GetUpperBound(0) + 1) / zoom, (heightMap.values.GetUpperBound(1) + 1) / zoom];
		Debug.Log("heightMap.values.GetUpperBound(0) + 1) " + (heightMap.values.GetUpperBound(0) + 1));

		
		var maxX = x + (heightMap.values.GetUpperBound(0) + 1) / 10;
		var maxY = y + (heightMap.values.GetUpperBound(1) + 1) / 10;

		for (int xIt = 0; xIt < maxX; xIt++)
			for (int yIt = 0; yIt < maxY; yIt++)
			{
				// get rid of negativ values
				newValue[xIt, yIt] = heightMap.values[xIt * zoom + x, yIt + y];
			}

		return new HeightMap(newValue, heightMap.minValue, heightMap.maxValue);
		

	}
	void OnObjectDataReceived(object intanceMapObject)
	{
		this.objectsData = (ObjectsData[]) intanceMapObject;
		objectsDataReceived = true;

		UpdateTerrainChunk();
	}

	Vector2 viewerPosition {
		get {
			return new Vector2 (viewer.position.x, viewer.position.z);
		}
	}


	public void UpdateTerrainChunk() {
		if (heightMapReceived) {
			float viewerDstFromNearestEdge = Mathf.Sqrt (bounds.SqrDistance (viewerPosition));

			bool wasVisible = IsVisible ();
			bool visible = viewerDstFromNearestEdge <= maxViewDst;

			if (visible) {
				int lodIndex = 0;

				for (int i = 0; i < detailLevels.Length - 1; i++) {
					if (viewerDstFromNearestEdge > detailLevels [i].visibleDstThreshold) {
						lodIndex = i + 1;
					} else {
						break;
					}
				}

				if (lodIndex != previousLODIndex) {
					LODMesh lodMesh = lodMeshes [lodIndex];
					if (lodMesh.hasMesh) {
						previousLODIndex = lodIndex;
						meshFilter.mesh = lodMesh.mesh;
					} else if (!lodMesh.hasRequestedMesh) {
						lodMesh.RequestMesh (heightMap, meshSettings);
					}
				}


			}

			if (wasVisible != visible) {
				
				SetVisible (visible);
				if (onVisibilityChanged != null) {
					onVisibilityChanged (this, visible);
				}
			}
		}
		if (objectsDataReceived)
		{
			float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

			bool wasVisible = IsVisible();
			bool visible = viewerDstFromNearestEdge <= maxViewDst;

			if (visible)
			{
				int lodIndex = 0;

				for (int i = 0; i < detailLevels.Length - 1; i++)
				{
					if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
					{
						lodIndex = i + 1;
					}
					else
					{
						break;
					}
				}

				if (objectInstances.Count == 0)
				TerrainGenerator.InstanciateObjects(objectsData, this, objectInstances, meshSettings);

			}

			if (wasVisible != visible)
			{

				SetVisible(visible);
				if (onVisibilityChanged != null)
				{
					onVisibilityChanged(this, visible);
				}
			}
		}
	}

	public void UpdateCollisionMesh() {
		if (meshSettings.isWater) return;
		if (!hasSetCollider) {
			float sqrDstFromViewerToEdge = bounds.SqrDistance (viewerPosition);

			if (sqrDstFromViewerToEdge < detailLevels [colliderLODIndex].sqrVisibleDstThreshold) {
				if (!lodMeshes [colliderLODIndex].hasRequestedMesh) {
					lodMeshes [colliderLODIndex].RequestMesh (heightMap, meshSettings);
				}
			}

			if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold) {
				if (lodMeshes [colliderLODIndex].hasMesh) {
					meshCollider.sharedMesh = lodMeshes [colliderLODIndex].mesh;
					hasSetCollider = true;
				}
			}
		}
	}

	public void SetVisible(bool visible) {
		meshObject.SetActive (visible);
	}

	public bool IsVisible() {
		return meshObject.activeSelf;
	}

}

class LODMesh {

	public Mesh mesh;
	public bool hasRequestedMesh;
	public bool hasMesh;
	int lod;
	public event System.Action updateCallback;

	public LODMesh(int lod) {
		this.lod = lod;
	}

	void OnMeshDataReceived(object meshDataObject) {
		mesh = ((MeshData)meshDataObject).CreateMesh ();
		hasMesh = true;

		updateCallback ();
	}

	public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings) {
		hasRequestedMesh = true;
		ThreadedDataRequester.RequestData (() => MeshGenerator.GenerateTerrainMesh (heightMap.values, meshSettings, lod), OnMeshDataReceived);
	}

}
