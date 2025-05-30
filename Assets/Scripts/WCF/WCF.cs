using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridGenerator;
using static GridGenerator.MeshData;

namespace WCF
{
    public class WCF : MonoBehaviour
    {
        //List<Tile> tiles = new List<Tile>();
        [SerializeField]
        TileAsset asset = null;
        private Material[] mapMaterials;
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
        private List<InstanciableObject> objectsToSpawn = new List<InstanciableObject>();

        [SerializeField]
        NoiseSettings noiseSettings;
        MeshModifier meshModifier;
        [SerializeField]
        AnimationCurve heightCurve;

        private bool done = false;
        private List<Tile>[] borderTiles;

        public List<Tile> Tiles { get => asset.Tiles;}
        public bool Done { get => done; set => done = value; }

        public IEnumerator StartWave(MeshData meshData, Material[] mapMaterials)
        {
            meshModifier = new MeshModifier();
            meshModifier.SetValuesToModMesh(asset.Tiles[0].meshGround.vertices);
            v3Quad[] grid = meshData.Quads.ToArray();
            this.mapMaterials = mapMaterials;
            gridTiled = new Tile[grid.Length];
            quadTiled = new v3Quad[grid.Length];
            hasBackUpTile = new bool[grid.Length];
            availableTiles = new List<Tile>[grid.Length];
            var baseAvailableTiles = new List<Tile>();

            foreach (var tile in asset.Tiles)
            {
                baseAvailableTiles.Add(new Tile(tile));
                //baseAvailableTiles.Add(new Tile(tile));
                baseAvailableTiles[baseAvailableTiles.Count - 1].name = tile.name;
                if (tile.symetry != Symetry.Full) {
                    baseAvailableTiles.Add(new Tile(tile, 1));
                    baseAvailableTiles[baseAvailableTiles.Count - 1].name = tile.name + " - rot 90"; }
                else {
                    baseAvailableTiles.Add(new Tile(tile));
                    baseAvailableTiles[baseAvailableTiles.Count - 1].name = tile.name; }
                if (tile.symetry == Symetry.None) {
                    baseAvailableTiles.Add(new Tile(tile, 2));
                    baseAvailableTiles[baseAvailableTiles.Count - 1].name = tile.name + " - rot 180"; }
                else {
                    baseAvailableTiles.Add(new Tile(tile));
                    baseAvailableTiles[baseAvailableTiles.Count - 1].name = tile.name; }
                if (tile.symetry == Symetry.None) {
                    baseAvailableTiles.Add(new Tile(tile, 3));
                    baseAvailableTiles[baseAvailableTiles.Count - 1].name = tile.name + " - rot 270"; }
                else {
                    baseAvailableTiles.Add(new Tile(tile));
                    baseAvailableTiles[baseAvailableTiles.Count - 1].name = tile.name; }

            }

            backupTiles = new List<Tile>[grid.Length];
            var baseBackupTiles = new List<Tile>();
            
            foreach(var tile in asset.BackUpTiles)
            {
                baseBackupTiles.Add(new Tile(tile));
                baseBackupTiles[baseBackupTiles.Count - 1].name = tile.name;
                if (tile.symetry != Symetry.Full)
                {
                    baseBackupTiles.Add(new Tile(tile, 1));
                    baseBackupTiles[baseBackupTiles.Count - 1].name = tile.name + " - rot 90";
                }
                if (tile.symetry == Symetry.None)
                {
                    baseBackupTiles.Add(new Tile(tile, 2));
                    baseBackupTiles[baseBackupTiles.Count - 1].name = tile.name + " - rot 180";
                }
                if (tile.symetry == Symetry.None)
                    {
                    baseBackupTiles.Add(new Tile(tile, 3));
                    baseBackupTiles[baseBackupTiles.Count - 1].name = tile.name + " - rot 270";
                }
            }

            var treated = new List<int>();

            //Set up available tiles
            for (int i = 0; i < availableTiles.Length; i++)
            {
                hasBackUpTile[i] = false;
                availableTiles[i] = new List<Tile>();
                backupTiles[i] = new List<Tile>();
                //Add all tiles to the available tile for each cell of the grid
                foreach (Tile tile in baseAvailableTiles)
                {
                    var newTile = new Tile(tile);
                    newTile.name = tile.name;
                    availableTiles[i].Add(newTile);
                    availableTiles[i][availableTiles[i].Count - 1].name = tile.name;
                }
                foreach(var tile in baseBackupTiles)
                {
                    var newTile = new Tile(tile);
                    newTile.name = tile.name;
                    backupTiles[i].Add(newTile);
                    backupTiles[i][backupTiles[i].Count - 1].name = tile.name;
                }
            }
            var retry = false;

            
            SetUpBorders(meshData.BorderQuads, treated, grid);
            RecalculateAvailables(treated, grid);

            foreach(var constantTiles in asset.ConstantTiles)
                SetUpSomeTile(constantTiles.nbIteration, constantTiles.maxPerIt, constantTiles.tile, treated, grid, constantTiles.prefab);

            while(treated.Count < grid.Length)
            {
                // Get the list of cells where the entropy is the lowest
                List<v3Quad> lowestEntropy = GetLowestEntropyCells(treated, grid, out int lowestEntropyNb);
                Debug.Log("lowest entropy = " + lowestEntropyNb + ", quad = " + lowestEntropy[0].Position);
                if (lowestEntropyNb == 0)
                {
                    foreach(var quad in lowestEntropy)
                    {
                        var id = FindIndexOfQuad(quad, grid);
                        if (hasBackUpTile[id]) retry = true;
                        else
                        {
                            availableTiles[id].AddRange(backupTiles[id]);
                            hasBackUpTile[id] = true;
                        }
                    }
                }
                else
                {
                    var rndCell = UnityEngine.Random.Range(0, lowestEntropy.Count);
                    var chosenQuad = lowestEntropy[rndCell];
                    retry = PickTileToAdd(grid, treated, chosenQuad);
                    if (!retry)
                        GenerateMesh(grid, mapMaterials, meshModifier, chosenQuad);
                    
                    yield return null;
                }
                if (retry)
                {
                    foreach(var tile in tiles)
                        Destroy(tile);
                    objectsToSpawn.Clear();
                    tileHolders.Clear();
                    tiles.Clear();
                    break;
                }

            }
            if (retry) yield return false;
            else
                yield return tileHolders;
            

            Debug.Log("End Time = " + Time.time);
        }

