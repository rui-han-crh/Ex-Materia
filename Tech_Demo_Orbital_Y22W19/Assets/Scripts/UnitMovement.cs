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
        return Pathfinder2D.GetAllReachablePositions(currentTurnUnit, mapData);
    }

    public Pathfinder2D.ShortestPathTree GetShortestPaths()
    {
        return Pathfinder2D.GetShortestPathTree(currentTurnUnit, mapData);
    }

    public IEnumerable<MovementRequest> GetAllMovementsPossible(GameMap gameMapRequesting)
    {
        Vector3Int currentUnitPosition = gameMapRequesting.GetPositionByUnit(currentTurnUnit);
        Pathfinder2D.ShortestPathTree shortestPathTree = GetShortestPaths();
        return shortestPathTree.Children.Select(child => new MovementRequest(gameMapRequesting,
                                                                            mapData.GetPositionByUnit(currentTurnUnit),
                                                                            shortestPathTree.GetShortestPathToPosition(child),
                                                                            shortestPathTree.GetCostToPosition(child)));
    }

    // PRIVATE METHODS
}