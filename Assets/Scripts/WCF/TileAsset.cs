using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WCF
{
    [CreateAssetMenu(fileName = "TileAsset", menuName = "WCF/TileAsset", order = 4)]
    public class TileAsset : ScriptableObject
    {
        [SerializeField]
        List<Tile> tiles = new List<Tile>();
        [SerializeField]
        List<Tile> backUpTiles = new List<Tile>();
        [SerializeField]
        List<Tile> borderTiles = new List<Tile>();

        public List<Tile> Tiles { get => tiles; }
        public List<Tile> BackUpTiles { get => backUpTiles; }
        public List<Tile> BorderTiles { get => borderTiles; }

    }
}
