using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

public class TileManagerOld : MonoBehaviour
{
    private static TileManagerOld instance;
    public static TileManagerOld Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TileManagerOld>();
                Debug.Assert(instance != null, "There was no TileManager in the scene, consider adding one");
            }
            return instance;
        }
    }

    [SerializeField]
    private Tilemap ground;

    [SerializeField]
    private Tilemap obstacles;

    [SerializeField]
    private Tilemap tileIndicators;

    [SerializeField]
    private TileBase selectorTile;

    [SerializeField]
    private Tile[] tiles;

    [SerializeField]
    private int[] tileCost;

    public Tilemap Ground => ground;

    public Tilemap Obstacles => obstacles;

    public Tilemap TileIndicators => tileIndicators;

    public TileBase SelectorTile => selectorTile;

    private Dictionary<Tile, int> tileCostMap = new Dictionary<Tile, int>();

    public void Awake()
    {
        Debug.Assert(tiles.Length == tileCost.Length, "Length of tiles array does not correspond to tileCost array");
        for (int i = 0; i < tiles.Length; i++)
        {
            tileCostMap[tiles[i]] = tileCost[i];
        }
    }

    public int GetTileCost(Tile tile)
    {
        if (tile == null || !tileCostMap.ContainsKey(tile))
        {
            return int.MaxValue;
        }
        return tileCostMap[tile];
    }
}