using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GridGenerator
{
    [Serializable]
    public class MeshData
    {
        [SerializeField]
        List<Vector3> vertices = new List<Vector3>();
        [SerializeField]
        List<int> triangles = new List<int>();

        private List<Vector3[]> layers = new List<Vector3[]>();

        //first phase
        private List<v3> v3Poly = new List<v3>();
        private List<v3Tris> trianglesV3 = new List<v3Tris>();
        private List<List<v3Tris>> v3Layers = new List<List<v3Tris>>();
        //private List<v3Tris> quad = new List<v3Tris>();

        //second phase
        [SerializeField]
        private List<v3Quad> quads = new List<v3Quad>();

        //third phase
        private float radius;
        private int layerNb;
        private float tiling;

        //Debuging
        private List<v3Tris> initialItems;
        private List<v3Tris> toBeProcessTris;
        private List<v3Tris> processed;
        public List<v3Tris> processedTris;
        [SerializeField]
        public List<Vector3> border = new List<Vector3>();
        public bool done = false;

        public List<Vector3> Vertices { get => vertices; set => vertices = value; }
        public List<v3Tris> TrianglesV3 { get => trianglesV3; set => trianglesV3 = value; }
        public List<Vector3[]> Layers { get => layers; set => layers = value; }
        public List<v3Quad> Quads { get => quads; set => quads = value; }
        public List<v3Tris> ToBeProcess { get => toBeProcessTris; set => toBeProcessTris = value; }
        public List<v3> V3Poly { get => v3Poly; set => v3Poly = value; }

        public MeshData(List<Vector3> vertices, float radius, int layerNb, float tiling, List<Vector3[]> layers = null)
        {
            this.vertices = vertices;
            this.radius = radius;
            this.layerNb = layerNb;
            this.tiling = tiling;
            if (layers != null) this.layers = layers;
            else this.layers = new List<Vector3[]>();

        }

        public void AddTris(Vector3 A, Vector3 B, Vector3 C, int layerNb, int neighbourLayer = -1)
        {
            var tris = new v3Tris(new Vector3[] { A, B, C }, layerNb, neighbourLayer);
            trianglesV3.Add(tris);
            if (v3Layers.Count == layerNb)
                v3Layers.Add(new List<v3Tris>());

            v3Layers[layerNb].Add(tris);

            triangles.Add(FindPointId(A));
            triangles.Add(FindPointId(B));
            triangles.Add(FindPointId(C));
            //DebugDrawer.DrawTriangle(A, B, C, new Color(255, 255, 100, 100));
        }
        private Vector4[] GetTangentArray(Vector3[] vector3s)
        {
            Vector4[] results = new Vector4[vector3s.Length];
            for (int i = 0; i < vector3s.Length; i++)
                results[i] = new Vector4(1f, 0f, 0f, -1f);
            return results;
        }
        private Vector3[] GetNormalArray(Vector3[] vector3s)
        {
            Vector3[] results = new Vector3[vector3s.Length];
            for (int i = 0; i < vector3s.Length; i++)
                results[i] = Vector3.up;
            return results;
        }
        private int FindPointId(Vector3 pt)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (pt.x == vertices[i].x
                    && pt.y == vertices[i].y
                    && pt.z == vertices[i].z) return i;
            }
            return -1;
        }
        private Vector2[] GetUvs(Vector3[] vector3s)
        {
            Vector2[] vector2s = new Vector2[vector3s.Length];
            for (int i = 0; i < vector3s.Length; i++)
                vector2s[i] = (vector3s[i].x * Vector2.right) / (radius * layerNb * tiling) + (vector3s[i].z * Vector2.up) / (radius * layerNb * tiling);
            return vector2s;
        }
        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = GetUvs(mesh.vertices);
            mesh.tangents = GetTangentArray(mesh.vertices);
            mesh.normals = GetNormalArray(mesh.vertices);
            return mesh;
        }

        public void DeleteTrisRandomly()
        {
            initialItems = new List<v3Tris>();
            toBeProcessTris = new List<v3Tris>();
            processed = new List<v3Tris>();
            processedTris = new List<v3Tris>();

            initialItems.AddRange(trianglesV3);
            toBeProcessTris.AddRange(trianglesV3);

            while ((processed.Count + processedTris.Count) < initialItems.Count)
            {
                var i = UnityEngine.Random.Range(0, toBeProcessTris.Count);
                if (!processed.Contains(toBeProcessTris[i]) && !processedTris.Contains(toBeProcessTris[i]))
                {
                    var v3Neighbour = Delete1Point(toBeProcessTris[i], processed, processedTris);

                    toBeProcessTris.Remove(toBeProcessTris[i]);
                    toBeProcessTris.Remove(v3Neighbour);
                }
            }

            v3Poly.AddRange(processedTris);
            trianglesV3.Clear();

        }


        private v3Tris Delete1Point(v3Tris v3Tris, List<v3Tris> processed, List<v3Tris> processedTris)
        {
            var freeNeighours = new List<int>();
            for (int i = 0; i < v3Tris.Neighbours.Count; i++)
                if (!processed.Contains((v3Tris)v3Tris.Neighbours[i].neighbour) && !processedTris.Contains((v3Tris)v3Tris.Neighbours[i].neighbour)) freeNeighours.Add(i);

            if (freeNeighours.Count > 0)
            {
                var rnd = UnityEngine.Random.Range(0, freeNeighours.Count);
                var pickNeghbour = v3Tris.Neighbours[freeNeighours[rnd]];
                var quad = MergeTris(v3Tris, (v3Tris)pickNeghbour.neighbour);

                processed.Add(v3Tris);
                processed.Add((v3Tris)pickNeghbour.neighbour);
                quads.Add(quad);
                return (v3Tris)pickNeghbour.neighbour;
            }
            else
            {
                processedTris.Add(v3Tris);
                //DebugDrawer.DrawPolygon(v3Tris.pts, Color.white);
                return null;
            }
        }

        private v3Quad MergeTris(v3Tris v3Tris1, v3Tris v3Tris2)
        {
            var tris1Alone = new List<int>();
            tris1Alone.AddRange(new int[] { 0, 1, 2 });
            var tris2Alone = new List<int>();
            tris2Alone.AddRange(new int[] { 0, 1, 2 });

            var tris1Common = new List<int>();
            var tris2Common = new List<int>();

            // We can the common points between 2 tris
            for (int i = 0; i < v3Tris1.pts.Length; i++)
                for (int j = 0; j < v3Tris2.pts.Length; j++)
                {
                    if (v3.isSamePoint(v3Tris1.pts[i], v3Tris2.pts[j]))
                    {
                        tris1Common.Add(i);
                        tris1Alone.Remove(i);
                        tris2Common.Add(j);
                        tris2Alone.Remove(j);
                    }
                }

            //We create the quad points from what find above
            var quadV = new Vector3[] { v3Tris1.pts[tris1Alone[0]], v3Tris1.pts[tris1Common[0]], v3Tris2.pts[tris2Alone[0]], v3Tris1.pts[tris1Common[1]] };

            // We create the quad neighbour list form the 2 tris neighbours
            var neighbours = new List<Neighbour>();
            foreach (Neighbour neigh in v3Tris1.Neighbours)
                if (neigh.neighbour != v3Tris2)
                {
                    var okay = true;
                    foreach (var testedNeigh in neighbours)
                        if (testedNeigh.IsSameQuad(neigh.neighbour)) okay = false;

                    if (okay) neighbours.Add(neigh);
                }

            foreach (Neighbour neigh in v3Tris2.Neighbours)
                if (neigh.neighbour != v3Tris1)
                {
                    var okay = true;
                    foreach (var testedNeigh in neighbours)
                        if (testedNeigh.IsSameQuad(neigh.neighbour)) okay = false;

                    if (okay) neighbours.Add(neigh);
                }

            //Then we create the final quad from these data;
            var quad = new v3Quad(quadV, neighbours, v3Tris1, v3Tris2);
            return quad;
        }

        internal void SubdivideGrid()
        {
            var newQuads = new List<v3Quad>();

            v3Poly.AddRange(quads);
            for (int i = 0; i < v3Poly.Count; i++)
            {
                var subdivided = v3Poly[i].Subdivide(this);
                newQuads.AddRange(subdivided);
            }
            
            quads.Clear();
            v3Poly.Clear();
            quads.AddRange(newQuads);


            foreach (v3Quad quad in quads)
            {
                quad.FindNeighbours(this);
            }
        }
        public Vector3[] CorrectQuad(v3Quad quad)
        {
            var angle = Vector3.SignedAngle(quad.pts[1] - quad.pts[0], quad.pts[3] - quad.pts[0], Vector3.up);
            if (angle > 0)
            {
                var A = quad.pts[0];
                var B = quad.pts[1];
                var C = quad.pts[2];
                var D = quad.pts[3];

                var newPts = new Vector3[] { A, D, C, B };
                return newPts;
            }
            else return quad.pts;
        }

        internal void SmoothGrid()
        {
            var dict = new Dictionary<Vector3, List<pointOnQuad>>();
            var dictBasicPos = new Dictionary<Vector3, List<pointOnQuad>>();
            var keys = new List<Vector3>();
            var borderKeys = new List<Vector3>();

            for (int i = 0; i < quads.Count; i++)
                for (int j = 0; j < quads.Count; j++)
                {
                    if (i != j)
                    {
                        for (int a = 0; a < quads[i].pts.Length; a++)
                            for (int b = 0; b < quads[j].pts.Length; b++)
                            {
                                if (v3.isSamePoint(quads[i].pts[a], quads[j].pts[b]))
                                {
                                    if (!dict.ContainsKey(quads[i].pts[a]))
                                    {
                                        dict.Add(quads[i].pts[a], new List<pointOnQuad>());
                                        keys.Add(quads[i].pts[a]);
                                    }
                                    var poq1 = new pointOnQuad(quads[i], a);
                                    var poq2 = new pointOnQuad(quads[j], b);
                                    dict[quads[i].pts[a]].Add(poq1);
                                    dict[quads[j].pts[b]].Add(poq2);

                                }
                            }
                    }
                }


                var it = 0;
                while (it < 100)
                {
                    foreach (v3Quad quad in Quads)
                    {
                        quad.SelfSmooth(border);
                    }


                    it++;
                }
                for (int i = 0; i < keys.Count; i++)
                {
                    var ptsOnQuad = dict[keys[i]];
                    var mid = Vector3.zero;
                    foreach (var ptOnQuad in ptsOnQuad)
                    {
                        mid += ptOnQuad.quad.pts[ptOnQuad.ptNb];
                    }
                    mid /= ptsOnQuad.Count;
                    foreach (var ptOnQuad in ptsOnQuad)
                    {
                        ptOnQuad.quad.pts[ptOnQuad.ptNb] = mid;
                    }
                }

            }


        public class pointOnQuad
        {
            public v3Quad quad;
            public int ptNb;

            public pointOnQuad(v3Quad quad, int ptNb)
            {
                this.quad = quad;
                this.ptNb = ptNb;
            }
        }
        public class ConnectedPoints
        {
            public List<Vector3> points = new List<Vector3>();
        }
        public class ConnectedCrossPoints
        {
            public List<Vector3> points = new List<Vector3>();
        }
    }
}