using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDrawer : MonoBehaviour
{
    static public void DrawTriangle(Vector3 A, Vector3 B, Vector3 C, Color color = new Color())
    {
        DrawTriangle(new Vector3[] { A, B, C }, color);
    }

    static public void DrawTriangle(List<Vector3> points, Color color = new Color())
    {
        DrawTriangle(points.ToArray(), color);
    }
    static public void DrawTriangle(Vector3[] points, Color color = new Color())
    {
        DrawLine(points[0] - Vector3.up, points[1] - Vector3.up, color);
        DrawLine(points[1] - Vector3.up, points[2] - Vector3.up, color);
        DrawLine(points[2] - Vector3.up, points[0] - Vector3.up, color);
    }

    static public void DrawQuad(Vector3 A, Vector3 B, Vector3 C, Vector3 D, Color color = new Color())
    {
        DrawQuad(new Vector3[] { A, B, C, D }, color);
    }

    static public void DrawQuad(List<Vector3> points, Color color = new Color())
    {
        DrawQuad(points.ToArray(), color);
    }
    static public void DrawQuad(Vector3[] points, Color color = new Color())
    {
        DrawLine(points[0], points[1], color);
        DrawLine(points[1], points[2], color);
        DrawLine(points[2], points[3], color);
        DrawLine(points[3], points[0], color);
    }
    static public void DrawPolygon(Vector3[] points, Color color = new Color(), GameObject go = null)
    {
        for (int i = 0; i < points.Length; i++)
            Debug.DrawLine(points[i], points[(i + 1) % points.Length], color, Mathf.Infinity, true);
        //DrawLine(points[i], points[(i+1) % points.Length], color);
        //StartCoroutine(Polygons(points, color));
    }

    static IEnumerator Polygons(Vector3[] points, Color color = new Color())
    {
        for (int i = 0; i < points.Length; i++)
        {
            DrawLine(points[i], points[(i + 1) % points.Length], color);
            yield return new WaitForSeconds(.1f);
        }
    }

    static public void DrawLine(Vector3 A, Vector3 B, Color color = new Color())
    {
        if (color == new Color()) color = Color.white;
        Debug.DrawLine(A, B, color, Mathf.Infinity, false);
    }
}
