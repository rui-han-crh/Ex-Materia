using CombatSystem.Censuses;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = CombatSystem.Entities.TileData;

namespace Facades
{
    [RequireComponent(typeof(TileRegisterFacade))]
    public class TileMapFacade : MonoBehaviour
    {
        private static TileMapFacade instance;
        public static TileMapFacade Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TileMapFacade();
                    Debug.Assert(instance != null, "There is no TileMapFacade in the scene, consider adding one");
                }
                return instance;
            }
        }

        public Tilemap[] groundTilemaps;
        public Tilemap[] halfCoverTilemaps;
        public Tilemap[] fullCoverTilemaps;
        public Tilemap indicatorMap;

        private TileRegisterFacade tileRegisterFacade;
        public TileCensus CreateTileCensus()
        {
            tileRegisterFacade = GetComponent<TileRegisterFacade>();
            IDictionary<Vector3Int, TileData> tileData = new Dictionary<Vector3Int, TileData>();

            void AddAllTilesToDictionary(Tilemap map, int cost, TileData.TileType tileType)
            {
                foreach (Vector3Int pos in map.cellBounds.allPositionsWithin)
                {
                    Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                    if (map.HasTile(localPlace) && !tileData.ContainsKey(localPlace))
                    {
                        tileData.Add(localPlace, new TileData(cost, tileType));
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
            foreach (Tilemap groundMap in groundTilemaps)
            {
                foreach (Vector3Int pos in groundMap.cellBounds.allPositionsWithin)
                {
                    Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                    if (groundMap.HasTile(localPlace) && !tileData.ContainsKey(localPlace))
                    {
                        Tile tile = groundMap.GetTile<Tile>(localPlace);
                        int cost = tileRegisterFacade.Contains(tile) ? tileRegisterFacade[tile] : 0;
                        tileData.Add(localPlace, new TileData(cost, TileData.TileType.Ground));
                    }
                }
            }
            return new TileCensus(tileData);
        }
    }
}