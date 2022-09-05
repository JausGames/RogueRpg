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
        private Material mapMaterial;
        [SerializeField]
        Tile[] gridTiled;
        bool[] hasBackUpTile;
        v3Quad[] quadTiled;
        [SerializeField]
        List<Tile>[] availableTiles;
        List<Tile>[] backupTiles;
        [SerializeField]
        private Tile tilesTest;
        [SerializeField]
        private GameObject home;
        private List<GameObject> tiles = new List<GameObject>();
        private List<TileHolder> tileHolders = new List<TileHolder>();

        [SerializeField]
        NoiseSettings noiseSettings;
        [SerializeField]
        AnimationCurve heightCurve;

        public List<Tile> Tiles { get => asset.Tiles;}
        private void Awake()
        {
            
        }

        public IEnumerator StartWave(v3Quad[] grid, Material mapMaterial)
        {
            this.mapMaterial = mapMaterial;
            gridTiled = new Tile[grid.Length];
            quadTiled = new v3Quad[grid.Length];
            hasBackUpTile = new bool[grid.Length];
            availableTiles = new List<Tile>[grid.Length];
            backupTiles = new List<Tile>[grid.Length];
            var treated = new List<int>();

            //Set up available tiles
            for (int i = 0; i < availableTiles.Length; i++)
            {
                hasBackUpTile[i] = false;
                availableTiles[i] = new List<Tile>();
                backupTiles[i] = new List<Tile>();
                //Add all tiles to the available tile for each cell of the grid
                foreach (Tile tile in asset.Tiles)
                {
                    availableTiles[i].Add(new Tile(tile));
                    availableTiles[i][availableTiles[i].Count - 1].name = tile.name;
                    if (tile.symetry != Symetry.Full)
                    {
                        availableTiles[i].Add(new Tile(tile, 1));
                        availableTiles[i][availableTiles[i].Count - 1].name = tile.name + " - rot 90";
                    }
                    else
                    {
                    availableTiles[i].Add(new Tile(tile));
                    availableTiles[i][availableTiles[i].Count - 1].name = tile.name;

                    }
                    if (tile.symetry == Symetry.None)
                    {
                        availableTiles[i].Add(new Tile(tile, 2));
                        availableTiles[i][availableTiles[i].Count - 1].name = tile.name + " - rot 180";
                    }
                    else
                    {
                        availableTiles[i].Add(new Tile(tile));
                        availableTiles[i][availableTiles[i].Count - 1].name = tile.name;

                    }
                    if (tile.symetry == Symetry.None)
                    {
                        availableTiles[i].Add(new Tile(tile, 3));
                        availableTiles[i][availableTiles[i].Count - 1].name = tile.name + " - rot 270";
                    }
                    else
                    {
                        availableTiles[i].Add(new Tile(tile));
                        availableTiles[i][availableTiles[i].Count - 1].name = tile.name;

                    }

                }
                foreach (Tile tile in asset.BackUpTiles)
                {
                    backupTiles[i].Add(new Tile(tile));
                    backupTiles[i][backupTiles[i].Count - 1].name = tile.name;
                }
            }

            /*foreach(var quad in grid)
            {
                var go = new GameObject("quad - " + quad.Position,  typeof(TileHolder));
                go.GetComponent<TileHolder>().Quad = quad;
                tiles.Add(go);
            }*/
            var meshModifier = new MeshModifier();
            var retry = false;

            var nbPlain = 1;
            var maxCellPlain = 50;
            var tilePlain = asset.Tiles[0];


            SetUpSomeTile(nbPlain, maxCellPlain, tilePlain, treated, grid, home);

            var nbMount = 2;
            var maxCellMount = 5;
            var tileMount = asset.BackUpTiles[0];


            SetUpSomeTile(nbMount, maxCellMount, tileMount, treated, grid);

            /*var nbPlain = 1;
            var maxCellPlain = 50;
            var tilePlain = asset.Tiles[0];


            SetUpSomeTile(nbPlain, maxCellPlain, tilePlain, treated, grid, home);

            var nbMount = 2;
            var maxCellMount = 50;
            var tileMount = asset.BackUpTiles[0];


            SetUpSomeTile(nbMount, maxCellMount, tileMount, treated, grid);
            yield return new WaitForSeconds(2f);*/

            var retryCount = 0;
            while(treated.Count < grid.Length)
            {
                // Get the list of cells where the entropy is the lowest
                List<v3Quad> lowestEntropy = GetLowestEntropyCells(treated, grid, out int lowestEntropyNb);
                Debug.Log("lowest entropy = " + lowestEntropyNb + ", quad = " + lowestEntropy[0].Position);
                if (lowestEntropy.Count == 0 || lowestEntropyNb == 0) 
                    retry = true;
                else
                {
                    var rndCell = UnityEngine.Random.Range(0, lowestEntropy.Count);
                    var chosenQuad = lowestEntropy[rndCell];
                    retry = PickTileToAdd(grid, treated, chosenQuad);
                    if (!retry)
                        GenerateMesh(grid, mapMaterial, meshModifier, chosenQuad);
                    
                    yield return null;
                }
                if (retry)
                {
                    foreach(var tile in tiles)
                        Destroy(tile);
                    tileHolders.Clear();
                    tiles.Clear();
                    StartCoroutine(StartWave(grid, mapMaterial));
                    break;
                }

            }

            if (!retry)
            {
                var noise = Noise.GenerateNoiseMap(500, 500, noiseSettings, Vector3.zero);

                StartCoroutine(meshModifier.ModifyMeshWithHeightMap(tileHolders, noise, 15f, heightCurve));
            }
            
            Debug.Log("End Time = " + Time.time);
        }

        private void SetUpSomeTile(int nbIteration, int maxCell, Tile tile, List<int> treated, v3Quad[] grid, GameObject prefab = null)
        {
            var meshModifier = new MeshModifier();
            var it = 0;
            while (it < nbIteration)
            {
                var rnd = UnityEngine.Random.Range(0, grid.Length - 1);
                var pickedQuad = grid[rnd];
                if (!treated.Contains(rnd))
                {
                    var newTile = new Tile(tile);
                    newTile.name = tile.name + " - static";
                    if(!AddTileToGrid(grid, treated, rnd, newTile, false))
                    {
                        GenerateMesh(grid, mapMaterial, meshModifier, pickedQuad);
                        //if(prefab)  tiles.Add(Instantiate(prefab, pickedQuad.Position, Quaternion.identity, transform));
                    }
                }


                var neighbourList = new List<v3Quad>();
                foreach (var neigh in pickedQuad.Neighbours)
                    neighbourList.Add((v3Quad)neigh.neighbour);
                foreach (var neigh in pickedQuad.CrossNeighbours)
                    neighbourList.Add((v3Quad)neigh.neighbour);

                var nbDone = 0f;
                foreach (var quad in neighbourList)
                {
                    if (nbDone == maxCell) break;

                    int id = FindIndexOfQuad(quad, grid);
                    if (!treated.Contains(id) && pickedQuad.GetCommonPoints(pickedQuad, quad).Count > 0)
                    {
                        var newTile = new Tile(tile);
                        newTile.name = tile.name + " - static";
                        if (!AddTileToGrid(grid, treated, id, newTile, false))
                            GenerateMesh(grid, mapMaterial, meshModifier, quad);
                        
                        nbDone++;
                    }

                }

                it++;
            }
        }

        private void GenerateMesh(v3Quad[] grid, Material mapMaterial, MeshModifier meshModifier, v3Quad chosenQuad)
        {
        // generate mesh
            var i = FindIndexOfQuad(chosenQuad, grid);
            if(gridTiled[i])
            {
                var go = new GameObject("quad - " + gridTiled[i].name, typeof(MeshFilter), typeof(MeshRenderer), typeof(TileHolder), typeof(MeshCollider));
                go.transform.parent = transform;
                go.transform.position = chosenQuad.Position;
                var filter = go.GetComponent<MeshFilter>();
                var rend = go.GetComponent<MeshRenderer>();
                var col = go.GetComponent<MeshCollider>();
                var holder = go.GetComponent<TileHolder>();
                holder.Tile = gridTiled[i];
                holder.Quad = quadTiled[i];
                tileHolders.Add(holder);
                tiles.Add(go);
                rend.material = mapMaterial;
                var modifiedMesh = meshModifier.ModifyMesh(grid[i].pts, gridTiled[i].mesh, chosenQuad.Position);
                holder.Tile.mesh = modifiedMesh;
                filter.mesh = modifiedMesh;
                col.sharedMesh = modifiedMesh;
            }
        }

        private bool PickTileToAdd(v3Quad[] grid, List<int> treated, v3Quad chosenQuad)
        {
            // We pick a tile from the available list of the chosen quad
            int index = FindIndexOfQuad(chosenQuad, grid);
            var rndTile = UnityEngine.Random.Range(0, availableTiles[index].Count);
            var pickedTile = availableTiles[index][rndTile];

            var retry = AddTileToGrid(grid, treated, index, pickedTile);
            return retry;
        }

        private bool AddTileToGrid(v3Quad[] grid, List<int> treated, int quadIndex, Tile pickedTile, bool autoComplete = true)
        {
            var retry = false;
            var rollBack = false;

            var chosenQuad = grid[quadIndex];
            var neighbours = chosenQuad.Neighbours;
            var crossNeighbours = chosenQuad.CrossNeighbours;

            var onlyOneAvailableIndexes = new List<int>();
            for (int x = 0; x < neighbours.Count; x++)
            {
                int j = FindIndexOfQuad((v3Quad)neighbours[x].neighbour, grid);
                if (gridTiled[j] == null)
                {
                    var oldList = new List<Tile>();
                    oldList.AddRange(availableTiles[j]);
                    RemoveUnavailableTiles(pickedTile, neighbours, x, j);
                    if (availableTiles[j].Count == 0)
                    {
                        Debug.Log("------ BUG NEIGHB ---------");
                        Debug.Log("picked tile = " + pickedTile.id);
                        Debug.Log("available count = " + availableTiles[quadIndex].Count);
                        Debug.Log("chosen quad = " + chosenQuad.Position);
                        Debug.Log("neighbour nb = " + x);
                        Debug.Log("------ END ---------");
                        if(autoComplete)
                        {
                            availableTiles[j] = oldList;
                            rollBack = true;
                        }
                        else if (!hasBackUpTile[j])
                        {
                            availableTiles[j].AddRange(backupTiles[j]);
                            hasBackUpTile[j] = true;
                        }
                    }
                    else 
                    if (availableTiles[j].Count == 1)
                    {
                        onlyOneAvailableIndexes.Add(j);
                    }
                }
            }
            for (int x = 0; x < crossNeighbours.Count; x++)
            {
                int j = FindIndexOfQuad((v3Quad)crossNeighbours[x].neighbour, grid);
                var oldList = new List<Tile>();
                oldList.AddRange(availableTiles[j]);
                if (gridTiled[j] == null)
                {
                    RemoveUnavailableTilesFromCross(pickedTile, crossNeighbours, x, j);
                    if (availableTiles[j].Count == 0)
                    {
                        Debug.Log("------ BUG CROSS ---------");
                        Debug.Log("picked tile = " + pickedTile.id);
                        Debug.Log("available count = " + availableTiles[quadIndex].Count);
                        Debug.Log("chosen quad = " + chosenQuad.Position);
                        Debug.Log("neighbour nb = " + x);
                        Debug.Log("------ END ---------");
                        if (autoComplete)
                        {
                            availableTiles[j] = oldList;
                            rollBack = true;
                        }
                        else if (!hasBackUpTile[j])
                        {
                            availableTiles[j].AddRange(backupTiles[j]);
                            hasBackUpTile[j] = true;
                        }
                    }
                    else if (availableTiles[j].Count == 1)
                    {
                        onlyOneAvailableIndexes.Add(j);
                    }
                }
            }
            if(!rollBack)
            {
                availableTiles[quadIndex].Clear();
                gridTiled[quadIndex] = pickedTile;
                quadTiled[quadIndex] = chosenQuad;
                treated.Add(quadIndex);
            }
            else if (autoComplete)
            {
                var oldAvailableList = new List<Tile> {};
                oldAvailableList.AddRange(availableTiles[quadIndex]);
                foreach(var tile in oldAvailableList)
                {
                    if(tile.CompareConnector(tile.connectors, pickedTile.connectors)) 
                        availableTiles[quadIndex].Remove(tile);
                }

                if (availableTiles[quadIndex].Count == 0)
                {
                    if(!hasBackUpTile[quadIndex])
                    {
                        availableTiles[quadIndex].AddRange(backupTiles[quadIndex]);
                        hasBackUpTile[quadIndex] = true;
                    }
                    else
                        retry = true;
                }
            }

            if(autoComplete)
            {
                var meshModifier = new MeshModifier();
                foreach (var index in onlyOneAvailableIndexes)
                {
                    if(availableTiles[index].Count == 1 && !AddTileToGrid(grid, treated, index, availableTiles[index][0])) 
                        GenerateMesh(grid, mapMaterial, meshModifier, grid[index]);
                }
            }
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
                if (!treated.Contains(i) && availableTiles[i].Count < lowestEntropy) lowestEntropy = availableTiles[i].Count;
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
