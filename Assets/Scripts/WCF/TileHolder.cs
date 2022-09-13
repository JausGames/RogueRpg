using GridGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WCF;

public class TileHolder : MonoBehaviour
{
    private Tile tile;
    private v3Quad quad;
    private GameObject center;

    public v3Quad Quad 
    { 
        get => quad;
        set
        {
            quad = value;
            Destroy(center);
            center = new GameObject("center");
            center.transform.position = quad.Position;
            center.transform.parent = transform;
        }
    }
    public Tile Tile { get => tile; set => tile = value; }

    private void OnDrawGizmos()
    {
        var basecolor = Color.blue;

        var quad0Pos = quad.pts[0] + (quad.pts[2] - quad.pts[0]).normalized + Vector3.up;
        var quad1Pos = quad.pts[1] + (quad.pts[3] - quad.pts[1]).normalized + Vector3.up;
        var quad2Pos = quad.pts[2] + (quad.pts[0] - quad.pts[2]).normalized + Vector3.up;
        var quad3Pos = quad.pts[3] + (quad.pts[1] - quad.pts[3]).normalized + Vector3.up;

        //Debug edge nb
        //Gizmos.color = Color.Lerp(basecolor, Color.white, 1f);

        Gizmos.color = basecolor;
        Gizmos.DrawSphere(quad0Pos, .1f);

        //Debug connectors
        if (tile.connectors[0].connection[0] == Connection.Border) Gizmos.color = Color.red;
        Gizmos.DrawLine(quad0Pos, (2 * quad0Pos + 1*  quad1Pos) / 3f);
        

        Gizmos.color = basecolor;
        if (tile.connectors[0].connection[1] == Connection.Border) Gizmos.color = Color.red;
        Gizmos.DrawLine((2 * quad0Pos + 1 * quad1Pos) / 3f, (1 * quad0Pos + 2*  quad1Pos) / 3f);
        

        Gizmos.color = basecolor;
        if (tile.connectors[0].connection[2] == Connection.Border) Gizmos.color = Color.red;
        Gizmos.DrawLine((1 * quad0Pos + 2 * quad1Pos) / 3f, quad1Pos);

        //check neigbbours
        /*foreach(Neighbour neigh in quad.Neighbours)
            if(neigh.edge == 0) Gizmos.color = Color.red;*/

        //draw single line
        //Gizmos.DrawLine(quad0Pos, quad1Pos);

        Gizmos.color = Color.Lerp(basecolor, Color.white, 0.66f);
        Gizmos.DrawSphere(quad1Pos, .1f);
        //Debug connectors
        Gizmos.color = basecolor;
        if (tile.connectors[1].connection[0] == Connection.Border) Gizmos.color = Color.red;
        Gizmos.DrawLine(quad1Pos, (2 * quad1Pos + 1 * quad2Pos) / 3f);

        Gizmos.color = basecolor;
        if (tile.connectors[1].connection[1] == Connection.Border) Gizmos.color = Color.red;
        Gizmos.DrawLine((2 * quad1Pos + 1 * quad2Pos) / 3f, (1 * quad1Pos + 2 * quad2Pos) / 3f);

        Gizmos.color = basecolor;
        if (tile.connectors[1].connection[2] == Connection.Border) Gizmos.color = Color.red;
        Gizmos.DrawLine((1 * quad1Pos + 2 * quad2Pos) / 3f, quad2Pos);

        /*foreach(Neighbour neigh in quad.Neighbours)
            if(neigh.edge == 1) Gizmos.color = Color.red;*/
        //Gizmos.DrawLine(quad1Pos, quad2Pos);

        //Gizmos.color = Color.Lerp(basecolor, Color.white, 0.33f);
        Gizmos.DrawSphere(quad2Pos, .1f);

        //Debug connectors
        Gizmos.color = basecolor;
        if (tile.connectors[2].connection[0] == Connection.Border) Gizmos.color = Color.red;
        Gizmos.DrawLine(quad2Pos, (2 * quad2Pos + 1 * quad3Pos) / 3f);
        

        Gizmos.color = basecolor;
        if (tile.connectors[2].connection[1] == Connection.Border) Gizmos.color = Color.red;
        Gizmos.DrawLine((2 * quad2Pos + 1 * quad3Pos) / 3f, (1 * quad2Pos + 2 * quad3Pos) / 3f);
        

        Gizmos.color = basecolor;
        if (tile.connectors[2].connection[2] == Connection.Border) Gizmos.color = Color.red;
        Gizmos.DrawLine((1 * quad2Pos + 2 * quad3Pos) / 3f, quad3Pos);
        
        /*foreach(Neighbour neigh in quad.Neighbours)
            if(neigh.edge == 2) Gizmos.color = Color.red;*/
        //Gizmos.DrawLine(quad2Pos, quad3Pos);

        //Gizmos.color = Color.Lerp(basecolor, Color.white, 0.0f);
        Gizmos.DrawSphere(quad3Pos, .1f);

        Gizmos.color = basecolor;
        if (tile.connectors[3].connection[0] == Connection.Border) Gizmos.color = Color.red;
        Gizmos.DrawLine(quad3Pos, (2 * quad3Pos + 1 * quad0Pos) / 3f);

        Gizmos.color = basecolor;
        if (tile.connectors[3].connection[1] == Connection.Border) Gizmos.color = Color.red;
        Gizmos.DrawLine((2 * quad3Pos + 1 * quad0Pos) / 3f, (1 * quad3Pos + 2 * quad0Pos) / 3f);
        
        Gizmos.color = basecolor;
        if (tile.connectors[3].connection[2] == Connection.Border) Gizmos.color = Color.red;
        Gizmos.DrawLine((1 * quad3Pos + 2 * quad0Pos) / 3f, quad0Pos);
        
        /*foreach(Neighbour neigh in quad.Neighbours)
            if(neigh.edge == 3) Gizmos.color = Color.red;*/
        //Gizmos.DrawLine(quad0Pos, quad3Pos);

        Gizmos.color = Color.yellow;
        if(center)Gizmos.DrawSphere(center.transform.position, .5f);

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
