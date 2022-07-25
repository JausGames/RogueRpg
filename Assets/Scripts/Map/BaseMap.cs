using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public class BaseMap : MonoBehaviour
{
    // DEBUG 
    int startLayer = 0;
    //
    private const int layerNb = 3;
    public MeshData meshData = null;
    float radius = 3f;
    public DebugDrawer DebugDrawer = null;
    Vector3 origin = new Vector3(0, 0, 0);
    public Vector3[] corners;

    //texture
    float tiling = 1/10f;
    public Material mapMaterial;

    MeshFilter filter = null;
    MeshRenderer renderer;




    /*private void Awake()
    {
        //InitializeBorders();

        meshData = new MeshData(new List<Vector3>(), radius, layerNb, tiling);
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

        
        StartCoroutine(meshData.SmoothGrid());
    }*/
    private void Start()
    {
        //InitializeBorders();

        meshData = new MeshData(new List<Vector3>(), radius, layerNb, tiling);
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

        
        StartCoroutine(meshData.SmoothGrid());
    }

    private void GenerateBaseMesh()
    {

        filter = gameObject.AddComponent<MeshFilter>();
        renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.material = new Material(mapMaterial);

        var tris = new List<int>();
        var normals = new List<Vector3>();
        var tangents = new List<Vector4>();

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
                if(i+1==meshData.Layers.Count - 1) meshData.AddTris(A, B, C, i);
                else meshData.AddTris(A, B, C, i , i + 1);
                UpdateMesh();

                //Add inside tris
                if (!((largeLayerIt + 1) % ((meshData.Layers[i + 1].Length / 6f)) == 0) && i > 0)
                {
                    A = meshData.Layers[i + 1][largeLayerIt + 1];
                    B = meshData.Layers[i][smallLayerIt % meshData.Layers[i].Length];
                    C = meshData.Layers[i][(smallLayerIt + 1) % meshData.Layers[i].Length];
                    meshData.AddTris(A, B, C, i, i - 1);
                    UpdateMesh();
                }

                if (!((largeLayerIt + 1) % Mathf.Ceil(meshData.Layers[i + 1].Length / 6f) == 0)) smallLayerIt++;
                largeLayerIt++;
            }
        }
    }


    private void UpdateMesh()
    {
        filter.mesh.Clear();
        filter.mesh = meshData.CreateMesh();
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
        corners = meshData.Layers[meshData.Layers.Count - 1];
        meshData.border = corners;
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
        /*foreach (var tris in meshData.TrianglesV3)
        {
            var inQuad = false;
            foreach (var quad in meshData.Quads)
                if (quad.internalTris[0] == tris
                    || quad.internalTris[1] == tris) inQuad = true;
            if(!meshData.processedTris.Contains(tris)
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

        }*/
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
        /*if (meshData.processedTris == null) return;
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

        }*/
        if (meshData.Quads == null) return;
        foreach(var quad in meshData.Quads)
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
    private List<v3Quad> quads = new List<v3Quad>() ;

    //third phase
    private float radius;
    private int layerNb;
    private float tiling;

    //Debuging
    private List<v3Tris> initialItems;
    private List<v3Tris> toBeProcessTris;
    private List<v3Tris> processed;
    public List<v3Tris> processedTris;
    internal Vector3[] border;

    public List<Vector3> Vertices { get => vertices; set => vertices = value; }
    public List<v3Tris> TrianglesV3 { get => trianglesV3; set => trianglesV3 = value; }
    public List<Vector3[]> Layers { get => layers; set => layers = value; }
    public List<v3Quad> Quads { get => quads; set => quads = value; }
    public List<v3Tris> ToBeProcess { get => toBeProcessTris; set => toBeProcessTris = value; }

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

        //for(int i = 0; i < toBeProcessTris.Count; i++)
        //foreach(toBeProcessTris.Count > 0 || time > 10)
        while((processed.Count + processedTris.Count) < initialItems.Count)
        {
            var i = UnityEngine.Random.Range(0, toBeProcessTris.Count);
            if(!processed.Contains(toBeProcessTris[i]) && !processedTris.Contains(toBeProcessTris[i]))
            {
                var v3Neighbour = Delete1Point(toBeProcessTris[i], processed, processedTris);

               toBeProcessTris.Remove(toBeProcessTris[i]);
               toBeProcessTris.Remove(v3Neighbour);
            }
        }

        v3Poly.AddRange(quads);
        v3Poly.AddRange(processedTris);

    }


    private v3Tris Delete1Point(v3Tris v3Tris, List<v3Tris> processed, List<v3Tris> processedTris)
    {
        var freeNeighours = new List<int>();
        for(int i = 0; i < v3Tris.Neighbours.Count; i++)
            if (!processed.Contains((v3Tris)v3Tris.Neighbours[i]) && !processedTris.Contains((v3Tris)v3Tris.Neighbours[i])) freeNeighours.Add(i);
        //var fakeNeighs = new List<>
        //foreach (int i in freeNeighours)
        //if (processed.Contains(v3Tris.neighbours[i])) freeNeighours.Remove(i);

        /*foreach(var neigh in freeNeighours)
        {
            v3Tris.Neighbours[neigh].chec;
        }*/

        if (freeNeighours.Count > 0)
        {
            var rnd = UnityEngine.Random.Range(0, freeNeighours.Count);
            var pickNeghbour = v3Tris.Neighbours[freeNeighours[rnd]];
            var quad = MergeTris(v3Tris,(v3Tris) pickNeghbour);

            processed.Add(v3Tris);
            processed.Add((v3Tris)pickNeghbour);
            quads.Add(quad);
            return (v3Tris)pickNeghbour;
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
        tris1Alone.AddRange(new int[] { 0, 1, 2});
        var tris2Alone = new List<int>();
        tris2Alone.AddRange(new int[] { 0, 1, 2});

        var tris1Common = new List<int>();
        var tris2Common = new List<int>();

        for(int i = 0; i < v3Tris1.pts.Length; i++)
            for(int j = 0; j < v3Tris2.pts.Length; j++)
            {
                if (v3.isSamePoint(v3Tris1.pts[i], v3Tris2.pts[j])){
                    tris1Common.Add(i);
                    tris1Alone.Remove(i);
                    tris2Common.Add(j);
                    tris2Alone.Remove(j);
                }
            }

        var quadV = new Vector3[] { v3Tris1.pts[tris1Alone[0]], v3Tris1.pts[tris1Common[0]], v3Tris2.pts[tris2Alone[0]], v3Tris1.pts[tris1Common[1]]};

        var neighs = new List<v3>();
        foreach (v3Tris tris in v3Tris1.Neighbours)
            if (tris != v3Tris2 && !neighs.Contains(tris)) neighs.Add(tris);

        foreach (v3Tris tris in v3Tris2.Neighbours)
            if (tris != v3Tris1 && !neighs.Contains(tris)) neighs.Add(tris);

        var quad = new v3Quad(quadV, neighs, v3Tris1, v3Tris2);
        //DebugDrawer.DrawPolygon(quad.pts, Color.blue);
        return quad;
    }

    internal void SubdivideGrid()
    {
        var newQuads = new List<v3Quad>();

        for(int i = 0; i < v3Poly.Count; i ++)
        {
            newQuads.AddRange(v3Poly[i].Subdivide(this));
        }
        for(int i = 0; i < quads.Count; i ++)
        {
            newQuads.AddRange(quads[i].Subdivide(this));
        }
        quads.Clear();
        v3Poly.Clear();
        quads.AddRange(newQuads);

        foreach (v3Quad quad in quads)
        {
            quad.FindNeighbours(this);
        }
    }

    internal IEnumerator SmoothGrid()
    {
        var dict = new Dictionary<Vector3, List<pointOnQuad>>();
        var keys = new List<Vector3>();

        for(int i = 0; i < quads.Count; i++)
            for(int j = 0; j < quads.Count; j++)
            {
                if(i != j)
                {
                    for (int a = 0; a < quads[i].pts.Length; a++)
                        for (int b = 0; b < quads[j].pts.Length; b++)
                            if (v3.isSamePoint(quads[i].pts[a], quads[j].pts[b]))
                            {
                                if (!dict.ContainsKey(quads[i].pts[a]))
                                {
                                    dict.Add(quads[i].pts[a], new List<pointOnQuad>());
                                    keys.Add(quads[i].pts[a]);
                                }
                                    dict[quads[i].pts[a]].Add(new pointOnQuad(quads[i], a));
                                    dict[quads[j].pts[b]].Add(new pointOnQuad(quads[j], b));
                            }
                }
            }

        var it = 0;
        while (it < 10)
        {
            /*foreach(var pt in vertices)
            {
                var quads = v3.GetQuadWithCommonPt(pt, this);
                foreach(v3Quad quad in quads)
                    quad.SmoothPoint(pt, this);
            }*/
            foreach (MeshData.v3Quad quad in Quads)
            {
                foreach (var pt in quad.pts)
                {
                    quad.SmoothPoint(pt, this);
                }
            }
                    yield return new WaitForEndOfFrame();


            it++;
        }

            for(int i = 0; i < keys.Count; i++)
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
        

        for(int i = 0; i < keys.Count; i++)
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

        foreach(v3Quad quad in quads)
        {
            DebugDrawer.DrawPolygon(quad.pts, Color.blue);
            yield return new WaitForEndOfFrame();
        }
    }

    public class v3
    {
        public Vector3[] pts;
        public List<v3> neighbours;
        [HideInInspector]
        public List<v3> Neighbours { get => neighbours; set => neighbours = value; }

        public virtual List<v3Quad> Subdivide(MeshData data)
        {
            throw new NotImplementedException();
        }
        public virtual void FindNeighbours(MeshData data)
        {
            foreach (v3Tris tris in data.v3Poly)
            {
                if (tris != this)
                {
                    var nbCommonPt = 0;
                    foreach(var pt in tris.pts)
                        if (haveCommonPoint(pt)) nbCommonPt++;
                    if (nbCommonPt >= 2) neighbours.Add(tris);
                }
            }
        }
        protected bool haveCommonPoint(Vector3 pt)
        {
            foreach(var ownPt in pts)
                if(isSamePoint(pt, ownPt)) return true;
            return false;
        }
        static public bool isSamePoint(Vector3 pt1, Vector3 pt2)
        {
            if (pt1.x == pt2.x
                && pt1.y == pt2.y
                && pt1.z == pt2.z) return true;
            return false;
        }
        static public List<v3> GetQuadWithCommonPt(Vector3 pt, MeshData meshData)
        {
            List<v3> result = new List<v3>();

            foreach (var quad in meshData.quads)
                foreach (var ptIt in quad.pts)
                    if (isSamePoint(pt, ptIt) && !result.Contains(quad)) result.Add(quad);

            return result;
        }
        public List<Vector3> GetCommonPoints(v3 poly1, v3 poly2)
        {
            List<Vector3> result = new List<Vector3>();

            foreach (var pt1 in poly1.pts)
                foreach (var pt2 in poly2.pts)
                    if (isSamePoint(pt1, pt2)) result.Add(pt1);
            //data.v3Poly.Remove(this);
            return result;
        }
        public List<Vector3> GetAdjacentPoints(Vector3 point, MeshData meshData)
        {
            List<Vector3> result = new List<Vector3>();
            var id = -1;
            for (int i = 0; i < pts.Length; i++)
                if (isSamePoint(point, pts[i])) id = i;

            result.AddRange(new Vector3[] { pts[(id + 1) % pts.Length], pts[(id + pts.Length - 1) % pts.Length] });

            foreach (var quads in meshData.Quads)
            {
                for (int i = 0; i < quads.pts.Length; i++)
                    if (isSamePoint(point, quads.pts[i]))
                    {
                        var add = true;
                        foreach (var pt in result)
                            if (isSamePoint(pt, quads.pts[(i + 1) % quads.pts.Length])) add = false;
                        if(add) result.Add(quads.pts[(i + 1) % quads.pts.Length]);

                        add = true;
                        foreach (var pt in result)
                            if (isSamePoint(pt, quads.pts[(i + quads.pts.Length - 1) % quads.pts.Length])) add = false;
                        if (add) result.Add(quads.pts[(i + quads.pts.Length - 1) % quads.pts.Length]) ;
                    }
                

            }
                
            Debug.Log("v3, GetAdjacentPoints : nb adjacentPoint = " + result.Count);
            return result;
        }

        private List<Vector3> RemoveDoubles(List<Vector3> list)
        {
            var result = new List<Vector3>();

            foreach(Vector3 vect in list)
            {
                var doAdd = true;
                foreach (Vector3 res in result)
                    if (isSamePoint(res, vect)) 
                        doAdd = false;
                if (doAdd) result.Add(vect);
            }
            return result;
        }
    }
    

    [Serializable]
    public class v3Tris : v3
    {

        public int layerNb, neighbourLayer = 0;
        public v3Tris(Vector3[] pts, int layerNb = 0, int neighbourLayer = 0)
        {
            this.pts = pts;
            neighbours = new List<v3>();
            this.layerNb = layerNb;
            this.neighbourLayer = neighbourLayer;
        }
        public override void FindNeighbours(MeshData data)
        {
            int idInLayer = -1;
            for (int i = 0; i < data.v3Layers[layerNb].Count; i++)
            {
                if (data.v3Layers[layerNb][i] == this) idInLayer = i;
            }
            if (idInLayer != -1)
            {
                var nextLayer = (idInLayer + 1) % data.v3Layers[layerNb].Count;
                neighbours.Add(data.v3Layers[layerNb][nextLayer]);
                var previousLayer = (idInLayer + data.v3Layers[layerNb].Count - 1) % data.v3Layers[layerNb].Count;
                neighbours.Add(data.v3Layers[layerNb][previousLayer]);
            }
            if (neighbourLayer < data.v3Layers.Count - 1 && neighbourLayer != -1)
                foreach (v3Tris tris in data.v3Layers[neighbourLayer])
                {
                    if (tris != this)
                    {
                        var nbCommonPt = 0;
                        if (haveCommonPoint(tris.pts[0])) nbCommonPt++;
                        if (haveCommonPoint(tris.pts[1])) nbCommonPt++;
                        if (haveCommonPoint(tris.pts[2])) nbCommonPt++;
                        if (nbCommonPt >= 2) neighbours.Add(tris);
                    }
                }
        }
        public override List<v3Quad> Subdivide(MeshData data)
        {
            var middle = (pts[0] + pts[1] + pts[2]) / 3f;

            var result = new List<v3Quad>();
            for (int i = 0; i < pts.Length; i++)
            {
                /*var ptss = new Vector3[] { pts[i], (pts[i] + pts[(i + 1) % pts.Length]) / 2f, middle};
                var newTris = new v3Tris(ptss);*/
                var ptss = new Vector3[] { pts[i], (pts[i] + pts[(i + 1) % pts.Length]) / 2f, middle, (pts[(i + pts.Length - 1) % pts.Length] + pts[i]) / 2f };
                var newQuad = new v3Quad(ptss);
                //data.Quads.Add(newQuad);
                //data.v3Poly.Add(newQuad);
                //DebugDrawer.DrawPolygon(newQuad.pts, Color.blue);
                result.Add(newQuad);
            }
            //data.v3Poly.Remove(this);
            return result;
        }
    }
        [Serializable]
        public class v3Quad : v3
        {
            public v3Tris[] internalTris = new v3Tris[2];

            public v3Quad(Vector3[] pts, List<v3> neighbours, v3Tris tris1, v3Tris tris2)
            {
                this.pts = pts;
                Neighbours = neighbours;
                internalTris[0] = tris1;
                internalTris[1] = tris2;
            }
        public v3Quad(Vector3[] pts)
        {
            this.pts = pts;
            neighbours = new List<v3>();
        }
        public override void FindNeighbours(MeshData data)
        {
            foreach (v3Quad quad in data.Quads)
            {
                if (quad != this)
                {
                    var nbCommonPt = 0;
                    foreach (var pt in quad.pts)
                        if (haveCommonPoint(pt)) nbCommonPt++;
                    if (nbCommonPt >= 2) neighbours.Add(quad);
                }
            }
        }
        public override List<v3Quad> Subdivide(MeshData data)
        {
            var middle = (pts[0] + pts[1] + pts[2]+ pts[3]) / 4f;

            var result = new List<v3Quad>();
            for (int i = 0; i < pts.Length; i++)
            {
                var ptss = new Vector3[] { pts[i], (pts[i] + pts[(i + 1) % pts.Length]) / 2f, middle, (pts[(i + pts.Length - 1) % pts.Length] + pts[i]) / 2f };
                var newQuad = new v3Quad(ptss);
                //data.Quads.Add(newQuad);
                result.Add(newQuad);
            }
            //data.v3Poly.Remove(this);
            return result;
        }

        internal void SmoothPoint(Vector3 pt, MeshData meshData)
        {
            for(int i = 0; i < meshData.border.Length; i++) 
                if(v3.isSamePoint(pt, meshData.border[i])
                        || v3.isSamePoint(pt, (meshData.border[(i + 1) % meshData.border.Length] + meshData.border[i]) / 2f)
                        || v3.isSamePoint(pt, (meshData.border[(i + meshData.border.Length - 1) % meshData.border.Length] + meshData.border[i]) / 2f)
                        ) return;

            float x, y;
            x = .3f;
            y = .3f;

            var id = -1;
            for (int i = 0; i < pts.Length; i++)
                if (isSamePoint(pt, pts[i])) id = i;

            var otherPoint = new List<Vector3>() { pts[(id + 1) % pts.Length], pts[(id + pts.Length - 1) % pts.Length] };
            var oppositePt = pts[(id + 2) % pts.Length];

            var Q = Vector3.Lerp(pt, otherPoint[0], x);
            var R = Vector3.Lerp(otherPoint[1], oppositePt, x);
            var P = Vector3.Lerp(Q, R, y);

            pts[id] = P;
        }
    }
}

public class pointOnQuad
{
    public MeshData.v3Quad quad;
    public int ptNb;

    public pointOnQuad(MeshData.v3Quad quad, int ptNb)
    {
        this.quad = quad;
        this.ptNb = ptNb;
    }
}