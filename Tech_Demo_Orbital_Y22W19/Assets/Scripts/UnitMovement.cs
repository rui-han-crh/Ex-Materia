using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class UnitMovement
{
    private Unit currentTurnUnit;
    private GameMapData mapData;
    private Dictionary<Vector3Int, int> reachableTiles;

    public Dictionary<Vector3Int, int> ReachableTiles => reachableTiles ??= GetReachableTiles();

    // CONSTRUCTORS
    public UnitMovement(Unit currentTurnUnit, GameMapData mapData)
    {
        this.currentTurnUnit = currentTurnUnit;
        this.mapData = mapData;
    }

    // PUBLIC STATIC METHODS

    // PRIVATE STATIC METHODS

    // PUBLIC METHODS
    public bool QueryMoveFeasibility(Vector3Int targetPosition)
    {
        return ReachableTiles.ContainsKey(targetPosition);
    }

    public Dictionary<Vector3Int, int> GetReachableTiles()
    {
        return Pathfinder2D.GetAllReachableTiles(currentTurnUnit, mapData);
    }

    public IEnumerable<MovementRequest> GetAllMovementsPossible(GameMap gameMapRequesting)
    {
        Vector3Int currentUnitPosition = gameMapRequesting.GetPositionByUnit(currentTurnUnit);
        return GetReachableTiles().Select(kvp => new MovementRequest(gameMapRequesting,
                                                                    mapData.GetPositionByUnit(currentTurnUnit), 
                                                                    kvp.Key, 
                                                                    kvp.Value));
    }

    // PRIVATE METHODS
}