        public void SpawnObjects()
        {
            foreach (var obj in objectsToSpawn)
            {
                if (Physics.Raycast(obj.position + Vector3.up * 100f, Vector3.down, out RaycastHit hit, 500f))
                    tiles.Add(Instantiate(obj.prefab, hit.point, obj.rotation, obj.parent));
            }

            objectsToSpawn.Clear();
        }

        private void RecalculateAvailables(List<int> treated, v3Quad[] grid)
        {
            foreach(var id in treated)
            {
                var chosenQuad = grid[id];
                var neighbours = chosenQuad.Neighbours;
                var crossNeighbours = chosenQuad.CrossNeighbours;

                for (int x = 0; x < neighbours.Count; x++)
                {
                    int j = FindIndexOfQuad((v3Quad)neighbours[x].neighbour, grid);
                    if (gridTiled[j] == null && !treated.Contains(j))
                    {
                        var oldList = new List<Tile>();
                        oldList.AddRange(availableTiles[j]);
                        RemoveUnavailableTiles(gridTiled[id], neighbours, x, j);
                        if (availableTiles[j].Count == 0)
                        {
                            Debug.Log("------ BUG NEIGHB ---------");
                            Debug.Log("picked tile = " + gridTiled[id].id);
                            Debug.Log("available count = " + availableTiles[id].Count);
                            Debug.Log("chosen quad = " + chosenQuad.Position);
                            Debug.Log("neighbour nb = " + x);
                            Debug.Log("------ END ---------");
                            if (!hasBackUpTile[j])
                            {
                                availableTiles[j].AddRange(backupTiles[j]);
                                hasBackUpTile[j] = true;
                            }
                        }
                    }
                }
                for (int x = 0; x < crossNeighbours.Count; x++)
                {
                    int j = FindIndexOfQuad((v3Quad)crossNeighbours[x].neighbour, grid);
                    var oldList = new List<Tile>();
                    oldList.AddRange(availableTiles[j]);
                    if (gridTiled[j] == null && !treated.Contains(j))
                    {
                        RemoveUnavailableTilesFromCross(gridTiled[id], crossNeighbours, x, j);
                        if (availableTiles[j].Count == 0)
                        {
                            Debug.Log("------ BUG CROSS ---------");
                            Debug.Log("picked tile = " + gridTiled[id].id);
                            Debug.Log("available count = " + availableTiles[id].Count);
                            Debug.Log("chosen quad = " + chosenQuad.Position);
                            Debug.Log("neighbour nb = " + x);
                            Debug.Log("------ END ---------");
                            if (!hasBackUpTile[j])
                            {
                                availableTiles[j].AddRange(backupTiles[j]);
                                hasBackUpTile[j] = true;
                            }
                        }
                    }
                }
            }
        }
        

