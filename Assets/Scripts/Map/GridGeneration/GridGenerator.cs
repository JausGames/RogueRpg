using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WCF;
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
        public Material mapMaterial;




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

            meshData.SmoothGrid();

            StartCoroutine(wcf.StartWave(meshData.Quads.ToArray(), mapMaterial));

        }


        private void Start()
        {
            //InitializeBorders();


            /*meshData = new MeshData(new List<Vector3>(), radius, layerNb, tiling);
            GenerateLayers(layerNb, radius);
            InitializeBorders();
            GenerateBaseMesh();

            foreach (MeshData.v3Tris triangles in meshData.TrianglesV3)
            {
                triangles.FindNeighbours(meshData);
            }

            meshData.DeleteTrisRandomly();

            meshData.SubdivideGrid();
            meshData.SubdivideGrid();
            meshData.SubdivideGrid();


            StartCoroutine(meshData.SmoothGrid());*/
        }

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
            var border = new List<Vector3>();
            for(int i = 0; i < meshData.Quads.Count; i++)
            {
                if(meshData.Quads[i].Neighbours.Count < 4)
                {
                    var edgeIndex = new List<int>();
                    edgeIndex.AddRange(new int[] { 0, 1, 2, 3 });
                    foreach (Neighbour neigh in meshData.Quads[i].Neighbours)
                        edgeIndex.Remove(neigh.edge);
                    foreach(int index in edgeIndex)
                    {
                        var ptIndex = GetPointIndexByEdge(index);
                        border.Add(meshData.Quads[i].pts[ptIndex[0]]);
                        border.Add(meshData.Quads[i].pts[ptIndex[1]]);
                    }
                }
            }


            corners = border.ToArray();
            meshData.border = border;
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

        private void OnDrawGizmos()
        {
            return;
            foreach (var pt in corners)
            {
                var color = Color.red;
                Gizmos.color = color;
                Gizmos.DrawSphere(pt, .3f);
            }
            if (meshData == null) return;

            foreach (var pt in meshData.Vertices)
            {
                var color = Color.white;
                Gizmos.color = color;
                Gizmos.DrawSphere(pt, .01f);
            }
            if (meshData.processedTris == null) return;
            foreach (var tris in meshData.TrianglesV3)
            {
                var inQuad = false;
                foreach (var quad in meshData.Quads)
                    if (quad.internalTris[0] == tris
                        || quad.internalTris[1] == tris) inQuad = true;
                if (!meshData.processedTris.Contains(tris)
                    && !inQuad)
                {
                    var color = Color.cyan;
                    Gizmos.color = color;
                    var sum = new Vector3();
                    for (int i = 0; i < tris.pts.Length; i++)
                    {
                        sum += tris.pts[i];
                    }
                    Gizmos.DrawSphere(sum / tris.pts.Length, .1f);
                }

            }
            foreach (var tris in meshData.ToBeProcess)
            {
                var color = Color.cyan;
                Gizmos.color = color;
                var sum = new Vector3();
                for (int i = 0; i < tris.pts.Length; i++)
                {
                    sum += tris.pts[i];
                    Gizmos.DrawLine(tris.pts[i], tris.pts[i % tris.pts.Length]);
                }
                Gizmos.DrawSphere(sum / tris.pts.Length, .1f);
            }
            if (meshData.processedTris == null) return;
            foreach (var tris in meshData.processedTris)
            {
                var color = Color.green;
                Gizmos.color = color;
                var sum = new Vector3();
                for (int i = 0; i < tris.pts.Length; i++)
                {
                    sum += tris.pts[i];
                    Gizmos.DrawLine(tris.pts[i], tris.pts[i % tris.pts.Length]);
                }
                Gizmos.DrawSphere(sum / tris.pts.Length, .3f);

            }
            if (meshData.Quads == null) return;
            foreach (var quad in meshData.Quads)
            {
                var color = Color.yellow;
                Gizmos.color = color;
                var sum = new Vector3();
                for (int i = 0; i < quad.pts.Length; i++)
                {
                    sum += quad.pts[i];
                    Gizmos.DrawLine(quad.pts[i], quad.pts[i % quad.pts.Length]);
                    //Gizmos.DrawSphere(quad.pts[i], .3f);
                }
                Gizmos.DrawSphere(sum / quad.pts.Length, .2f);
            }
        }
    }


}