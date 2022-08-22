using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridGenerator;

namespace WCF
{
    public class WCF : MonoBehaviour
    {
        //List<Tile> tiles = new List<Tile>();
        [SerializeField]
        TileAsset asset = null;

        [SerializeField]
        Tile[] gridTiled;
        v3Quad[] quadTiled;
        [SerializeField]
        List<Tile>[] availableTiles;
        [SerializeField]
        private Tile tilesTest;
        [SerializeField]
        private GameObject home;

        public List<Tile> Tiles { get => asset.Tiles;}

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
                foreach(Tile tile in asset.Tiles)
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

            var nbPlain = 1;
            var maxCellPlain = 5;
            var it = 0;
            while (it < nbPlain)
            {
                var rnd = UnityEngine.Random.Range(0, grid.Length - 1);
                var pickedQuad = grid[rnd];
                if (!treated.Contains(rnd))
                {
                    var newTile = new Tile(asset.Tiles[0]);
                    newTile.name = asset.Tiles[0].name + " - dungeon";
                    AddTileToGrid(grid, treated, rnd, newTile);
                }
                var homeObj = Instantiate(home, pickedQuad.Position, Quaternion.identity, transform);

                //var neighbourList = new List<Neighbour>();
                var neighbourList = new List<v3Quad>();
                foreach(var neigh in pickedQuad.Neighbours)
                    neighbourList.Add((v3Quad)neigh.neighbour);
                foreach(var neigh in pickedQuad.CrossNeighbours)
                    neighbourList.Add((v3Quad)neigh.neighbour);

                foreach(var quad in neighbourList)
                {
                    int id = FindIndexOfQuad(quad, grid);
                    if (!treated.Contains(id) && pickedQuad.GetCommonPoints(pickedQuad, quad).Count > 0)
                    {
                        var newTile = new Tile(asset.Tiles[0]);
                        newTile.name = asset.Tiles[0].name + " - dungeon";
                        AddTileToGrid(grid, treated, id, newTile);
                    }

                }
                /*var crossNeighbourList = new List<CrossNeighbour>();
                crossNeighbourList.AddRange(pickedQuad.CrossNeighbours);
                foreach (CrossNeighbour neigh in crossNeighbourList)
                {
                    int id = FindIndexOfQuad((v3Quad)neigh.neighbour, grid);
                    if (!treated.Contains(id) && pickedQuad.GetCommonPoints(pickedQuad, neigh.neighbour).Count > 0)
                        AddTileToGrid(grid, treated, id, new Tile(asset.Tiles[0]));

                }*/

                it++;
            }

            while(treated.Count < grid.Length && !retry)
            {
                // Get the list of cells where the entropy is the lowest
                List<v3Quad> lowestEntropy = GetLowestEntropyCells(treated, grid, out int lowestEntropyNb);
                if (lowestEntropy.Count == 0 || lowestEntropyNb == 0) retry = true;
                else
                {
                    var rndCell = UnityEngine.Random.Range(0, lowestEntropy.Count);
                    var chosenQuad = lowestEntropy[rndCell];
                    retry = PickTileToAdd(grid, treated, chosenQuad);
                    yield return null;
                }

            }
            if (retry) StartCoroutine(StartWave(grid, mapMaterial));
            else
                for(int i = 0; i < gridTiled.Length; i++)
                {
                    // generate mesh
                    var go = new GameObject("quad - " + gridTiled[i].name, typeof(MeshFilter), typeof(MeshRenderer), typeof(TileHolder), typeof(MeshCollider));
                    go.transform.parent = transform;
                    var filter = go.GetComponent<MeshFilter>();
                    var rend = go.GetComponent<MeshRenderer>();
                    var col = go.GetComponent<MeshCollider>();
                    go.GetComponent<TileHolder>().tile = gridTiled[i];
                    go.GetComponent<TileHolder>().quad = quadTiled[i];
                    rend.material = mapMaterial;
                    var modifiedMesh = meshModifier.ModifyMesh(grid[i].pts, gridTiled[i].mesh);
                    filter.mesh = modifiedMesh;
                    col.sharedMesh = modifiedMesh;
                    yield return null;
                }
            Debug.Log("End Time = " + Time.time);
        }

        private bool PickTileToAdd(v3Quad[] grid, List<int> treated, v3Quad chosenQuad)
        {
            // We pick a tile from the available list of the chosen quad
            int i = FindIndexOfQuad(chosenQuad, grid);
            var rndTile = UnityEngine.Random.Range(0, availableTiles[i].Count);
            var pickedTile = availableTiles[i][rndTile];

            var retry = AddTileToGrid(grid, treated, i, pickedTile);
            return retry;
        }

        private bool AddTileToGrid(v3Quad[] grid, List<int> treated, int quadIndex, Tile pickedTile)
        {
            if (pickedTile.name.Contains("High"))
                Debug.Log("high tile picked");
            var retry = false;
            var chosenQuad = grid[quadIndex];
            var neighbours = chosenQuad.Neighbours;
            var crossNeighbours = chosenQuad.CrossNeighbours;
            for (int x = 0; x < neighbours.Count; x++)
            {
                int j = FindIndexOfQuad((v3Quad)neighbours[x].neighbour, grid);
                if (gridTiled[j] == null)
                {
                    RemoveUnavailableTiles(pickedTile, neighbours, x, j);
                    if (availableTiles[j].Count == 0)
                    {
                        retry = true;
                    }
                }
            }
            for (int x = 0; x < crossNeighbours.Count; x++)
            {
                int j = FindIndexOfQuad((v3Quad)crossNeighbours[x].neighbour, grid);
                if (gridTiled[j] == null)
                {
                    RemoveUnavailableTilesFromCross(pickedTile, crossNeighbours, x, j);
                    if (availableTiles[j].Count == 0)
                    {
                        retry = true;
                    }
                }
            }
            availableTiles[quadIndex].Clear();
            gridTiled[quadIndex] = pickedTile;
            quadTiled[quadIndex] = chosenQuad;
            treated.Add(quadIndex);
            return retry;
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
        private void RemoveUnavailableTilesFromCross(Tile pickedTile, List<CrossNeighbour> neighbours, int neighIndex, int availablesIndex)
        {
            var availableTileList = new List<Tile>();
            availableTileList.AddRange(availableTiles[availablesIndex]);

            foreach (Tile tile in availableTileList)
            {
                if (!Tile.CheckIfTileConnectCross(pickedTile, tile, neighbours[neighIndex].ptIndex, neighbours[neighIndex].neighbourPtIndex))
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

        private List<v3Quad> GetLowestEntropyCells(List<int> treated, v3Quad[] grid, out int lowestEntropy)
        {
            lowestEntropy = asset.Tiles.Count * 4;
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
