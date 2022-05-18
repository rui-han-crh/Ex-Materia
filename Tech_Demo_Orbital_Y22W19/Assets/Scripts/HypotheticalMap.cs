//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class HypotheticalMap : IUnitManager
//{
//    private HypotheticalMap[] orderedMoves;
//    private Dictionary<Vector3Int, Unit> playerUnits;
//    private Unit[] enemyUnits;
//    private Unit currentTurnUnit;

//    private UnitCombatOld unitCombat;
//    private UnitMovementOld unitMovement;

//    public HypotheticalMap(Unit currentTurnUnit, Unit[] playerUnits, Unit[] enemyUnits)
//    {
//        this.playerUnits = playerUnits;
//        this.enemyUnits = enemyUnits;
//        this.currentTurnUnit = currentTurnUnit;
//        this.unitMovement = new UnitMovement();
//        this.unitCombat = new UnitCombat();

//        this.orderedMoves = CreateAllPossibleMoves();
//    }

//    private HypotheticalMap[] CreateAllPossibleMoves()
//    {
        
//    }

//    public HypotheticalMap[] OrderedMoves => orderedMoves;

//    public EvaluatedMap Evaluate()
//    {

//    }

//    public bool IsGameOver()
//    {
//        return playerUnits.Length == 0 || enemyUnits.Length == 0;
//    }

//    public Unit GetSelectedUnit()
//    {
//        return currentTurnUnit;
//    }

//    public Dictionary<Vector3Int, Unit> GetPositionsOfUnits()
//    {
//        return new 
//    }

//    public bool IsOverUI()
//    {
//        throw new NotImplementedException();
//    }

//    public bool IsRoutineEmpty()
//    {
//        throw new NotImplementedException();
//    }

//    public Vector3Int GetPositionByUnit(Unit unit)
//    {
//        throw new NotImplementedException();
//    }

//    public Unit GetUnitByPosition(Vector3Int position)
//    {
//        throw new NotImplementedException();
//    }

//    public void UpdateUnitPosition(Unit unit, Vector3Int position)
//    {
//        throw new NotImplementedException();
//    }

//    public void Enqueue(IEnumerator routine)
//    {
//        throw new NotImplementedException();
//    }
//}