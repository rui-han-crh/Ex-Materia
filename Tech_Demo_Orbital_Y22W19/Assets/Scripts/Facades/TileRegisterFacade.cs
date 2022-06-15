using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace CombatSystem.Facades
{
    public class TileRegisterFacade : MonoBehaviour
    {
        [Serializable]
        public class TileCostPair
        {
            [SerializeField]
            private Tile tile;
            [SerializeField]
            private int cost;
            public Tile Tile => tile;
            public int Cost => cost;
        }

        public TileCostPair[] tileCostPairs;
        private Dictionary<Tile, int> tileCosts = new Dictionary<Tile, int>();
        private void Awake()
        {
            foreach (TileCostPair tileCostPair in tileCostPairs)
            {
                tileCosts.Add(tileCostPair.Tile, tileCostPair.Cost);
            }
        }
        public int this[Tile tile]
        {
            get => tileCosts[tile];
        }
    }
}
