using GridGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WCF;

public class TileHolder : MonoBehaviour
{
    public Tile tile;
    public v3Quad quad;

    private void OnDrawGizmos()
    {
        var basecolor = Color.blue;

        var quad0Pos = quad.pts[0] + (quad.pts[2] - quad.pts[0]).normalized + Vector3.up;
        var quad1Pos = quad.pts[1] + (quad.pts[3] - quad.pts[1]).normalized + Vector3.up;
        var quad2Pos = quad.pts[2] + (quad.pts[0] - quad.pts[2]).normalized + Vector3.up;
        var quad3Pos = quad.pts[3] + (quad.pts[1] - quad.pts[3]).normalized + Vector3.up;

        Gizmos.color = Color.Lerp(basecolor, Color.white, 1f);
        Gizmos.DrawSphere(quad0Pos, .1f);
        Gizmos.color = basecolor;
        foreach(Neighbour neigh in quad.Neighbours)
            if(neigh.edge == 0) Gizmos.color = Color.red;
        Gizmos.DrawLine(quad0Pos, quad1Pos);

        Gizmos.color = Color.Lerp(basecolor, Color.white, 0.66f);
        Gizmos.DrawSphere(quad1Pos, .1f);
        Gizmos.color = basecolor;
        foreach(Neighbour neigh in quad.Neighbours)
            if(neigh.edge == 1) Gizmos.color = Color.red;
        Gizmos.DrawLine(quad1Pos, quad2Pos);

        Gizmos.color = Color.Lerp(basecolor, Color.white, 0.33f);
        Gizmos.DrawSphere(quad2Pos, .1f);
        Gizmos.color = basecolor;
        foreach(Neighbour neigh in quad.Neighbours)
            if(neigh.edge == 2) Gizmos.color = Color.red;
        Gizmos.DrawLine(quad2Pos, quad3Pos);

        Gizmos.color = Color.Lerp(basecolor, Color.white, 0.0f);
        Gizmos.DrawSphere(quad3Pos, .1f);
        Gizmos.color = basecolor;
        foreach(Neighbour neigh in quad.Neighbours)
            if(neigh.edge == 3) Gizmos.color = Color.red;
        Gizmos.DrawLine(quad0Pos, quad3Pos);

        /*if (edgesIndex[0] == 0 && edgesIndex[1] == 1
            || edgesIndex[1] == 0 && edgesIndex[0] == 1)
            // 0 up
            return 0;
        else if (edgesIndex[0] == 1 && edgesIndex[1] == 2
            || edgesIndex[1] == 1 && edgesIndex[0] == 2)
            // 3 left
            return 3;
        else if (edgesIndex[0] == 2 && edgesIndex[1] == 3
            || edgesIndex[1] == 2 && edgesIndex[0] == 3)
            // 2 back
            return 2;
        else if (edgesIndex[0] == 3 && edgesIndex[1] == 0
            || edgesIndex[1] == 3 && edgesIndex[0] == 0)
            // 1 right
            return 1;*/
    }
}
