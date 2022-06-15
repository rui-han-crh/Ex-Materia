using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;
using System;
using Tests;
using DataStructures;
using System.Linq;
using CombatSystem.Entities;
using CombatSystem.Consultants;
using CombatSystem.Censuses;

namespace Tests.PlayMap
{
    public class GameMapTest
    {
        [Test]
        public void CorrectlyFindsCostToNearestRival() {
            GameMap gameMap = Warehouse.MakeGameMapA();
            Assert.AreEqual(66, gameMap.FindCostToNearestRival(gameMap.CurrentActingUnit));
        }

        [Test]
        public void CorrectlyEvaluatesPositionSafety()
        {
            Unit.ResetClass();

            TileCensus tileCensus = Warehouse.TILE_CENSUS_DEFINED_PATH_5X5;

            Unit unitA = Warehouse.MakeUnitA();
            Unit unitB = Warehouse.MakeUnitB();

            UnitCensus unitCensus = new UnitCensus(new Dictionary<Vector3Int, Unit>()
            {
                [new Vector3Int(0, 0, 0)] = unitA,
                [new Vector3Int(1, 4, 0)] = unitB
            }
            );

            GameMapData gameMapData = new GameMapData(unitCensus, tileCensus);
            GameMap gameMap = GameMap.MakeNewMap(gameMapData);

            Assert.AreEqual(-5, gameMap.EvaluatePositionSafetyOf(gameMap.CurrentActingUnit));
            Assert.AreEqual(-30, gameMap.EvaluatePositionSafetyOf(unitB));
        }

        [Test]
        public void CorrectlyGetsUnitOfFaction()
        {
            Unit.ResetClass();

            TileCensus tileCensus = Warehouse.TILE_CENSUS_DEFINED_PATH_5X5;

            Unit unitA = Warehouse.MakeUnitA();
            Unit unitB = Warehouse.MakeUnitB();
            Unit unitC = Warehouse.MakeUnitC();

            UnitCensus unitCensus = new UnitCensus(new Dictionary<Vector3Int, Unit>()
            {
                [new Vector3Int(0, 0, 0)] = unitA,
                [new Vector3Int(1, 4, 0)] = unitB,
                [new Vector3Int(4, 2, 0)] = unitC,
            }
            );

            GameMapData gameMapData = new GameMapData(unitCensus, tileCensus);
            GameMap gameMap = GameMap.MakeNewMap(gameMapData);

            CollectionAssert.AreEquivalent(new Unit[] { unitB }, gameMap.GetUnitsOfFaction(Unit.UnitFaction.Enemy));
            CollectionAssert.AreEquivalent(new Unit[] { unitA, unitC }, gameMap.GetUnitsOfFaction(Unit.UnitFaction.Friendly));
        }
    }
}
