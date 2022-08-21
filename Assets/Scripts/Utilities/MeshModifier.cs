using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshModifier
{
    public Mesh mesh;
    public MeshFilter filter;

    public Vector3[] newVerticies;
    private float height = 250f;

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

    public Mesh ModifyMesh(Vector3[] irregularCell, Mesh mesh)
    {
        var modMesh = new Mesh();
        newVerticies = ModifyMesh(irregularCell, mesh.vertices);
        modMesh.vertices = newVerticies;
        modMesh.triangles = mesh.triangles;
        modMesh.RecalculateNormals();
        modMesh.RecalculateBounds();
        modMesh.RecalculateTangents();
        return modMesh;
    }
    public Vector3[] ModifyMesh(Vector3[] irregularCell, Vector3[] points)
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
            newVerticies[i] = P;
        }

        return newVerticies;
    }

}

