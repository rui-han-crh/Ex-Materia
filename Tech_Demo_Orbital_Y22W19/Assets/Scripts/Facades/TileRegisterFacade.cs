//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Tilemaps;

//namespace Facades
//{
//    public class TileRegisterFacade : MonoBehaviour
//    {
//        [Serializable]
//        public class TileCostPair
//        {
//            [SerializeField]
//            private string tileName;
//            [SerializeField]
//            private Tile tile;
//            [SerializeField]
//            private int cost;

//            public string TileName => tileName;
//            public Tile Tile => tile;
//            public int Cost => cost;
//        }

//        public TileCostPair[] tileCostPairs;
//        private Dictionary<Tile, int> tileCosts = new Dictionary<Tile, int>();
//        private Dictionary<Tile, string> tileNames = new Dictionary<Tile, string>();
//        private void Awake()
//        {
//            foreach (TileCostPair tileCostPair in tileCostPairs)
//            {
//                tileCosts.Add(tileCostPair.Tile, tileCostPair.Cost);
//                tileNames.Add(tileCostPair.Tile, tileCostPair.TileName);
//            }
//        }

//        public bool Contains(Tile tile)
//        {
//            return tileCosts.ContainsKey(tile);
//        }

//        public int this[Tile tile]
//        {
//            get => tileCosts[tile];
//        }

//        public string GetTileName(Tile tile)
//        {
//            if (tile == null)
//            {
//                Debug.LogWarning($"The tile given was null");
//            }

//            if (!tileNames.ContainsKey(tile))
//            {
//                Debug.LogWarning($"Tile {tile.name} was not found in tileNames mapping");
//            }

//            return tileNames[tile];
//        }
//    }
//}
