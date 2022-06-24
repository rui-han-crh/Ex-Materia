using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PutToGrid : MonoBehaviour
{
    public Tilemap ground;
    public Vector3 position;

    private void Awake()
    {
        transform.position = ground.CellToWorld(Vector3Int.FloorToInt(position));
    }
}
