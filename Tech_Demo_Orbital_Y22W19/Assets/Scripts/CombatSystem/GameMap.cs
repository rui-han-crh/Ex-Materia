using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Entities;
using System.Linq;
using CombatSystem.Consultants;
using CombatSystem.Censuses;
using DataStructures;

public class GameMap
{
    private readonly MapActionRequest lastAction;
    private readonly GameMapData gameMapData;
    private readonly Unit currentActingUnit;

    public static GameMap MakeNewMap(GameMapData gameMapData)
    {
        return new GameMap(gameMapData, null);
    }

    public static GameMap MakeNewMap(UnitCensus unitCensus, TileCensus tileCensus)
    {
        return new GameMap(new GameMapData(unitCensus, tileCensus), null);
    }

    private GameMap(GameMap oldMap)
    {
        lastAction = oldMap.lastAction;
        gameMapData = oldMap.gameMapData;
        currentActingUnit = oldMap.currentActingUnit;
    }

    private GameMap(GameMapData data, MapActionRequest lastAction)
    {
        this.lastAction = lastAction;

        this.gameMapData = data.CleanUnitStatusEffects(data.GetUnitWithMinimumTime());
        this.currentActingUnit = gameMapData.GetUnitWithMinimumTime();
    }

    public Vector3Int this[Unit unit]
    {
        get => gameMapData[unit];
    }

    public Unit this[Vector3Int position]
    {
        get => gameMapData[position];
    }

    public Unit CurrentActingUnit => currentActingUnit;

    public GameMapData Data => gameMapData;

    public GameMap DoAction(MapActionRequest request)
    {
        GameMapData newData;
        switch (request.Type)
        {
            case MapActionRequest.RequestType.Attack:
                newData = gameMapData.AttackUnit((AttackRequest)request);
                break;

            case MapActionRequest.RequestType.Movement:
                newData = gameMapData.MoveUnit((MovementRequest)request);
                break;

            case MapActionRequest.RequestType.Wait:
                newData = gameMapData.WaitUnit((WaitRequest)request);
                break;

            case MapActionRequest.RequestType.Overwatch:
                newData = gameMapData.OverwatchUnit((OverwatchRequest)request);
                break;

            case MapActionRequest.RequestType.Skill:
                newData = gameMapData.UseSkill((UseSkillRequest)request);
                break;

            default:
                throw new InvalidOperationException(
                    $"The operation for type {request.Type} is not supported by DoAction in GameMap");
        }

        return new GameMap(newData, request);
    }

    public IEnumerable<Unit> GetUnitsOfFaction(Unit.UnitFaction faction)
    {
        return gameMapData.UnitsInPlay.Where(unit => unit.Faction.Equals(faction));
    }

    public IEnumerable<Unit> GetUnits(Predicate<Unit> predicate)
    {
        return gameMapData.UnitsInPlay.Where(unit => predicate(unit));
    }

    public Unit GetSimilarUnit(Unit unit)
    {
        return gameMapData.GetSimilarUnit(unit);
    }

    public bool HasTile(Vector3Int position)
    {
        return gameMapData.HasTile(position);
    }

    public bool ContainsUnit(Unit unit)
    {
        return gameMapData.ContainsUnit(unit);
    }

    public GameMap ClearOffDeadUnits()
    {
        return new GameMap(gameMapData.ClearDeadUnits(), lastAction);
    }

    public bool IsWon()
    {
        return gameMapData.IsWon();
    }

    public bool IsLost()
    {
        return gameMapData.IsLost();
    }

