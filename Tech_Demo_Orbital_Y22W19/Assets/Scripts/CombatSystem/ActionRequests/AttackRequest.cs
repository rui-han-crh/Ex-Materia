using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Entities;
using System.Linq;

public class AttackRequest : MapActionRequest
{
    public enum Outcome
    {
        Successful,
        NoLineOfSight,
        NotEnoughActionPoints,
        OutOfRange,
        Pending
    }

    private Outcome outcome;

    private Unit targetUnit;

    private int potentialDamageDealt;

    private IEnumerable<Vector3Int> tilesHit;

    private float chanceToHit;

    public bool Successful => outcome == Outcome.Successful;

    public Unit TargetUnit => targetUnit;

    public Vector3Int SourcePosition => tilesHit.First();


    public int PotentialDamageDealt => potentialDamageDealt;

    public float ChanceToHit => chanceToHit;

    public IEnumerable<Vector3Int> TilesHit => tilesHit;

    public AttackRequest(
        Unit actingUnit, 
        Unit defendingUnit, 
        int potentialDamageDealt,
        float chanceToHit,
        IEnumerable<Vector3Int> lineOfSightPositions, 
        int actionPointCost, 
        int timeSpent, 
        Outcome outcome)
        : base(actingUnit, actionPointCost, timeSpent, RequestType.Attack)
    {
        this.targetUnit = defendingUnit;
        this.potentialDamageDealt = potentialDamageDealt;
        this.tilesHit = lineOfSightPositions;
        this.outcome = outcome;
        this.chanceToHit = chanceToHit;
    }

    public AttackRequest(
        Unit actingUnit,
        Unit defendingUnit,
        int potentialDamageDealt,
        float chanceToHit,
        int actionPointCost,
        int timeSpent,
        Outcome outcome)
        : base(actingUnit, actionPointCost, timeSpent, RequestType.Attack)
    {
        this.targetUnit = defendingUnit;
        this.potentialDamageDealt = potentialDamageDealt;
        this.tilesHit = new Vector3Int[0];
        this.outcome = outcome;
        this.chanceToHit = chanceToHit;
    }

    private AttackRequest(Unit attacker, Unit defender, IEnumerable<Vector3Int> tilesHit, Outcome reason) : base(attacker, 0, 0, RequestType.Attack)
    {
        this.targetUnit = defender;
        this.potentialDamageDealt = 0;
        this.tilesHit = tilesHit;
        this.outcome = reason;
        this.chanceToHit = 0;
    }

    public static AttackRequest CreateFailedRequest(Unit attacker, Unit defender, IEnumerable<Vector3Int> tilesHit, Outcome reason)
    {
        return new AttackRequest(attacker, defender, tilesHit, reason);
    }

    public static AttackRequest CreateFailedRequest(Unit attacker, Unit defender, Outcome reason)
    {
        return new AttackRequest(attacker, defender, new Vector3Int[0], reason);
    }

    public static AttackRequest CreatePendingRequest(Unit attacker, Unit defender, IEnumerable<Vector3Int> tilesHit, int actionPointCost, int timeSpent)
    {
        return new AttackRequest(attacker, defender, 0, 0, tilesHit, actionPointCost, timeSpent, Outcome.Pending);
    }

    public override int GetUtility(GameMap map)
    {
        return (int)((chanceToHit * ActingUnit.Attack) - targetUnit.Defence);
    }
}
