using GridGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WCF;
using static GridGenerator.MeshData;

public class MeshModifier
{
    public Mesh mesh;
    public MeshFilter filter;
    public Dictionary<Vector3, PointOnMesh> dict = new Dictionary<Vector3, PointOnMesh>();
    public List<PointOnMesh> ptOnMesh = new List<PointOnMesh>();

    public Vector3[] newVerticies;
    private float height = 100f;
    private float noiseCoef = 20f;

    public Mesh ModifyMesh(Vector3[] irregularCell, Mesh mesh, Vector3 offset)
    {
        var modMesh = new Mesh();
        newVerticies = ModifyMesh(irregularCell, mesh.vertices, offset);


        float lenght = (irregularCell[0] - irregularCell[2]).magnitude > (irregularCell[1] - irregularCell[3]).magnitude ? (irregularCell[0] - irregularCell[2]).magnitude : (irregularCell[1] - irregularCell[3]).magnitude;

        modMesh.vertices = newVerticies;

        modMesh.uv = new Vector2[newVerticies.Length];
        modMesh.triangles = mesh.triangles;
        /*for (int i = 0; i < modMesh.vertices.Length; i++)
        {
            modMesh.uv[i] = new Vector2(modMesh.vertices[i].y / lenght, modMesh.vertices[i].x / lenght);
        }*/
        modMesh.uv = mesh.uv;
        modMesh.RecalculateNormals();
        modMesh.RecalculateBounds();
        modMesh.RecalculateTangents();
        //modMesh.RecalculateUVDistributionMetrics();
        return modMesh;
    }
    public Vector3[] ModifyMesh(Vector3[] irregularCell, Vector3[] points, Vector3 offset)
    {
        var A = irregularCell[0];
        var B = irregularCell[1];
        var C = irregularCell[2];
        var D = irregularCell[3];

        //var modMesh = new Mesh();
        //modMesh = mesh;
        newVerticies = new Vector3[points.Length];
        float minX, minZ;
        minX = minZ = Mathf.Infinity;
        float maxX, maxZ;
        maxX = maxZ = -Mathf.Infinity;

        for (int i = 0; i < points.Length; i++)
        {
            var X = points[i].x;
            var Z = points[i].y;
            //var Z = cell[i].z;
            if (X < minX) minX = X;
            if (X > maxX) maxX = X;
            if (Z < minZ) minZ = Z;
            if (Z > maxZ) maxZ = Z;
        }


        float xFactor = 1f / (maxX - minX);
        float zFactor = 1f / (maxZ - minZ);
        float xOffset = -minX;
        float zOffset = -minZ;

        for (int i = 0; i < points.Length; i++)
        {
            var X = (points[i].x + xOffset) * xFactor;
            var Z = (points[i].y + zOffset) * zFactor;
            var Y = points[i].z * height;
            /*var Z = (cell[i].z + zOffset) * zFactor;
            var Y = cell[i].y * 100;*/

            var Q = Vector3.Lerp(A, B, X);
            var R = Vector3.Lerp(D, C, X);
            var P = Vector3.Lerp(R, Q, Z) + Y * Vector3.up;
            newVerticies[i] = P - offset;
        }

        return newVerticies;
    }

    public Mesh ModifyTileWithHeightMap(TileHolder holder, float[,] noise, float maxHeight, float terrainWidth, AnimationCurve heightCurve)
    {
        var mesh = holder.GetComponent<MeshFilter>().mesh;
        var offset = holder.transform.position;
        var width = noise.GetLength(0);
        var ratio = width / (terrainWidth + 25);

        var newMesh = new Mesh();
        newMesh.vertices = new Vector3[mesh.vertices.Length];
        var vertx = new Vector3[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            var vertex = mesh.vertices[i];
            var x = Mathf.RoundToInt((vertex.x + offset.x) * ratio) + (width / 2) + 1;
            var z = Mathf.RoundToInt((vertex.z + offset.z) * ratio) + (width / 2) + 1;
            var y = vertex.y;
            Debug.Log("MeshModifier, ModifyTileWithHeightMap : x = " + x + ", z = " + z);
            var evaluatedNoise = noise[x, z] * heightCurve.Evaluate(y / 1f);
            vertx[i] = mesh.vertices[i] + (evaluatedNoise * maxHeight - (maxHeight * .5f * heightCurve.Evaluate(0f))) * Vector3.up;
        }
        //newMesh.vertices = mesh.vertices;
        newMesh.vertices = vertx;
        newMesh.triangles = mesh.triangles;
        newMesh.uv = mesh.uv;
        newMesh.RecalculateNormals();
        newMesh.RecalculateBounds();
        newMesh.RecalculateTangents();
        return newMesh;
    }

