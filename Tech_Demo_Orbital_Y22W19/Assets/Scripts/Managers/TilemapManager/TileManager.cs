using Facades;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

        [SerializeField]
        private TileMapFacade tileMapFacade;

        [SerializeField]
        private Tilemap indicatorMap;

        [SerializeField]
        private Tile indicator;

        public Tile Indicator => indicator;
        public Tilemap IndicatorMap => indicatorMap;
    }
}
