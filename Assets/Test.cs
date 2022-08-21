using GridGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WCF;

public class Test : MonoBehaviour
{
    private MeshModifier modifier;

    public Vector3[] pointsA = new Vector3[4];
    public Vector3[] pointsB = new Vector3[4];
    public Vector3[] pointsC = new Vector3[4];
    public Vector3[] pointsD = new Vector3[4];
    public Tile tile;
    public Material mat;
    public MeshFilter filterA;
    public MeshFilter filterB;
    public MeshFilter filterC;
    public MeshFilter filterD;

    public WCF.WCF wcf;
    // Start is called before the first frame update
    void Start()
    {
        /*var modMesh = new Mesh();
         var newVerticies = modifier.ModifyMesh(pointsA, tile.mesh);
         modMesh.vertices = newVerticies.vertices;
         modMesh.triangles = tile.mesh.triangles;
         modMesh.RecalculateNormals();
         modMesh.RecalculateBounds();
         modMesh.RecalculateTangents();
         filterA.mesh = modMesh;

         var newTile = new Tile(tile, 1);

         modMesh = new Mesh();
         newVerticies = modifier.ModifyMesh(pointsB, newTile.mesh);
         modMesh.vertices = newVerticies.vertices;
         modMesh.triangles = tile.mesh.triangles;
         modMesh.RecalculateNormals();
         modMesh.RecalculateBounds();
         modMesh.RecalculateTangents();
         filterB.mesh = modMesh;

         newTile = new Tile(tile, 2);
         modMesh = new Mesh();
         newVerticies = modifier.ModifyMesh(pointsC, newTile.mesh);
         modMesh.vertices = newVerticies.vertices;
         modMesh.triangles = tile.mesh.triangles;
         modMesh.RecalculateNormals();
         modMesh.RecalculateBounds();
         modMesh.RecalculateTangents();
         filterC.mesh = modMesh;*/

        var width = 20;
        var height = 20;
        var size = 5f;

        //var quads = new v3Quad[width*height];
        var quads = new List<v3Quad>();
        for (int i = 0; i < width ; i++)
            for (int j = 0; j < height; j++)
            {
                var a = new Vector3(i, 0, j) * size;
                var b = new Vector3(i + 1, 0, j) * size;
                var c = new Vector3(i + 1, 0, j + 1) * size;
                var d = new Vector3(i, 0, j + 1) * size;
                quads.Add( new v3Quad(new Vector3[] {a,b,c,d }) );
            }
        /*quads[0] = new v3Quad(pointsA);
        quads[1] = new v3Quad(pointsB);
        quads[2] = new v3Quad(pointsC);
        quads[3] = new v3Quad(pointsD);*/
        for (int i = 1; i < quads.Count; i++)
            quads[i].FindNeighbours(quads.ToArray());
            

       /*         Debug.Log("quad = " + 0);
        Debug.Log("quad = " + 2);
        quads[1].FindNeighbours(quads);
        Debug.Log("quad = " + 3);
        quads[2].FindNeighbours(quads);
        Debug.Log("quad = " + 4);
        quads[3].FindNeighbours(quads);

        var test01 = Tile.CheckIfTileConnect(tiles[2], tiles[0], 0);
        var test02 = Tile.CheckIfTileConnect(tiles[2], tiles[0], 1);
        var test03 = Tile.CheckIfTileConnect(tiles[2], tiles[0], 2);
        var test04 = Tile.CheckIfTileConnect(tiles[2], tiles[0], 3);

        var test05 = Tile.CheckIfTileConnect(tiles[2], tiles[1], 0);
        var test06 = Tile.CheckIfTileConnect(tiles[2], tiles[1], 1);
        var test07 = Tile.CheckIfTileConnect(tiles[2], tiles[1], 2);
        var test08 = Tile.CheckIfTileConnect(tiles[2], tiles[1], 3);

        var test09 = Tile.CheckIfTileConnect(tiles[2], tiles[2], 0);
        var test10 = Tile.CheckIfTileConnect(tiles[2], tiles[2], 1);
        var test11 = Tile.CheckIfTileConnect(tiles[2], tiles[2], 2);
        var test12 = Tile.CheckIfTileConnect(tiles[2], tiles[2], 3);*/
        //wcf.Tiles = tiles;

        StartCoroutine(wcf.StartWave(quads.ToArray(), mat));

        var tileHolder = FindObjectsOfType<TileHolder>();

    }
    [SerializeField]
    List<Tile> tiles = new List<Tile>();


    [ExecuteInEditMode]
    void DoIt() {

        var modMesh = new Mesh();
        var newVerticies = modifier.ModifyMesh(pointsA, tile.mesh);
        modMesh.vertices = newVerticies.vertices;
        modMesh.triangles = tile.mesh.triangles;
        modMesh.RecalculateNormals();
        modMesh.RecalculateBounds();
        modMesh.RecalculateTangents();
        filterA.mesh = modMesh;

        var newTile = new Tile(tile, 1);

        modMesh = new Mesh();
        newVerticies = modifier.ModifyMesh(pointsB, newTile.mesh);
        modMesh.vertices = newVerticies.vertices;
        modMesh.triangles = tile.mesh.triangles;
        modMesh.RecalculateNormals();
        modMesh.RecalculateBounds();
        modMesh.RecalculateTangents();
        filterB.mesh = modMesh;

        newTile = new Tile(tile, 2);
        modMesh = new Mesh();
        newVerticies = modifier.ModifyMesh(pointsC, newTile.mesh);
        modMesh.vertices = newVerticies.vertices;
        modMesh.triangles = tile.mesh.triangles;
        modMesh.RecalculateNormals();
        modMesh.RecalculateBounds();
        modMesh.RecalculateTangents();
        filterC.mesh = modMesh;
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var pt in pointsA)
            Gizmos.DrawWireSphere(pt, .1f);

        Gizmos.color = Color.red;
        foreach (var pt in pointsB)
            Gizmos.DrawWireSphere(pt, .1f);

        Gizmos.color = Color.green;
        foreach (var pt in pointsC)
            Gizmos.DrawWireSphere(pt, .1f);
    }
}