    private Mesh CalculateNormal(TileHolder holder)
    {
            var tile = holder.Tile;
            var mesh = tile.mesh;
            Vector3[] vertexNormals = new Vector3[mesh.vertices.Length];
            Debug.Log("MeshModifier, CalculateNormal : Calculating Tile in " + tile.Center);
        for (int i = 0; i < mesh.vertices.Length; i++)
            {
                var pt = Round(mesh.vertices[i] + tile.Center);

                if (dict.ContainsKey(pt))
                {
                    for (int x = 0; x < dict[pt].tile.Count; x++)
                    {
                        //if(tile != dict[pt].tile[x]) 
                        //{ 
                            var connectedMesh = dict[pt].tile[x].mesh;
                            Vector3 offset = dict[pt].tile[x].Center;
                            List<int[]> trisList = dict[pt].connectedTris[x];

                            //int triangleCount = connectedMesh.triangles.Length / 3;
                            if(tile != dict[pt].tile[x])
                            {
                                Debug.Log("MeshModifier, CalculateNormal : Tile = " + dict[pt].tile[x].Center + ", trisList.Count = " + trisList.Count + ", pt = " + pt);
                            }
                            for (int y = 0; y < trisList.Count; y++)
                            {
                                //int TrisIndex = i * 3;
                                int A = connectedMesh.triangles[trisList[y][0]];
                                int B = connectedMesh.triangles[trisList[y][1]];
                                int C = connectedMesh.triangles[trisList[y][2]];

                                /*if (connectedMesh.vertices[A] + offset == pt
                                    || connectedMesh.vertices[B] + offset == pt
                                    || connectedMesh.vertices[C] + offset == pt)
                                {*/
                                    Vector3 TrisIndexNormal = SurfaceNormalFromIndices(connectedMesh.vertices, A, B, C);
                                    vertexNormals[i] += TrisIndexNormal;
                                    //vertexNormals[ptIndex] = Vector3.up;
                                    //vertexNormals[ptIndex] = Vector3.zero;
                                //}
                            }
                        
                        //}
                    }
                }
            
        }
        //}
        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
            //vertexNormals[i] = Vector3.up;
        }
        mesh.normals = vertexNormals;
        mesh.RecalculateTangents();
        mesh.RecalculateUVDistributionMetrics();
        return mesh;
    }
    Vector3 SurfaceNormalFromIndices(Vector3[] vertices, int indexA, int indexB, int indexC)
    {
        Vector3 pointA = vertices[indexA];
        Vector3 pointB = vertices[indexB];
        Vector3 pointC = vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public IEnumerator ModifyMeshWithHeightMap(List<TileHolder> tileHolders, float[,] noise, float maxHeight, float mapWidth, AnimationCurve heightCurve)
    {

        dict.Clear();
        foreach (var holder in tileHolders)
        {
            var modMesh = ModifyTileWithHeightMap(holder, noise, maxHeight, mapWidth, heightCurve);
            holder.GetComponent<MeshFilter>().mesh = modMesh;
            holder.Tile.mesh = modMesh;
            holder.GetComponent<MeshCollider>().sharedMesh = modMesh;
            yield return null;
        }

        var coroutWithdata = new CoroutineWithData(tileHolders[0], SetPointOnMesh(tileHolders));
        while (coroutWithdata.result == null || (coroutWithdata.result.GetType() == typeof(bool) && (bool)coroutWithdata.result == false))
            yield return null;

        

        //yield return SetPointOnMesh(tileHolders);;

        /*var filteredDict = new Dictionary<Vector3, PointOnMesh>();
        foreach (var key in dict.Keys)
        {
            if (dict[key].tile.Count > 1) filteredDict.Add(key, dict[key]);
        }
        dict = filteredDict;*/

        foreach (var holder in tileHolders)
        {
            holder.Tile.mesh = CalculateNormal(holder);
            holder.GetComponent<MeshFilter>().mesh = holder.Tile.mesh;
            yield return null;
        }
        yield return true;

    }
    public Vector2 Round(Vector3 vector, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
          Mathf.Round(vector.x * multiplier) / multiplier,
            Mathf.Round(vector.y * multiplier) / multiplier,
            Mathf.Round(vector.z * multiplier) / multiplier);
    }
    private IEnumerator SetPointOnMesh(List<TileHolder> tileHolders)
    {
        foreach (var holder in tileHolders)
        {
            var offset = holder.transform.position;
            holder.Tile.Center = offset;
            var mesh = holder.Tile.mesh;

            for (int v = 0; v < mesh.vertices.Length; v++)
            {
                var pointPosition = mesh.vertices[v];

                if (!dict.ContainsKey(Round(pointPosition + offset, 2)))
                {
                    var pt = new PointOnMesh(Round(pointPosition + offset, 2));
                    dict.Add(Round(pointPosition + offset, 2), pt);
                    ptOnMesh.Add(pt);

                }
                var trisList = new List<int[]>();

                int triangleCount = mesh.triangles.Length / 3;
                for (int t = 0; t < triangleCount; t++)
                {
                    int TrisIndex = t * 3;
                    int A = mesh.triangles[TrisIndex];
                    int B = mesh.triangles[TrisIndex + 1];
                    int C = mesh.triangles[TrisIndex + 2];

                    /*if (mesh.vertices[A] == pointPosition
                        || mesh.vertices[B] == pointPosition
                        || mesh.vertices[C] == pointPosition)*/
                    if ((mesh.vertices[A] - pointPosition).magnitude < 0.1f
                        || (mesh.vertices[B] - pointPosition).magnitude < 0.1f
                        || (mesh.vertices[C] - pointPosition).magnitude < 0.1f)
                    {
                        trisList.Add(new int[] { TrisIndex, TrisIndex + 1, TrisIndex + 2 });
                    }
                }

                var pointOnMesh = dict[Round(pointPosition + offset, 2)];
                pointOnMesh.tile.Add(holder.Tile);
                pointOnMesh.ptNb.Add(v);
                pointOnMesh.connectedTris.Add(trisList);
                //dict[pointPosition + offset] = pointOnMesh;

            }
            yield return null;
        }
        yield return true;
    }
}

