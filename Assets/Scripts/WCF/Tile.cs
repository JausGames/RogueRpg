using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WCF
{
    [CreateAssetMenu(fileName = "Tile", menuName = "WCF/Tile", order = 3)]
    public class Tile : ScriptableObject
    {
        //0-2 top
        //3-5 left
        //6-8 bottom
        //9-11 right
        public TileConnector[] connectors = new TileConnector[4];
        public int id;
        public Mesh mesh;


        public Tile(Tile tile)
        {
            this.connectors = tile.connectors;
            this.mesh = tile.mesh;
            this.id = tile.id;
        }
        public Tile(Tile tile, int nbRotation)
        {
            this.connectors = tile.RotateTile(nbRotation);
            this.mesh = tile.RotateMesh(nbRotation);
            this.id = tile.id;
        }

        private Mesh RotateMesh(int rotation)
        {
            var rotatedMesh = new Mesh();
            var points = new Vector3[mesh.vertices.Length];
            for(int i = 0; i < mesh.vertices.Length; i++)
            {
                points[i] = Quaternion.Euler(0, 0, rotation * -90f) * mesh.vertices[i];
            }
            rotatedMesh.vertices = points;
            rotatedMesh.triangles = mesh.triangles;
            rotatedMesh.RecalculateNormals();
            rotatedMesh.RecalculateBounds();
            rotatedMesh.RecalculateTangents();
            return rotatedMesh;
        }

        /// <summary>
        /// Check if the given tiles can be connected.
        /// Direction (on tile1):
        /// 0 for top
        /// 1 for right
        /// 2 for bottom
        /// 3 for left
        /// </summary>
        /// <param name="tile1"></param>
        /// <param name="tile2"></param>
        /// <param name="direction"></param>
        /// <param name="oppoDirection"></param>
        /// <returns></returns>
        public static bool CheckIfTileConnect(Tile tile1, Tile tile2, int direction, int oppoDirection)
        {
            if (tile1.connectors[direction].connection[0] == tile2.connectors[oppoDirection].connection[2]
            && tile1.connectors[direction].connection[1] == tile2.connectors[oppoDirection].connection[1]
            && tile1.connectors[direction].connection[2] == tile2.connectors[oppoDirection].connection[0])
                return true;

            return false;
        }
        public static bool CheckIfTileConnectCross(Tile tile1, Tile tile2, int ptIndex1, int ptIndex2)
        {

            /*if (tile1.name.Contains("High") && tile2.name.Contains("Mount") || tile2.name.Contains("High") && tile1.name.Contains("Mount"))
                Debug.Log("debug");*/

            if (tile1.connectors[(3 + ptIndex1) % 4].connection[0] == tile2.connectors[(3 + ptIndex2) % 4].connection[0]
            && tile1.connectors[(ptIndex1) % 4].connection[2] == tile2.connectors[(ptIndex2) % 4].connection[2])
                return true;

            return false;
        }
        public TileConnector[] RotateTile(int nb90Rotation)
        {
            var newConnector = new TileConnector[connectors.Length];

            for (int i = 0; i < connectors.Length ; i++)
            {
                newConnector[(i + nb90Rotation) % connectors.Length] = connectors[i];
            }

            return newConnector;
        }

        internal bool CompareConnector(TileConnector[] connectors1, TileConnector[] connectors2)
        {
            for(int i = 0; i < connectors1.Length; i ++)
            if (connectors1[i].connection[0] == connectors2[i].connection[0]
            && connectors1[i].connection[1] == connectors2[i].connection[1]
            && connectors1[i].connection[2] == connectors2[i].connection[2])
                return true;

            return false;
        }
    }
    [Serializable]
    public class TileConnector
    {
        public Connection[] connection = new Connection[3];
        public List<Tile> blackList = new List<Tile>();
        public List<Tile> whiteList = new List<Tile>();
        public List<int> BlackList
        {
            get
            {
                List<int> list = new List<int>();
                for (int i = 0; i < blackList.Count; i++)
                        list.Add(blackList[i].id);
                return list;
            }
        }
        public List<int> WhiteList
        {
            get
            {
                List<int> list = new List<int>();
                for (int i = 0; i < whiteList.Count; i++)
                        list.Add(whiteList[i].id);
                return list;
            }
        }
    }
    public enum Connection
    {
        Low,
        Mid,
        High
    }

}