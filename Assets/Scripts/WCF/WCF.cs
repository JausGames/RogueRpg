using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridGenerator;

namespace WCF
{
    public class WCF : MonoBehaviour
    {
        [SerializeField]
        List<Tile> tiles = new List<Tile>();

        [SerializeField]
        Tile[] gridTiled;
        v3Quad[] quadTiled;
        [SerializeField]
        List<Tile>[] availableTiles;

        public List<Tile> Tiles { get => tiles; set => tiles = value; }

        public IEnumerator StartWave(v3Quad[] grid, Material mapMaterial)
        {
            gridTiled = new Tile[grid.Length];
            quadTiled = new v3Quad[grid.Length];
            availableTiles = new List<Tile>[grid.Length];
            var treated = new List<int>();

            for (int i = 0; i < availableTiles.Length; i++)
            {
                    availableTiles[i] = new List<Tile>();
                //Add all tiles to the available tile for each cell of the grid
                foreach(Tile tile in tiles)
                {
                    availableTiles[i].Add(new Tile(tile));
                    availableTiles[i][availableTiles[i].Count - 1].name = tile.name;
                    availableTiles[i].Add(new Tile(tile, 1));
                    availableTiles[i][availableTiles[i].Count - 1].name = tile.name + " - rot 90";
                    availableTiles[i].Add(new Tile(tile, 2));
                    availableTiles[i][availableTiles[i].Count - 1].name = tile.name + " - rot 180";
                    availableTiles[i].Add(new Tile(tile, 3));
                    availableTiles[i][availableTiles[i].Count - 1].name = tile.name + " - rot 270";
                }
            }
            var meshModifier = new MeshModifier();
            var retry = false;
            while(treated.Count < grid.Length && !retry)
            {
                // Get the list of cells where the entropy is the lowest
                List<v3Quad> lowestEntropy = GetLowestEntropyCells(treated, grid);
                if (lowestEntropy.Count == 0) throw (new Exception("lowest entropy array empty"));
                var rndCell = UnityEngine.Random.Range(0, lowestEntropy.Count);
                var chosenQuad = lowestEntropy[rndCell];



                // We pick a tile from the available list of the chosen quad
                int i = FindIndexOfQuad(chosenQuad, grid);
                var rndTile = UnityEngine.Random.Range(0, availableTiles[i].Count);
                var pickedTile = availableTiles[i][rndTile];

                var neighbours = chosenQuad.Neighbours;
                for(int x = 0; x < neighbours.Count; x++)
                {
                    int j = FindIndexOfQuad((v3Quad)neighbours[x].neighbour, grid);
                    if(gridTiled[j] == null)
                    {
                        RemoveUnavailableTiles(pickedTile, neighbours, x, j);
                        if (availableTiles[j].Count == 0)
                        {
                            retry = true;
                        }
                    }
                }
                availableTiles[i].Clear();
                gridTiled[i] = pickedTile;
                quadTiled[i] = lowestEntropy[rndCell];
                treated.Add(i);
                yield return null;

            }
            if (retry) StartCoroutine(StartWave(grid, mapMaterial));
            else
                for(int i = 0; i < gridTiled.Length; i++)
                {
                    // generate mesh
                    var go = new GameObject("quad - " + gridTiled[i].name, typeof(MeshFilter), typeof(MeshRenderer), typeof(TileHolder));
                    go.transform.parent = transform;
                    var filter = go.GetComponent<MeshFilter>();
                    var rend = go.GetComponent<MeshRenderer>();
                    go.GetComponent<TileHolder>().tile = gridTiled[i];
                    go.GetComponent<TileHolder>().quad = quadTiled[i];
                    rend.material = mapMaterial;
                    filter.mesh = meshModifier.ModifyMesh(grid[i].pts, gridTiled[i].mesh);
                    yield return null;
                }

            /*for (int i = 0; i < gridTiled.Length; i++)
            {
                var go = new GameObject("quad", typeof(MeshFilter), typeof(MeshRenderer));
                var filter = go.GetComponent<MeshFilter>();
                var rend = go.GetComponent<MeshRenderer>();
                rend.material = mapMaterial;
                filter.mesh = meshModifier.ModifyMesh(grid[i].pts, gridTiled[i].mesh);
            }*/
            Debug.Log("End Time = " + Time.time);
        }

        private void RemoveUnavailableTiles(Tile pickedTile, List<Neighbour> neighbours, int neighIndex, int availablesIndex)
        {
            var availableTileList = new List<Tile>();
            availableTileList.AddRange(availableTiles[availablesIndex]);
            foreach (Tile tile in availableTileList)
            {
                if (
                    //If not in whitelist : Remove
                    pickedTile.connectors[neighbours[neighIndex].edge].WhiteList.Count > 0 && !pickedTile.connectors[neighbours[neighIndex].edge].WhiteList.Contains(tile.id)
                    //If in blacklist : Remove
                    || pickedTile.connectors[neighbours[neighIndex].edge].BlackList.Contains(tile.id)
                    //If can't connect : Remove
                    || !Tile.CheckIfTileConnect(pickedTile, tile, neighbours[neighIndex].edge, neighbours[neighIndex].oppositeEdge)
                    )
                    availableTiles[availablesIndex].Remove(tile);

            }
        }

        private int GetOppositeEdge(int edge)
        {
            if (edge == 0) return 2;
            else if (edge == 1) return 3;
            else if (edge == 2) return 1;
            else if (edge == 3) return 0;
            return -1;
        }

        private int FindIndexOfQuad(v3Quad v3Quad, v3Quad[] grid)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                if (v3Quad == grid[i]) return i;
            }
            return -1;
        }

        private List<v3Quad> GetLowestEntropyCells(List<int> treated, v3Quad[] grid)
        {
            int lowestEntropy = tiles.Count * 4;
            for(int i = 0; i < grid.Length; i++)
            {
                if (!treated.Contains(i) && availableTiles[i].Count < lowestEntropy && availableTiles[i].Count != 0) lowestEntropy = availableTiles[i].Count;
            }
            List<v3Quad> result = new List<v3Quad>();
            for (int i = 0; i < grid.Length; i++)
            {
                if (!treated.Contains(i) && availableTiles[i].Count == lowestEntropy) result.Add(grid[i]);
            }
            return result;
        }
    }

}
