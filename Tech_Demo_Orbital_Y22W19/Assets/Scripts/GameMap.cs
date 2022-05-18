using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// An immutable data structure that comprises of map data, such as the unit positions,
/// cover positions and other systems pertaining to the map
/// </summary>
public struct GameMap
{

    public static readonly Vector3 UNIT_GRID_OFFSET = Vector2.one / 2;

    private readonly UnitCombat unitCombat;
    private readonly UnitMovement unitMovement;
    private readonly GameMapData mapData;

    private readonly Unit currentTurnUnit;

    private readonly MapActionRequest lastAction;


    public MapActionRequest LastAction => lastAction;

    public Unit CurrentUnit => currentTurnUnit;
    public Vector3Int CurrentUnitPosition => mapData.UnitPositionMapping[currentTurnUnit];

    public HashSet<Vector3Int> AllUnitPositions => new HashSet<Vector3Int> (mapData.PositionUnitMapping.Keys);

    // CONSTRUCTORS

    // Creates a new map from scratch
    public GameMap( Dictionary<Vector3Int, Unit> positionUnitMap, 
                IEnumerable<Vector3Int> fullCoverPositions,
                IEnumerable<Vector3Int> halfCoverPositions,
                IEnumerable<Vector3Int> groundPositions,
                Dictionary<Vector3Int, int> groundTileCosts) 
    {
        this.currentTurnUnit = GetUnitWithLeastExhaustion(positionUnitMap.Values);
        this.lastAction = null;

        mapData = new GameMapData(positionUnitMap, fullCoverPositions, halfCoverPositions, groundPositions, groundTileCosts);
        unitCombat = new UnitCombat(currentTurnUnit, mapData);
        unitMovement = new UnitMovement(currentTurnUnit, mapData);
    }

    // Clones a new map from the oldmap with new updates units
    private GameMap(GameMap oldMap,
                Dictionary<Vector3Int, Unit> positionUnitMap, 
                MapActionRequest lastAction)
    {
        this.currentTurnUnit = GetUnitWithLeastExhaustion(positionUnitMap.Values);
        this.lastAction = lastAction;

        mapData = new GameMapData(positionUnitMap, oldMap.mapData);
        unitCombat = new UnitCombat(currentTurnUnit, mapData);
        unitMovement = new UnitMovement(currentTurnUnit, mapData);

    }

    // STATIC METHODS
    private static Unit GetUnitWithLeastExhaustion(IEnumerable<Unit> units)
    {
        if (units.Count() == 0)
        {
            throw new System.Exception("There are no units");
        }

        Unit leastExhaustedUnit = units.First();
        foreach (Unit unit in units)
        {
            if (unit.Health > 0 && (unit.Exhaustion < leastExhaustedUnit.Exhaustion))
            {
                leastExhaustedUnit = unit;
            }
        }
        return leastExhaustedUnit;
    }

    // PUBLIC METHODS

    public Dictionary<Vector3Int, int> GetReachableTiles()
    {
        return unitMovement.GetReachableTiles();
    }

    public HashSet<Vector3Int> GetAttackablePositions()
    {
        return unitCombat.GetAllPositionsAttackable(this);
    }

    public IEnumerable<AttackRequest> GetAllAttackRequestsPossible()
    {
        return unitCombat.GetAllPossibleAttacks(this);
    }

    public IEnumerable<MovementRequest> GetAllMovementRequestsPossible()
    {
        return unitMovement.GetAllMovementsPossible(this);
    }

    public AttackRequest IsAttackableAt(Vector3Int position)
    {
        if (!mapData.PositionUnitMapping.ContainsKey(position))
        {
            return new AttackRequest(this, 
                                    CurrentUnitPosition,
                                    position, 
                                    AttackStatus.IllegalTarget,
                                    new Vector3Int[] { CurrentUnitPosition, position },
                                    UnitCombat.ATTACK_COST);
        }
        return unitCombat.QueryTargetAttackable(this, GetUnitByPosition(position));
    }

    public HashSet<Vector3Int> GetRange()
    {
        return unitCombat.GetAllPositionsInRange();
    }

    public Vector3Int GetPositionByUnit(Unit unit)
    {
        return mapData.GetPositionByUnit(unit);
    }

