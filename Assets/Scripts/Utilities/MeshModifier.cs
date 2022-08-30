using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshModifier
{
    public Mesh mesh;
    public MeshFilter filter;

    public Vector3[] newVerticies;
    private float height = 50f;
    private float noiseCoef = 20f;

    // Start is called before the first frame update
    void Start()
    {
        /*var modMesh = new Mesh();
        newVerticies = ModifyMesh(irrPoints, mesh.vertices);
        modMesh.vertices = newVerticies;
        modMesh.triangles = mesh.triangles;
        modMesh.RecalculateNormals();
        modMesh.RecalculateBounds();
        modMesh.RecalculateTangents();
        filter.mesh = modMesh;*/
    }

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

        float xOffset = 0;
        float zOffset = 0;
        float xFactor = 1;
        float zFactor = 1;
        for (int i = 0; i < points.Length; i++)
        {
            var X = points[i].x;
            var Z = points[i].y;
            //var Z = cell[i].z;
            if (X < minX)
                minX = X;
            if (X > maxX)
                maxX = X;
            if (Z < minZ)
                minZ = Z;
            if (Z > maxZ)
                maxZ = Z;
        }


        xFactor = 1f / (maxX - minX);
        zFactor = 1f / (maxZ - minZ);
        xOffset = -minX;
        zOffset = -minZ;

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
        var ratio = width / (terrainWidth + 15);

        var newMesh = new Mesh();
        newMesh.vertices = new Vector3[mesh.vertices.Length];
        var vertx = new Vector3[mesh.vertices.Length];
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            var vertex = mesh.vertices[i];
            var x = Mathf.RoundToInt((vertex.x + offset.x) * ratio) + (width / 2) + 1;
            var z = Mathf.RoundToInt((vertex.z + offset.z) * ratio) + (width / 2) + 1;
            var y = vertex.y;
            Debug.Log("ModifyTileWithHeightMap : x = " + x + ", y = " + z);
            var evaluatedNoise = noise[x, z] * heightCurve.Evaluate(y / 1f); 
            vertx[i] = mesh.vertices[i] + (evaluatedNoise * maxHeight - (maxHeight * .5f * heightCurve.Evaluate(0f))) * Vector3.up ;
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

    public void ModifyMeshWithHeightMap(List<TileHolder> tileHolders, float[,] noise, float maxHeight, AnimationCurve heightCurve)
    {
        float width;
        var farestX = 0f;
        var farestZ = 0f;
        var closestX = 0f;
        var closestZ = 0f;


        foreach (var holder in tileHolders)
        {
            if (holder.transform.position.x > farestX) farestX = holder.transform.position.x;
            if (holder.transform.position.x < closestX) closestX = holder.transform.position.x;
            if (holder.transform.position.z > farestZ) farestZ = holder.transform.position.z;
            if (holder.transform.position.z < closestZ) closestZ = holder.transform.position.z;
        }

        width = (farestX - closestX) > (farestZ - closestZ) ? (farestX - closestX) : (farestZ - closestZ);



        foreach (var holder in tileHolders)
        {
            holder.GetComponent<MeshFilter>().mesh = ModifyTileWithHeightMap(holder, noise, maxHeight, width, heightCurve);
        }
    }
}

