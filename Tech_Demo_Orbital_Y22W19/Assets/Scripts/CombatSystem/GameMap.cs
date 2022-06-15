using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Entities;
using System.Linq;
using CombatSystem.Consultants;
using Algorithms.ShortestPathSearch;
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
        this.gameMapData = data;
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
        switch (request.Type)
        {
            case MapActionRequest.RequestType.Attack:
                return new GameMap(gameMapData.AttackUnit((AttackRequest)request), request);

            case MapActionRequest.RequestType.Movement:
                return new GameMap(gameMapData.MoveUnit((MovementRequest)request), request);

            case MapActionRequest.RequestType.Wait:
                return new GameMap(gameMapData.WaitUnit((WaitRequest)request), request);

            case MapActionRequest.RequestType.Overwatch:
                return new GameMap(gameMapData.OverwatchUnit((OverwatchRequest)request), request);

            default:
                throw new InvalidOperationException(
                    $"The operation for type {request.Type} is not supported by DoAction in GameMap");
        }
    }

    public IEnumerable<Unit> GetUnitsOfFaction(Unit.UnitFaction faction)
    {
        return gameMapData.UnitsInPlay.Where(unit => unit.Faction.Equals(faction));
    }

    public IEnumerable<Unit> GetUnits(Predicate<Unit> predicate)
    {
        return gameMapData.UnitsInPlay.Where(unit => predicate(unit));
    }

    public bool HasTile(Vector3Int position)
    {
        return gameMapData.HasTile(position);
    }

    public MapActionRequest GetKthBestAction(int k)
    {
        IEnumerable<MapActionRequest> actions = CombatConsultant.GetAllAttacks(gameMapData, currentActingUnit)
            .Concat(MovementConsultant.GetAllMovements(gameMapData, currentActingUnit)
            .Concat(new MapActionRequest[] { new WaitRequest(currentActingUnit, 75, 75) })
            .Concat(new MapActionRequest[] { new OverwatchRequest(currentActingUnit) }));

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
                    return (int)(xUtility - yUtility);
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
                else if (x.ActingUnit.CurrentActionPoints < CombatConsultant.ATTACK_AP_COST)
                {
                    return x.Type == MapActionRequest.RequestType.Wait ? 1 : -1;
                }

                return x.Type - y.Type;
            }
        });

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

        return (int)rivalUnits.Select(rival => Vector3Int.Distance(this[unit], this[rival])).Min();

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