    public Unit GetUnitByPosition(Vector3Int position)
    {
        return mapData.GetUnitByPosition(position);
    }

    public bool ExistsUnitAt(Vector3Int position)
    {
        return mapData.ExistsUnitAt(position);
    }

    public bool HasFullCoverAt(Vector3Int position)
    {
        return mapData.HasFullCoverAt(position);
    }

    public bool HasHalfCoverAt(Vector3Int position)
    {
        return mapData.HasHalfCoverAt(position);
    }

    public bool IsWalkableOn(Vector3Int position)
    {
        return mapData.IsWalkableOn(position);
    }

    public int GetTileCost(Vector3Int position)
    {
        return mapData.GetTileCost(position);
    }

    public int FindShortestPathTo(Vector3Int destination, out IEnumerable<Vector3Int> path)
    {
        Pathfinder2D pathfinder = new Pathfinder2D(currentTurnUnit, GetPositionByUnit(currentTurnUnit), destination, mapData);
        path = pathfinder.FindDirectedPath().Select(x => x.Coordinates);
        return pathfinder.PathCost;
    }

    public bool IsGameOver()
    {
        // Either there is no unit who is an enemy, or there is no unit who is a friendly
        return mapData.UnitPositionMapping.Keys.Where(unit => unit.Faction == Faction.Enemy).All(unit => unit.Health == 0)
            || mapData.UnitPositionMapping.Keys.Where(unit => unit.Faction == Faction.Friendly).All(unit => unit.Health == 0);
    }

    public AttackRequest QueryAttackability(Vector3Int offensivePosition, Vector3Int defensivePosition, int range)
    {
        return UnitCombat.QueryTargetAttackable(this, mapData, offensivePosition, defensivePosition, range);
    }

    public int Evaluate()
    {
        IEnumerable<Unit> friendlyUnits = mapData.UnitPositionMapping
                                            .Keys
                                            .Where(x => x.Faction == Faction.Friendly);
        IEnumerable<Unit> enemyUnits = mapData.UnitPositionMapping
                                            .Keys
                                            .Where(x => x.Faction == Faction.Enemy);
        if (IsGameOver())
        {
            return friendlyUnits.Where(x => x.Health > 0).Count() == 0 ? int.MinValue : int.MaxValue;
        }
        return friendlyUnits.Sum(x => x.Health) / friendlyUnits.Count()
                - enemyUnits.Sum(x => x.Health) / enemyUnits.Count();
    }

    public GameMap DoAction(MapActionRequest action)
    {
        if (action.ActionType == MapActionType.Failed)
        {
            return this;
        }

        Dictionary<Vector3Int, Unit> newPositionUnitMap = new Dictionary<Vector3Int, Unit>();

        foreach (Unit unit in mapData.UnitPositionMapping.Keys)
        {
            Unit clonedUnit = unit.Clone();
            newPositionUnitMap[mapData.UnitPositionMapping[unit]] = clonedUnit;
        }

        switch (action.ActionType)
        {
            case MapActionType.Attack:
                AttackRequest attackRequest = (AttackRequest)action;

                Unit unitAttacked = newPositionUnitMap[attackRequest.TargetPosition];

                if (Random.Range(0, 1.0f) <= attackRequest.ChanceToHit)
                {
                    unitAttacked = unitAttacked.DecreaseHealth(attackRequest.ActingUnit.Attack - unitAttacked.Defence);
                    Debug.Log($"Attack was successful {unitAttacked}");
                    newPositionUnitMap[attackRequest.TargetPosition] = unitAttacked;
                } else
                {
                    Debug.Log("Attack failed");
                }

                Unit newMapAttackingUnit = newPositionUnitMap[attackRequest.ActingUnitPosition];

                newMapAttackingUnit = newMapAttackingUnit.UseActionPoints(attackRequest.ActionPointCost);

                newPositionUnitMap[attackRequest.ActingUnitPosition] = newMapAttackingUnit;

                break;

            case MapActionType.Movement:
                MovementRequest movementRequest = (MovementRequest)action;
                Unit newMapMovingUnit = newPositionUnitMap[movementRequest.ActingUnitPosition];

                newMapMovingUnit = newMapMovingUnit.UseActionPoints(movementRequest.ActionPointCost);

                newPositionUnitMap.Remove(movementRequest.ActingUnitPosition);
                newPositionUnitMap[movementRequest.DestinationPosition] = newMapMovingUnit;
                break;

            case MapActionType.Wait:
                WaitRequest waitRequest = (WaitRequest)action;

                Unit recoveringUnit = newPositionUnitMap[waitRequest.ActingUnitPosition];

                recoveringUnit = recoveringUnit.ResetActionPoints().AddTime(WaitRequest.TIME_CONSUMED);

                newPositionUnitMap.Remove(waitRequest.ActingUnitPosition);
                newPositionUnitMap[waitRequest.ActingUnitPosition] = recoveringUnit;
                break;
        }

        Unit currentTurnUnit = GetUnitWithLeastExhaustion(newPositionUnitMap.Values);

        return new GameMap(this, newPositionUnitMap, action);
    }

