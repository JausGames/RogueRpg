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
    public GameObject[] meshHolders = new GameObject[2];
    private Color[] borderColors = new Color[] { Color.white, Color.Lerp(Color.yellow, Color.red, 0.5f), Color.red, Color.blue };

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
        var points = new Vector3[] { quad0Pos, quad1Pos, quad2Pos, quad3Pos };

        //Debug edge nb
        //Gizmos.color = Color.Lerp(basecolor, Color.white, 1f);

        Gizmos.color = basecolor;
        Gizmos.DrawSphere(quad0Pos, .1f);

        for(int i = 0; i < 4; i++)
        {

                //Debug connectors
                Gizmos.color = borderColors[(int)tile.connectors[i].connection[0]];
                Gizmos.DrawLine(points[(i + 1) % 4], (2 * points[(i + 1) % 4] + 1 * points[i]) / 3f);


                Gizmos.color = borderColors[(int)tile.connectors[i].connection[1]];
                Gizmos.DrawLine((2 * points[(i + 1) % 4] + 1 * points[i]) / 3f, (1 * points[(i + 1) % 4] + 2 * points[i]) / 3f);


                Gizmos.color = borderColors[(int)tile.connectors[i].connection[2]];
                Gizmos.DrawLine((1 * points[(i + 1) % 4] + 2 * points[i]) / 3f, points[i]);
 /*           for (int j = 0; j < 3; j++)
            {
                //Debug connectors
                Gizmos.color = borderColors[(int)tile.connectors[i].connection[j]];
                Gizmos.DrawLine((points[(j + 1) % 4] * ((j-3)%4) + j *points[i])/3, (((j - 2) % 4) * points[(j  + 1) % 4] + (j+1) * points[i]) / 3f);
            }*/
        }
/*
        //Debug connectors
        Gizmos.color = borderColors[(int)tile.connectors[0].connection[0]];
        Gizmos.DrawLine(quad0Pos, (2 * quad0Pos + 1*  quad1Pos) / 3f);

        Gizmos.color = borderColors[(int)tile.connectors[0].connection[1]];
        Gizmos.DrawLine((2 * quad0Pos + 1 * quad1Pos) / 3f, (1 * quad0Pos + 2*  quad1Pos) / 3f);


        Gizmos.color = borderColors[(int)tile.connectors[0].connection[2]];
        Gizmos.DrawLine((1 * quad0Pos + 2 * quad1Pos) / 3f, quad1Pos);

        //check neigbbours
        *//*foreach(Neighbour neigh in quad.Neighbours)
            if(neigh.edge == 0) Gizmos.color = Color.red;*//*

        //draw single line
        //Gizmos.DrawLine(quad0Pos, quad1Pos);

        Gizmos.color = Color.Lerp(basecolor, Color.white, 0.66f);
        Gizmos.DrawSphere(quad1Pos, .1f);
        //Debug connectors
        Gizmos.color = borderColors[(int)tile.connectors[1].connection[0]];
        Gizmos.DrawLine(quad1Pos, (2 * quad1Pos + 1 * quad2Pos) / 3f);

        Gizmos.color = borderColors[(int)tile.connectors[1].connection[1]];
        Gizmos.DrawLine((2 * quad1Pos + 1 * quad2Pos) / 3f, (1 * quad1Pos + 2 * quad2Pos) / 3f);

        Gizmos.color = borderColors[(int)tile.connectors[1].connection[2]];
        Gizmos.DrawLine((1 * quad1Pos + 2 * quad2Pos) / 3f, quad2Pos);

        *//*foreach(Neighbour neigh in quad.Neighbours)
            if(neigh.edge == 1) Gizmos.color = Color.red;*//*
        //Gizmos.DrawLine(quad1Pos, quad2Pos);

        //Gizmos.color = Color.Lerp(basecolor, Color.white, 0.33f);
        Gizmos.DrawSphere(quad2Pos, .1f);

        //Debug connectors
        Gizmos.color = borderColors[(int)tile.connectors[2].connection[0]];
        Gizmos.DrawLine(quad2Pos, (2 * quad2Pos + 1 * quad3Pos) / 3f);


        Gizmos.color = borderColors[(int)tile.connectors[2].connection[1]];
        Gizmos.DrawLine((2 * quad2Pos + 1 * quad3Pos) / 3f, (1 * quad2Pos + 2 * quad3Pos) / 3f);


        Gizmos.color = borderColors[(int)tile.connectors[2].connection[2]];
        Gizmos.DrawLine((1 * quad2Pos + 2 * quad3Pos) / 3f, quad3Pos);
        
        *//*foreach(Neighbour neigh in quad.Neighbours)
            if(neigh.edge == 2) Gizmos.color = Color.red;*//*
        //Gizmos.DrawLine(quad2Pos, quad3Pos);

        //Gizmos.color = Color.Lerp(basecolor, Color.white, 0.0f);
        Gizmos.DrawSphere(quad3Pos, .1f);

        Gizmos.color = borderColors[(int)tile.connectors[3].connection[0]];
        Gizmos.DrawLine(quad3Pos, (2 * quad3Pos + 1 * quad0Pos) / 3f);

        Gizmos.color = borderColors[(int)tile.connectors[3].connection[1]];
        Gizmos.DrawLine((2 * quad3Pos + 1 * quad0Pos) / 3f, (1 * quad3Pos + 2 * quad0Pos) / 3f);
        
        Gizmos.color = basecolor;
        Gizmos.color = borderColors[(int)tile.connectors[3].connection[2]];
        Gizmos.DrawLine((1 * quad3Pos + 2 * quad0Pos) / 3f, quad0Pos);
        */
        /*foreach(Neighbour neigh in quad.Neighbours)
            if(neigh.edge == 3) Gizmos.color = Color.red;*/
        //Gizmos.DrawLine(quad0Pos, quad3Pos);

        Gizmos.color = Color.yellow;
        if(center)Gizmos.DrawSphere(center.transform.position, .5f);

    }
}