        private void SetUpBorders(List<BorderQuad> borders, List<int> treated, v3Quad[] grid)
        {
            for(int i = 0; i < borders.Count; i++)
            {
                int id = FindIndexOfQuad(borders[i].quad, grid);
                if (!treated.Contains(id))
                {
                    if (borders[i].edges.Length == 2)
                    {
                        Debug.Log("borders[i].edges[0] = " + borders[i].edges[0]);
                        Debug.Log("borders[i].edges[1] = " + borders[i].edges[1]);
                        //var rotation = (borders[i].edges[0] == 0 && borders[i].edges[1] == 3) || (borders[i].edges[0] > borders[i].edges[1]) ? borders[i].edges[1] : borders[i].edges[0];
                        var rotation = 0;
                        for (int b = 0; b < 4; b++)
                            if (asset.BorderTiles[2].connectors[b].connection[0] == Connection.Border
                                && asset.BorderTiles[2].connectors[b].connection[1] == Connection.Border
                                && asset.BorderTiles[2].connectors[b].connection[2] == Connection.Border
                                && asset.BorderTiles[2].connectors[(b + 1) % 3].connection[0] == Connection.Border
                                && asset.BorderTiles[2].connectors[(b + 1) % 3].connection[1] == Connection.Border
                                && asset.BorderTiles[2].connectors[(b + 1) % 3].connection[2] == Connection.Border)
                                rotation += (b + 3) % 4;
                        var tile = new Tile(asset.BorderTiles[2], rotation);
                        tile.name = asset.BorderTiles[2].name + " - rot " + (rotation * 90f);

                        //var tile = new Tile(asset.BorderTiles[0], borders[i].edges[0] > borders[i].edges[1] ? borders[i].edges[0] : borders[i].edges[1]);
                        if (!AddTileToGrid(grid, treated, id, tile, false, false, false))
                        {
                            GenerateMesh(grid, mapMaterials, meshModifier, borders[i].quad);
                        }
                    }
                    else if (borders[i].edges.Length == 1)
                    {
                        var offset = -1;
                        for (int b = 0; b < 4; b++)
                            if (asset.BorderTiles[1].connectors[b].connection[0] == Connection.Border
                                && asset.BorderTiles[1].connectors[b].connection[1] == Connection.Border
                                && asset.BorderTiles[1].connectors[b].connection[2] == Connection.Border)
                                offset = (borders[i].edges[0] + (b + 0)) % 4;
                        var tile = new Tile(asset.BorderTiles[1], offset);
                        tile.name = asset.BorderTiles[1].name + " - rot " + (offset * 90f);
                        // for ASSET 02 var tile = new Tile(asset.BorderTiles[0], borders[i].edges[0] + 2);
                        if (!AddTileToGrid(grid, treated, id, tile, false, false, false))
                        {
                            GenerateMesh(grid, mapMaterials, meshModifier, borders[i].quad);
                        }
                    }
                    else if (borders[i].points.Length == 1)
                    {
                        // for ASSET 02 var tile = new Tile(asset.BorderTiles[2], 1);
                        var rotation = -1;
                        for(int b = 0; b < 4; b++)
                            if (asset.BorderTiles[0].connectors[b].connection[0] == Connection.Border)
                                rotation = (b+3)%4;

                        var tile = new Tile(asset.BorderTiles[0], rotation);
                        tile.name = asset.BorderTiles[0].name + " - rot " + (rotation * 90f);
                        if (!AddTileToGrid(grid, treated, id, tile, false, false, false))
                        {
                            GenerateMesh(grid, mapMaterials, meshModifier, borders[i].quad);
                        }
                    }
                }
            }
        }

        private void SetUpSomeTile(int nbIteration, int maxCell, Tile tile, List<int> treated, v3Quad[] grid, GameObject prefab = null)
        {
            var it = 0;
            while (it < nbIteration)
            {
                var rnd = treated[0]; // I know but it goes in the while this way
                var pickedQuad = grid[rnd];
                while(treated.Contains(rnd))
                {
                    rnd = UnityEngine.Random.Range(0, grid.Length - 1);
                    pickedQuad = grid[rnd];
                    
                }
                if (!treated.Contains(rnd))
                {
                    var newTile = new Tile(tile);
                    newTile.name = tile.name + " - static";
                    if(!AddTileToGrid(grid, treated, rnd, newTile, false, false))
                    {
                        GenerateMesh(grid, mapMaterials, meshModifier, pickedQuad);
                        /*if(prefab)  
                            tiles.Add(Instantiate(prefab, pickedQuad.Position, Quaternion.identity, transform));*/
                        if(prefab) 
                            objectsToSpawn.Add(new InstanciableObject(prefab, pickedQuad.Position, Quaternion.identity, transform));
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
                        if (!AddTileToGrid(grid, treated, id, newTile, false, false))
                            GenerateMesh(grid, mapMaterials, meshModifier, quad);
                        
                        nbDone++;
                    }

                }

                it++;
            }
        }

