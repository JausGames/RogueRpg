using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WCF;
using static GridGenerator.MeshData;
using static UnityEngine.Mathf;

namespace GridGenerator
{
    public class GridGenerator : MonoBehaviour
    {
        // DEBUG 
        int startLayer = 0;
        public Mesh mesh;
        public WCF.WCF wcf;
        //
        public int layerNb = 2;
        public MeshData meshData = null;
        public float radius = 4;
        public DebugDrawer DebugDrawer = null;
        Vector3 origin = new Vector3(0, 0, 0);
        public Vector3[] corners;

        //texture
        float tiling = 1 / 10f;
        public Material[] mapMaterials;
        public Material[] miniMapMaterials;
        public Camera minimapCamera = null;
        public Image minimapImage = null;

        [SerializeField]
        NoiseSettings noiseSettings;
        [SerializeField]
        AnimationCurve heightCurve;



        private void Awake()
        {
            Debug.Log("Start Time = " + Time.time);

            meshData = new MeshData(new List<Vector3>(), radius, layerNb, tiling);
            GenerateLayers(layerNb, radius);
           GenerateBaseMesh();

            meshData.DeleteTrisRandomly();

            meshData.SubdivideGrid();
            //meshData.SubdivideGrid();
            //meshData.SubdivideGrid();
            InitializeBorders();

            var ptsOnQuad = meshData.SmoothGrid();

            StartCoroutine(StartWfc());

        }
        private IEnumerator StartWfc()
        {
            var coroutWithdata = new CoroutineWithData(this, wcf.StartWave(meshData, miniMapMaterials));
            while (coroutWithdata.result == null || (coroutWithdata.result.GetType() == typeof(bool) && (bool)coroutWithdata.result == false) || coroutWithdata.result.GetType() == typeof(WaitForSeconds))
            {
                if (coroutWithdata.result != null && (coroutWithdata.result.GetType() == typeof(bool) && (bool)coroutWithdata.result == false))
                {
                    coroutWithdata = new CoroutineWithData(this, wcf.StartWave(meshData, miniMapMaterials));
                }
                yield return new WaitForEndOfFrame();
            }


            StartCoroutine(StartNoiseMapping((List<TileHolder>)coroutWithdata.result));
        }

        private IEnumerator StartNoiseMapping(List<TileHolder> tiles)
        {
            var meshModifier = new MeshModifier();
            var noise = Noise.GenerateNoiseMap(600, 600, noiseSettings, Vector3.zero);


            float width;
            var farestX = 0f;
            var farestZ = 0f;
            var closestX = 0f;
            var closestZ = 0f;

            foreach (var holder in tiles)
            {
                if (holder.transform.position.x > farestX) farestX = holder.transform.position.x;
                if (holder.transform.position.x < closestX) closestX = holder.transform.position.x;
                if (holder.transform.position.z > farestZ) farestZ = holder.transform.position.z;
                if (holder.transform.position.z < closestZ) closestZ = holder.transform.position.z;
            }


            width = (farestX - closestX) > (farestZ - closestZ) ? (farestX - closestX) : (farestZ - closestZ);


            var coroutWithdata = new CoroutineWithData(this, meshModifier.ModifyMeshWithHeightMap(tiles, noise, 40f, width, heightCurve));
            while (coroutWithdata.result == null || (coroutWithdata.result.GetType() == typeof(bool) && (bool)coroutWithdata.result == false))
                yield return new WaitForEndOfFrame();

            wcf.SpawnObjects();

            SetUpMinimapPicture(width);
            /*foreach(var holder in tiles)
            {
                holder.meshHolders[0].GetComponent<MeshRenderer>().sharedMaterial = mapMaterials[0];
                holder.meshHolders[1].GetComponent<MeshRenderer>().sharedMaterial = mapMaterials[1];
            }*/
            
        }


        void SetUpMinimapPicture(float mapWidth)
        {
            var cameraSaver = new CameraSaver();
            //minimapCamera.orthographicSize = (mapWidth + radius) * .5f;
            minimapCamera.enabled = true;
            minimapCamera.orthographicSize = 170f;
            minimapImage.sprite = cameraSaver.CameraToSprite(minimapCamera);
            minimapCamera.enabled = false;
        }
        /*private IEnumerator WaitForMapEnd()
        {
            while (!wcf.Done)
            {
                yield return new WaitForEndOfFrame();
            }
        }*/

        private void GenerateBaseMesh()
        {
            for (int i = 0; i < meshData.Layers.Count - 1; i++)
            {
                var smallLayerIt = 0;
                var largeLayerIt = 0;
                while (largeLayerIt < meshData.Layers[i + 1].Length)
                {
                    //Add outside tris
                    var C = meshData.Layers[i][smallLayerIt % meshData.Layers[i].Length];
                    var B = meshData.Layers[i + 1][largeLayerIt % meshData.Layers[i + 1].Length];
                    var A = meshData.Layers[i + 1][(largeLayerIt + 1) % meshData.Layers[i + 1].Length];
                    if (i + 1 == meshData.Layers.Count - 1) meshData.AddTris(A, B, C, i);
                    else meshData.AddTris(A, B, C, i, i + 1);
                    //UpdateMesh();

                    //Add inside tris
                    if (!((largeLayerIt + 1) % ((meshData.Layers[i + 1].Length / 6f)) == 0) && i > 0)
                    {
                        A = meshData.Layers[i + 1][largeLayerIt + 1];
                        B = meshData.Layers[i][smallLayerIt % meshData.Layers[i].Length];
                        C = meshData.Layers[i][(smallLayerIt + 1) % meshData.Layers[i].Length];
                        meshData.AddTris(A, B, C, i, i - 1);
                        //UpdateMesh();
                    }

                    if (!((largeLayerIt + 1) % Mathf.Ceil(meshData.Layers[i + 1].Length / 6f) == 0)) smallLayerIt++;
                    largeLayerIt++;
                }
            }

            foreach (v3Tris triangles in meshData.TrianglesV3)
            {
                triangles.FindNeighbours(meshData);
            }
        }


