using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PositionOnTileMap : MonoBehaviour
{
    public Tilemap referenceTilemap;

    public Vector3 positionInWorldSpace;
    public Vector3 positionInGridSpace;

    public Vector3 GetWorldPositionOnTilemap()
    {
        return referenceTilemap.CellToWorld(GetCellPositionOnTilemap());
    }

    public Vector3Int GetCellPositionOnTilemap()
    {
        return referenceTilemap.WorldToCell(transform.position);
    }
}
