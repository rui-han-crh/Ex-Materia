using CombatSystem.Censuses;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = CombatSystem.Entities.TileData;
namespace CombatSystem.Facades
{
    [RequireComponent(typeof(TileRegisterFacade))]
    public class TileMapFacade : MonoBehaviour
    {
        public Tilemap[] groundTilemaps;
        public Tilemap[] halfCoverTilemaps;
        public Tilemap[] fullCoverTilemaps;
        private TileRegisterFacade tileRegisterFacade;
        public TileCensus CreateTileCensus()
        {
            tileRegisterFacade = GetComponent<TileRegisterFacade>();
            IDictionary<Vector3Int, TileData> tileData = new Dictionary<Vector3Int, TileData>();
            void AddAllTilesToDictionary(Tilemap map, int cost, TileData.TileType tileType)
            {
                for (int i = 0; i < map.size.x; i++)
                {
                    for (int j = 0; j < map.size.y; j++)
                    {
                        Vector3Int coordinates = new Vector3Int(i, j, 0);
                        if (map.HasTile(coordinates) && !tileData.ContainsKey(coordinates))
                        {
                            tileData.Add(coordinates, new TileData(cost, tileType));
                        }
                    }
                }
            }
            foreach (Tilemap fullCoverMap in fullCoverTilemaps)
            {
                AddAllTilesToDictionary(fullCoverMap, int.MaxValue, TileData.TileType.FullCover);
            }
            foreach (Tilemap halfCoverMap in fullCoverTilemaps)
            {
                AddAllTilesToDictionary(halfCoverMap, int.MaxValue, TileData.TileType.HalfCover);
            }
            foreach (Tilemap groundMap in fullCoverTilemaps)
            {
                for (int i = 0; i < groundMap.size.x; i++)
                {
                    for (int j = 0; j < groundMap.size.y; j++)
                    {
                        Vector3Int coordinates = new Vector3Int(i, j, 0);
                        if (groundMap.HasTile(coordinates) && !tileData.ContainsKey(coordinates))
                        {
                            tileData.Add(coordinates,
                                new TileData(tileRegisterFacade[groundMap.GetTile<Tile>(coordinates)], TileData.TileType.Ground));
                        }
                    }
                }
            }
            return new TileCensus(tileData);
        }
    }
}