        private void GenerateMesh(v3Quad[] grid, Material[] mapMaterials, MeshModifier meshModifier, v3Quad chosenQuad)
        {
        // generate mesh
            var i = FindIndexOfQuad(chosenQuad, grid);
            if(gridTiled[i])
            {
                var go = new GameObject("quad - " + gridTiled[i].name, typeof(TileHolder));
                go.transform.parent = transform;
                go.transform.position = chosenQuad.Position;
                go.layer = LayerMask.NameToLayer("Terrain");

                var holder = go.GetComponent<TileHolder>();
                holder.Tile = gridTiled[i];
                holder.Quad = quadTiled[i];
                tileHolders.Add(holder);
                tiles.Add(go);

                if (gridTiled[i].meshGround)
                {
                    var goGround = new GameObject("ground - " + gridTiled[i].name, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
                    goGround.transform.parent = go.transform;
                    goGround.transform.position = chosenQuad.Position;
                    goGround.layer = LayerMask.NameToLayer("Terrain");

                    var filterGround = goGround.GetComponent<MeshFilter>();
                    var rendGround = goGround.GetComponent<MeshRenderer>();
                    var colGround = goGround.GetComponent<MeshCollider>();
                    rendGround.material = mapMaterials[0];
                    var modifiedGroundMesh = meshModifier.ModifyMesh(grid[i].pts, gridTiled[i].meshGround, chosenQuad.Position);
                    holder.Tile.meshGround = modifiedGroundMesh;
                    filterGround.mesh = modifiedGroundMesh;
                    colGround.sharedMesh = modifiedGroundMesh;
                    holder.meshHolders[0] = goGround;
                }
                if (gridTiled[i].meshHighground)
                {
                    var goHighround = new GameObject("highground - " + gridTiled[i].name, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
                    goHighround.transform.parent = go.transform;
                    goHighround.transform.position = chosenQuad.Position;
                    goHighround.layer = LayerMask.NameToLayer("Terrain");

                    var filterHighground = goHighround.GetComponent<MeshFilter>();
                    var rendHighground = goHighround.GetComponent<MeshRenderer>();
                    var colHighGround = goHighround.GetComponent<MeshCollider>();
                    rendHighground.material = mapMaterials[1];
                    var modifiedHighgroundMesh = meshModifier.ModifyMesh(grid[i].pts, gridTiled[i].meshHighground, chosenQuad.Position);
                    holder.Tile.meshHighground = modifiedHighgroundMesh;
                    filterHighground.mesh = modifiedHighgroundMesh;
                    colHighGround.sharedMesh = modifiedHighgroundMesh;
                    holder.meshHolders[1] = goHighround;
                }
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

        private bool AddTileToGrid(v3Quad[] grid, List<int> treated, int quadIndex, Tile pickedTile, bool autoComplete = true, bool canRollback = true, bool checkNeigbours = true)
        {
            var retry = false;
            var rollBack = false;

            var chosenQuad = grid[quadIndex];
            var neighbours = chosenQuad.Neighbours;
            var crossNeighbours = chosenQuad.CrossNeighbours;

            var onlyOneAvailableIndexes = new List<int>();
            if (checkNeigbours)
            {
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
                            if (canRollback)
                            {
                                availableTiles[j] = oldList;
                                rollBack = true;
                            }
                            else if (!hasBackUpTile[j])
                            {
                                availableTiles[j].AddRange(backupTiles[j]);
                                hasBackUpTile[j] = true;
                            }
                            else retry = true;
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
                            if (canRollback)
                            {
                                availableTiles[j] = oldList;
                                rollBack = true;
                            }
                            else if (!hasBackUpTile[j])
                            {
                                availableTiles[j].AddRange(backupTiles[j]);
                                hasBackUpTile[j] = true;
                            }
                            else retry = true;
                        }
                        else if (availableTiles[j].Count == 1)
                        {
                            onlyOneAvailableIndexes.Add(j);
                        }
                    }
                }
            }
            if(!rollBack && !retry)
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
                    else retry = true;
                }
            }

            if(autoComplete)
            {
                foreach (var index in onlyOneAvailableIndexes)
                {
                    if(availableTiles[index].Count == 1 && !AddTileToGrid(grid, treated, index, availableTiles[index][0])) 
                        GenerateMesh(grid, mapMaterials, meshModifier, grid[index]);
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

    public class InstanciableObject
    {
        public Vector3 position;
        public Quaternion rotation;
        public GameObject prefab;
        public Transform parent;

        public InstanciableObject(GameObject prefab, Vector3 position, Quaternion rotation,  Transform parent = null)
        { 
            this.position = position;
            this.rotation = rotation;
            this.prefab = prefab;
            this.parent = parent;
        }
    }

}
