using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Entities;
using System;

namespace CombatSystem.Censuses
{
    public class TileCensus
    {
        private readonly Dictionary<Vector3Int, TileData> census = new Dictionary<Vector3Int, TileData>();

        public IEnumerable<Vector3Int> Census => census.Keys;

        public TileCensus(IDictionary<Vector3Int, TileData> positionToUnitMapping)
        {
            census = new Dictionary<Vector3Int, TileData>(positionToUnitMapping);
        }

        public TileCensus(Dictionary<Vector3Int, TileData> newCensus)
        {
            census = newCensus;
        }

        private TileCensus(TileCensus previousCensus)
        {
            this.census = previousCensus.census;
        }

        public TileCensus()
        {

        }

        public TileData this[Vector3Int position]
        {
            get
            {
                return census[position];
            }
        }

        public int Count => census.Count;

        public TileCensus Add(Vector3Int position, TileData tileData)
        {
            Dictionary<Vector3Int, TileData> newCensus = new Dictionary<Vector3Int, TileData>(census);
            newCensus.Add(position, tileData);
            return new TileCensus(newCensus);
        }

        public TileCensus Remove(Vector3Int position)
        {
            if (!census.ContainsKey(position))
            {
                throw new KeyNotFoundException($"No data exists at {position}");
            }

            Dictionary<Vector3Int, TileData> newCensus = new Dictionary<Vector3Int, TileData>(census);
            newCensus.Remove(position);
            return new TileCensus(newCensus);
        }

        public bool Contains(Vector3Int position)
        {
            return census.ContainsKey(position);
        }

        public TileData.TileType GetTileType(Vector3Int position)
        {
            return census[position].Type;
        }

        public int GetTileCost(Vector3Int position)
        {
            return census[position].Cost;
        }

        public TileCensus Clone()
        {
            return new TileCensus(this);
        }
    }
}
