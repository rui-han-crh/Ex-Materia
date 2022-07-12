using CombatSystem.Censuses;
using Facades;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = CombatSystem.Entities.TileData;

namespace Managers
{
    public class TileManager : MonoBehaviour
    {
        private static TileManager instance;
        public static TileManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TileManager>();
                    Debug.Assert(instance != null, "There is no TilemapManager in the scene, consider adding one");
                }
                return instance;
            }
        }

        [HideInInspector]
        public TileDatabase tileDatabase;

        [SerializeField]
        private Tilemap indicatorMap;

        public Tilemap IndicatorMap => indicatorMap;

        public Tile Indicator => tileDatabase.indicatorTile;

        [SerializeField]
        private Tilemap[] groundTilemaps;
        [SerializeField]
        private Tilemap[] halfCoverTilemaps;
        [SerializeField]
        private Tilemap[] fullCoverTilemaps;

        public Tilemap Ground => groundTilemaps[0];

        public void Awake()
        {
            tileDatabase = Instantiate(tileDatabase);
        }

        public TileCensus CreateTileCensus()
        {
            IDictionary<Vector3Int, TileData> tileData = new Dictionary<Vector3Int, TileData>();

            void AddAllTilesToDictionary(Tilemap map, int cost, TileData.TileType tileType)
            {
                foreach (Vector3Int pos in map.cellBounds.allPositionsWithin)
                {
                    Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                    if (map.HasTile(localPlace) && !tileData.ContainsKey(localPlace))
                    {
                        Tile tile = map.GetTile<Tile>(localPlace);
                        Tile groundTile = groundTilemaps[0].GetTile<Tile>(localPlace);
                        tileData.Add(localPlace, new TileData(tileDatabase.GetTileName(groundTile), cost, tileType));
                    }
                }
            }

            foreach (Tilemap fullCoverMap in fullCoverTilemaps)
            {
                AddAllTilesToDictionary(fullCoverMap, int.MaxValue, TileData.TileType.FullCover);
            }

            foreach (Tilemap halfCoverMap in halfCoverTilemaps)
            {
                AddAllTilesToDictionary(halfCoverMap, int.MaxValue, TileData.TileType.HalfCover);
            }

            Debug.Assert(groundTilemaps.Length > 0, "FATAL ERROR: There was no ground map to be registered as playable. " +
                "This is extremely unlikely, as at least one Ground Tilemap should exist in a combat scene.");

            foreach (Tilemap groundMap in groundTilemaps)
            {
                foreach (Vector3Int pos in groundMap.cellBounds.allPositionsWithin)
                {
                    Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                    if (groundMap.HasTile(localPlace) && !tileData.ContainsKey(localPlace))
                    {
                        Tile tile = groundMap.GetTile<Tile>(localPlace);
                        int cost = tileDatabase.Contains(tile) ? tileDatabase[tile] : 0;
                        tileData.Add(localPlace, new TileData(tileDatabase.GetTileName(tile), cost, TileData.TileType.Ground));
                    }
                }
            }
            return new TileCensus(tileData);
        }
    }
}