    public MapActionRequest GetKthBestAction(int k)
    {
        IEnumerable<MovementRequest> movementRequests = MovementConsultant.GetAllMovements(gameMapData, currentActingUnit);
        List<MapActionRequest> actions = new List<MapActionRequest>(CombatConsultant.GetAllAttacks(gameMapData, currentActingUnit));
        actions.AddRange(new MapActionRequest[] { new WaitRequest(currentActingUnit, 75, 75) });
        actions.AddRange(new MapActionRequest[] { new OverwatchRequest(currentActingUnit) });


        if (movementRequests.Count() > 0)
        {

            int maximumMovementRating = movementRequests.Max(x => x.GetUtility(this));
            int stationarySafetyRating = MovementConsultant.GetRemainStationaryRating(this, currentActingUnit);

            if (maximumMovementRating > stationarySafetyRating)
            {
                Debug.Log($"Moving is a valid decision in this stage, movement rating: {maximumMovementRating} | stationary rating : {stationarySafetyRating}");
                actions.AddRange(movementRequests);
            }
        }

        Debug.Log($"Actions consolidated, {actions.Count()} actions");

        List<MapActionRequest> actionsList = actions.ToList();

        actionsList.Sort((x, y) =>
        {
            Debug.Assert(x != null, $"{x} is null");
            float xUtility = x.GetUtility(this);
            float yUtility = y.GetUtility(this);

            if ((x.Type == y.Type)
                || (x.Type == MapActionRequest.RequestType.Overwatch && y.Type == MapActionRequest.RequestType.Movement)
                || (x.Type == MapActionRequest.RequestType.Movement && y.Type == MapActionRequest.RequestType.Overwatch))
            {
                if (xUtility != yUtility)
                {
                    return (int)Mathf.Sign(xUtility - yUtility);
                }

                return y.ActionPointCost - x.ActionPointCost;
            }
            else
            {
                if (x.Type == MapActionRequest.RequestType.Attack || y.Type == MapActionRequest.RequestType.Attack)
                {
                    if (x.Type == MapActionRequest.RequestType.Attack && -EvaluatePositionSafetyOf(x.ActingUnit) <= x.ActingUnit.Risk)
                    {
                        return 1;
                    }
                    else if (y.Type == MapActionRequest.RequestType.Attack && -EvaluatePositionSafetyOf(y.ActingUnit) <= y.ActingUnit.Risk)
                    {
                        return -1;
                    }
                }
                else if (x.ActingUnit.CurrentActionPoints < CombatConsultant.ATTACK_COST)
                {
                    return x.Type == MapActionRequest.RequestType.Wait ? 1 : -1;
                }

                return x.Type - y.Type;
            }
        });

        Debug.Log(string.Join(", ", actionsList.Select(x => $"{x}: {x.GetUtility(this)}")));
        return actionsList.Last();
    }

    public int EvaluatePositionSafetyOf(Unit unit)
    {
        if (!gameMapData.ContainsUnit(unit))
        {
            return 0;
        }

        IEnumerable<Unit> rivalUnits = gameMapData.UnitsInPlay.Where(other => !other.Faction.Equals(unit.Faction));

        int safety = 0;
        foreach (Unit rival in rivalUnits)
        {
            AttackRequest attackRequest = CombatConsultant.SimulateAttack(rival, unit, gameMapData);
            if (attackRequest.Successful)
            {
                safety -= Mathf.Max(CombatConsultant.MINIMUM_DAMAGE_DEALT, (int)(attackRequest.PotentialDamageDealt * attackRequest.ChanceToHit));
            }
        }

        return safety;
    }

    public int FindCostToNearestRival(Unit unit)
    {
        IEnumerable<Unit> rivalUnits = gameMapData.UnitsInPlay.Where(other => !other.Faction.Equals(unit.Faction));

        if (rivalUnits.Any(rival => Vector3Int.Distance(this[unit], this[rival]) < unit.Range))
        {
            return 0;
        }

        //return (int)rivalUnits.Select(rival => Vector3Int.Distance(this[unit], this[rival])).Min();

        int cost = int.MaxValue;
        IUndirectedGraph<Vector3Int> graph = gameMapData.ToUndirectedGraph(
            rivalUnits.Select(x => this[x]).Concat(new Vector3Int[] { this[unit] }));
        
        foreach (Unit rival in rivalUnits) {
            IEnumerable<Vector3Int> shortestPath = MovementConsultant.FindShortestPath(
                gameMapData[unit], 
                gameMapData[rival], 
                gameMapData, 
                graph);

            cost = Mathf.Min(cost, MovementConsultant.GetPathCost(shortestPath, gameMapData));
        }
        return cost;
    }
}
