using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public static class TileDrawer
{
    public static void Draw(Tilemap map, IEnumerable<Vector3Int> tilePositions, TileBase tileBase)
    {
        foreach (Vector3Int position in tilePositions)
        {
            map.SetTile(position, tileBase);
        }
    }

    public static void SetColorToTiles(Tilemap map, IEnumerable<Vector3Int> tilePositions, Color color)
    {
        foreach (Vector3Int position in tilePositions)
        {
            map.SetTileFlags(position, TileFlags.None);
            map.SetColor(position, color);
        }
    }

    public static void SetColorToTiles(Tilemap map, Color color)
    {
        foreach (Vector3Int position in map.cellBounds.allPositionsWithin)
        {
            if (map.HasTile(position))
            {
                map.SetTileFlags(position, TileFlags.None);
                map.SetColor(position, color);
            }
        }
    }
}