    public IEnumerable<MapActionRequest> GetOrderedMapActions()
    {

        IEnumerable<MovementRequest> movementRequests = GetAllMovementRequestsPossible();
        IEnumerable<AttackRequest> attacks = GetAllAttackRequestsPossible();

        List<MapActionRequest> requests = movementRequests.Concat<MapActionRequest>(attacks).ToList();

        requests.Add(new WaitRequest(this, GetPositionByUnit(currentTurnUnit)));

        requests.Sort((x, y) => {
            Debug.Assert(x != null, $"{x} is null");
            if (x.ActionType == y.ActionType)
            {
                float xUtility = x.GetUtility();
                float yUtility = y.GetUtility();

                if (xUtility != yUtility)
                {
                    return (int)Mathf.Sign(xUtility - yUtility);
                }
                else if (x.ActionType == MapActionType.Movement)
                {
                    return (int)Mathf.Sign(((MovementRequest)x).GetAttackRating() - ((MovementRequest)y).GetAttackRating());
                }
                else
                {
                    return y.ActionPointCost - x.ActionPointCost;
                }
            }
            else
            {
                if (x.ActionType == MapActionType.Attack && x.PreviousMap.EvaluateCurrentUnitPosition() >= x.ActingUnit.Risk)
                {
                    return 1;
                }
                else if (y.ActionType == MapActionType.Attack && y.PreviousMap.EvaluateCurrentUnitPosition() >= y.ActingUnit.Risk)
                {
                    return -1;
                }
                else
                {
                    return x.ActionType - y.ActionType;
                }
            }
        });
        Debug.Assert(requests.Count > 0, "There are no actions");

        return requests;
    }

    public override string ToString()
    {
        return $"GameMap | {currentTurnUnit.Name}'s turn | {string.Join(", ", mapData.UnitPositionMapping)}";
    }

    public override bool Equals(object obj)
    {
        if (!(obj is GameMap))
        {
            return false;
        } else
        {
            GameMap otherMap = (GameMap)obj;
            return mapData.Equals(otherMap.mapData);
        }
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + unitCombat.GetHashCode();
            hash = hash * 23 + unitMovement.GetHashCode();
            hash = hash * 23 + mapData.GetHashCode();
            hash = hash * 23 + currentTurnUnit.GetHashCode();
            hash = hash * 23 + lastAction.GetHashCode();
            return hash;
        }
    }


    // PRIVATE METHODS

    public float EvaluateCurrentUnitPosition()
    {
        List<Vector3Int> allRivalPositions = new List<Vector3Int>();
        foreach (Vector3Int rivalPosition in AllUnitPositions) 
        {
            if (GetUnitByPosition(rivalPosition).Faction != currentTurnUnit.Faction)
            {
                allRivalPositions.Add(rivalPosition);
            } 
        }

        float rivalAttackRating = 0;

        foreach (Vector3Int rivalPosition in allRivalPositions)
        {
            Unit rivalUnit = GetUnitByPosition(rivalPosition);

            AttackRequest hypotheticalRequest = QueryAttackability(rivalPosition, CurrentUnitPosition, rivalUnit.Range * 2);
            if (hypotheticalRequest.Successful)
            {
                rivalAttackRating += hypotheticalRequest.ChanceToHit * rivalUnit.Attack;
            }
        }

        return currentTurnUnit.Defence - rivalAttackRating;
    }
}