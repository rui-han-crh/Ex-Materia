using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class LineDrawer
{
    private static Vector3 WORLD_OFFSET = new Vector3(0, 0.25f, 0);
    public static void DrawLineOnTileMap(Tilemap tilemap, LineRenderer lineRenderer, IEnumerable<Vector3Int> pathPosition, Vector3Int startPosition)
    {
        lineRenderer.positionCount = 0;

        lineRenderer.positionCount = 1 + pathPosition.Count();
        lineRenderer.SetPosition(0, tilemap.CellToWorld(startPosition) + WORLD_OFFSET);

        for (int i = 0; i < pathPosition.Count(); i++) 
        { 
            lineRenderer.SetPosition(i + 1, tilemap.CellToWorld(pathPosition.ElementAt(i)) + WORLD_OFFSET);
        }
    }

    public static void DrawLineOnTileMap(Tilemap tilemap, LineRenderer lineRenderer, IEnumerable<Vector3Int> pathPosition)
    {
        lineRenderer.positionCount = 0;

        lineRenderer.positionCount = pathPosition.Count();
        lineRenderer.SetPositions(pathPosition.Select(x => tilemap.CellToWorld(x) + WORLD_OFFSET).ToArray());
    }

    public static void ColorLine(LineRenderer lineRenderer, Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
}