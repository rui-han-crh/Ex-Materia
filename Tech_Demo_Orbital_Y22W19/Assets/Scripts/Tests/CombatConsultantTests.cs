using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Entities;
using System.Reflection;
using System;
using Censuses;
using Tests;
using Requests;
using Consultants;
using System.Linq;

namespace Tests.Consultants
{
    public class CombatConsultantTests
    {
        [Test]
        public void CorrectlySimulatesAttackProperties()
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

            AttackRequest attackRequest = CombatConsultant.SimulateAttack(unitA, unitB, gameMapData);

            Assert.IsTrue(attackRequest.Successful);
            Assert.AreEqual(unitA.Attack - unitB.Defence, attackRequest.PotentialDamageDealt);
            CollectionAssert.AreEqual(new Vector3Int[]
            {
                new Vector3Int(0, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, 2, 0),
                new Vector3Int(1, 2, 0),
                new Vector3Int(1, 3, 0),
                new Vector3Int(1, 4, 0)
            }, attackRequest.TilesHit);
        }

        [Test]
        public void CorrectlyRetrievesAttackRequests()
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
            IEnumerable<MapActionRequest> requests = CombatConsultant.GetAllAttacks(gameMapData, unitA);

            Assert.AreEqual(1, requests.Count());
            AttackRequest attackRequest = (AttackRequest)requests.First();

            Assert.AreEqual(unitA, attackRequest.ActingUnit);
        }
    }
}
