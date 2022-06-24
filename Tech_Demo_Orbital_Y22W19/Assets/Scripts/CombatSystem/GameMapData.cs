using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Censuses;
using CombatSystem.Entities;
using System;
using DataStructures;
using Algorithms;
using System.Linq;
using UnityEngine.Extensions;

public class GameMapData
{
    private readonly UnitCensus unitCensus;
    private readonly TileCensus tileCensus;

    public int UnitCount => unitCensus.Count;

    public IEnumerable<Unit> UnitsInPlay => unitCensus.Census;


    public GameMapData(UnitCensus unitCensus, TileCensus tileCensus)
    {
        this.unitCensus = unitCensus;
        this.tileCensus = tileCensus;
    }

    private GameMapData(GameMapData data)
    {
        this.unitCensus = data.unitCensus.Clone();
        this.tileCensus = data.tileCensus.Clone();
    }

    public Vector3Int this[Unit unit]
    {
        get
        {
            return unitCensus[unit];
        }
    }

    public Unit this[Vector3Int position]
    {
        get
        {
            return unitCensus[position];
        }
    }

    public int GetTileCost(Vector3Int position)
    {
        return tileCensus[position].Cost;
    }

    public bool HasFullCover(Vector3Int position)
    {
        return tileCensus.GetTileType(position).Equals(TileData.TileType.FullCover);
    }

    public bool HasHalfCover(Vector3Int position)
    {
        return tileCensus.GetTileType(position).Equals(TileData.TileType.HalfCover);
    }

    public bool HasUnitAt(Vector3Int position)
    {
        return unitCensus.Contains(position);
    }

    public bool IsVacant(Vector3Int position)
    {
        return !unitCensus.Contains(position) && tileCensus.GetTileType(position).Equals(TileData.TileType.Ground);
    }

    public bool HasTile(Vector3Int position)
    {
        return tileCensus.Contains(position);
    }

    public GameMapData CleanUnitStatusEffects(Unit unit)
    {
        unit = GetSimilarUnit(unit).CleanStatusEffects();
        return new GameMapData(unitCensus.UpdateUnit(unit), tileCensus);
    }

    public GameMapData ChangeUnitPosition(Unit unit, Vector3Int position)
    {
        if (!unitCensus.Contains(unit))
        {
            throw new ArgumentException($"Cannot move {unit.Name} as it does not exist in data");
        }

        if (!tileCensus.Contains(position))
        {
            throw new ArgumentException($"Cannot move {unit.Name} to {position} as {position} does not have any tiles");
        }

        UnitCensus newUnitCensus = unitCensus.MoveUnit(unit, position);
        return new GameMapData(newUnitCensus, tileCensus);
    }

    public GameMapData ClearDeadUnits()
    {
        return new GameMapData(unitCensus.FilterUnits(x => x.CurrentHealth > 0), tileCensus);
    }

    public bool IsWon()
    {
        return unitCensus.FilterUnits(x => x.Faction == Unit.UnitFaction.Enemy).Count == 0;
    }

    public bool IsLost()
    {
        return unitCensus.FilterUnits(x => x.Faction == Unit.UnitFaction.Friendly).Count == 0;
    }


    public Unit GetSimilarUnit(Unit unit)
    {
        return this[this[unit]];
    }

    public GameMapData MoveUnit(MovementRequest movementRequest)
    {
        Vector3Int destination = movementRequest.Destination;
        if (!IsVacant(destination))
        {
            throw new ArgumentException($"The position {destination} was not vacant to move to.");
        }

        Unit movingUnit = GetSimilarUnit(movementRequest.ActingUnit);

        movingUnit = movingUnit
            .ChangeTime(movingUnit.Time + movementRequest.TimeSpent)
            .RemoveStatusEffect("Overwatch");

        //.ChangeActionPoints(movingUnit.CurrentActionPoints - movementRequest.ActionPointCost)

        return new GameMapData(unitCensus.MoveUnit(movingUnit, destination), tileCensus);
    }