        private void GenerateLayers(int layerNb, float radius)
        {
            var i = 1;

            meshData.Vertices.Add(origin);
            meshData.Layers.Add(new Vector3[] { origin });
            while (i < layerNb + 1)
            {
                var nHex = GenerateHexagonPoints(origin, (float)i * radius, i - 1);
                meshData.Layers.Add(nHex);
                meshData.Vertices.AddRange(nHex);
                //DebugDrawer.DrawPolygon(nHex, Color.yellow);
                i++;
            }
        }

        private void InitializeBorders()
        { 
            var border = new List<BorderQuad>();
            var borderPt = new List<Vector3>();
            var notBorderQuad = new List<v3Quad>();
            notBorderQuad.AddRange(meshData.Quads);

            for(int i = 0; i < meshData.Quads.Count; i++)
            {
                if(meshData.Quads[i].Neighbours.Count < 4)
                {
                    notBorderQuad.Remove(meshData.Quads[i]);
                    var edgeIndex = new List<int>();
                    var pointList = new List<Vector3>();
                    var quadToAdd = meshData.Quads[i];

                    edgeIndex.AddRange(new int[] { 0, 1, 2, 3 });
                    foreach (Neighbour neigh in meshData.Quads[i].Neighbours)
                        edgeIndex.Remove(neigh.edge);

                    foreach(int index in edgeIndex)
                    {
                        var ptIndex = GetPointIndexByEdge(index);
                        pointList.AddRange(new Vector3[] { meshData.Quads[i].pts[ptIndex[0]], meshData.Quads[i].pts[ptIndex[0]]});
                        borderPt.AddRange(new Vector3[] { meshData.Quads[i].pts[ptIndex[0]], meshData.Quads[i].pts[ptIndex[0]]});
                    }

                    var borderPoint = new BorderQuad(pointList.ToArray(), edgeIndex.ToArray(), quadToAdd);
                    border.Add(borderPoint);
                }
            }

            for (int i = 0; i < notBorderQuad.Count; i++)
            {
                for(int p = 0; p < notBorderQuad[i].pts.Length; p++)
                {
                    for (int b = 0; b < borderPt.Count; b++)
                    {
                        if (notBorderQuad[i].pts[p] == borderPt[b])
                        {
                            var borderPoint = new BorderQuad(new Vector3[] { notBorderQuad[i].pts[p] }, new int[0], notBorderQuad[i]);
                            border.Add(borderPoint);
                        }
                    }
                }
            }

            meshData.BorderQuads.AddRange(border.ToArray());
            corners = borderPt.ToArray();
            meshData.border = borderPt;
        }
        public int[] GetPointIndexByEdge(int edge)
        {
            if (edge == 0)
                // 0 up
                return new int[] { 0, 1 };
            else if (edge == 1)
                // 1 left
                return new int[] { 1, 2 };
            else if (edge == 2)
                // 2 back
                return new int[] { 2, 3 };
            else if (edge == 3)
                // 3 right
                return new int[] { 3, 0 };
            else
                throw new Exception("no edge found");
        }

        private Vector3[] GenerateHexagonPoints(Vector3 origin, float radius, int subDivision = 0)
        {
            var offset = 0f;
            var corners = new Vector3[] {
        new Vector3(Cos(offset + 0), 0, Sin(offset + 0)) * radius + origin,
        new Vector3(Cos(offset + PI / 3f), 0, Sin(offset + PI / 3f)) * radius + origin,
        new Vector3(Cos(offset + (2 * PI) / 3f), 0, Sin(offset + (2 * PI) / 3f)) * radius + origin,
        new Vector3(Cos(offset + (3 * PI) / 3f), 0, Sin(offset + (3 * PI) / 3f)) * radius + origin,
        new Vector3(Cos(offset + (4 * PI) / 3f), 0, Sin(offset + (4 * PI) / 3f)) * radius + origin,
        new Vector3(Cos(offset + (5 * PI) / 3f), 0, Sin(offset + (5 * PI) / 3f)) * radius + origin
        };
            int v = 6 * (subDivision + 1);
            int p = 0;
            var result = new Vector3[v];
            if (subDivision > 0)
            {
                for (int i = 0; i < corners.Length; i++)
                {
                    float it = 1f / ((float)subDivision + 1);
                    result[p] = corners[i];
                    p++;
                    Vector3 nextPt = corners[(i + 1) % corners.Length];
                    Vector3 pt = corners[i];

                    while (it < 1f)
                    {
                        float factorP = (1f - it);
                        float factorM = it;
                        var newPt = (pt * factorP + nextPt * factorM);
                        result[p] = newPt;
                        p++;
                        it += 1f / ((float)subDivision + 1);
                    }
                }
            }


            return subDivision > 0 ? result : corners;
        }

    }


}