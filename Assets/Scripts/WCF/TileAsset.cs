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

        public List<Tile> Tiles { get => tiles; }
    }
}