    public GameMapData AttackUnit(AttackRequest attackRequest)
    {
        if (!attackRequest.Successful)
        {
            Debug.Log("Failed");
            return this;
        }

        Unit defendingUnit = GetSimilarUnit(attackRequest.TargetUnit);
        Unit attackingUnit = GetSimilarUnit(attackRequest.ActingUnit);

        Unit resultantDefendingUnit = defendingUnit;
        Unit resultantAttackingUnit = attackingUnit;

        Debug.Log($"Chance to hit: {attackRequest.ChanceToHit}");

        if (UnityEngine.Random.Range(0.0f, 1.0f) <= attackRequest.ChanceToHit)
        {
            Debug.Log($"Attack succeeded {attackingUnit.Name} vs. {defendingUnit.Name} for {attackRequest.PotentialDamageDealt}");

            resultantDefendingUnit = defendingUnit
                .ChangeHealth(defendingUnit.CurrentHealth - attackRequest.PotentialDamageDealt)
                .ChangeSkillPoints(defendingUnit.CurrentSkillPoints + 2);

            resultantAttackingUnit = attackingUnit.ChangeSkillPoints(attackingUnit.CurrentSkillPoints + 3);
        }

        resultantAttackingUnit = resultantAttackingUnit
            .ChangeTime(attackingUnit.Time + attackRequest.TimeSpent)
            .RemoveStatusEffect("Overwatch");

        //.ChangeActionPoints(attackingUnit.CurrentActionPoints - attackRequest.ActionPointCost)

        return new GameMapData(unitCensus.SwapUnit(defendingUnit, resultantDefendingUnit)
                                        .SwapUnit(attackingUnit, resultantAttackingUnit), 
                                        tileCensus);
    }

    public GameMapData WaitUnit(WaitRequest waitRequest)
    {
        Unit actingUnit = GetSimilarUnit(waitRequest.ActingUnit);

        actingUnit = actingUnit.ChangeTime(actingUnit.Time + waitRequest.TimeSpent);
        //  .ChangeActionPoints(actingUnit.CurrentActionPoints + waitRequest.ActionPointsReplenished);

        return new GameMapData(unitCensus.UpdateUnit(actingUnit), tileCensus);
    }

    public GameMapData OverwatchUnit(OverwatchRequest overwatchRequest)
    {
        Unit actingUnit = GetSimilarUnit(overwatchRequest.ActingUnit);
        actingUnit = actingUnit.ApplyStatusEffect("Overwatch", actingUnit.Time);

        actingUnit = actingUnit.ChangeTime(actingUnit.Time + overwatchRequest.TimeSpent);
        return new GameMapData(unitCensus.UpdateUnit(actingUnit), tileCensus);
    }

    public GameMapData UseSkill(UseSkillRequest useSkillRequest)
    {
        Unit actingUnit = GetSimilarUnit(useSkillRequest.ActingUnit);
        actingUnit = actingUnit.ApplyStatusEffect(useSkillRequest.SkillName, actingUnit.Time);

        actingUnit = actingUnit.ChangeTime(actingUnit.Time + useSkillRequest.TimeSpent);
        Debug.Log($"Skill {useSkillRequest.SkillName} was used on {actingUnit}");
        return new GameMapData(unitCensus.UpdateUnit(actingUnit), tileCensus);
    }

    public Unit GetUnitWithMinimumTime()
    {
        return unitCensus.GetUnitWithLeastTime();
    }

    public bool ContainsUnit(Unit unit)
    {
        return unitCensus.Contains(unit);
    }

    public IUndirectedGraph<Vector3Int> ToUndirectedGraph(IEnumerable<Vector3Int> inclusion = null)
    {
        inclusion ??= new Vector3Int[0];

        IUndirectedGraph<Vector3Int> graph = new UndirectedGraph<Vector3Int>();
        HashSet<Vector3Int> unvisited = new HashSet<Vector3Int>(tileCensus.Census.Where(x => IsVacant(x)));

        while (unvisited.Count > 0)
        {
            Vector3Int root = unvisited.First();
            graph.Add(root);
            unvisited.Remove(root);

            SearchAlgorithms.BreadthFirstSearch(root,
                x =>
                    {
                        List<Vector3Int> children = new List<Vector3Int>();
                        for (float angle = 0; angle < Mathf.PI * 2; angle += Mathf.PI / 4)
                        {
                            Vector3Int child = x + Vector3Int.one.Rotate(angle);

                            if (inclusion.Contains(child) || (tileCensus.Contains(child) && IsVacant(child)))
                            {
                                children.Add(child);

                                if (!graph.Contains(child))
                                {
                                    graph.Add(child);
                                    unvisited.Remove(child);
                                }

                                graph.Connect(x, child);
                            }
                        }
                        return children;
                    },
                _ => { }
                );
        }
        return graph;
    }

}
