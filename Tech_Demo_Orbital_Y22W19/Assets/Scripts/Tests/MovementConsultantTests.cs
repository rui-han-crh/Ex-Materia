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

namespace Tests.Consultants
{
    public class MovementConsultantTests
    {

        [Test]
        public void In3x3_RetracesPathCorrectlyWithNoUnits()
        {
            GameMapData gameMapData = Warehouse.GMD_EMPTY_POPTART;
            IEnumerable<Vector3Int> path = MovementConsultant.FindShortestPath(Vector3Int.zero, new Vector3Int(2, 2, 0), gameMapData);

            Vector3Int[] expectedPath = new Vector3Int[]
            {
                new Vector3Int(0, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(1, 2, 0),
                new Vector3Int(2, 2, 0)
            };

            CollectionAssert.AreEqual(expectedPath, path);
        }

        [Test]
        public void In3x3_RetracesPathCorrectlyWithObstructingUnitNearStart()
        {
            GameMapData gameMapData = Warehouse.GMD_POPTART_OCCUPIED_TOP_RIGHT;
            IEnumerable<Vector3Int> path = MovementConsultant.FindShortestPath(Vector3Int.zero, new Vector3Int(2, 2, 0), gameMapData);

            Vector3Int[] expectedPath = new Vector3Int[]
            {
                new Vector3Int(0, 0, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(2, 1, 0),
                new Vector3Int(2, 2, 0)
            };

            CollectionAssert.AreEqual(expectedPath, path);
        }

        [Test]
        public void In3x3_RetracesPathCorrectlyWithObstructingUnitNearEnd()
        {
            GameMapData gameMapData = Warehouse.GMD_POPTART_OCCUPIED_TOP_MIDDLE;
            IEnumerable<Vector3Int> path = MovementConsultant.FindShortestPath(Vector3Int.zero, new Vector3Int(2, 2, 0), gameMapData);

            Vector3Int[] expectedPath = new Vector3Int[]
            {
                new Vector3Int(0, 0, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(2, 1, 0),
                new Vector3Int(2, 2, 0)
            };

            CollectionAssert.AreEqual(expectedPath, path);
        }

        [Test]
        public void In5x5CardinalWithOccupied_RetracesPathCorrectly()
        {
            GameMapData gameMapData = Warehouse.GMD_CARDINAL_5X5_OCCUPIED_120;
            IEnumerable<Vector3Int> path = MovementConsultant.FindShortestPath(Vector3Int.zero, new Vector3Int(4, 4, 0), gameMapData);

            Vector3Int[] expectedPath = new Vector3Int[]
            {
                new Vector3Int(0, 0, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(2, 1, 0),
                new Vector3Int(3, 2, 0),
                new Vector3Int(4, 3, 0),
                new Vector3Int(4, 4, 0)
            };

            CollectionAssert.AreEqual(expectedPath, path);
        }

        [Test]
        public void In5x5DefinedPath_RetracesPathCorrectly()
        {
            GameMapData gameMapData = Warehouse.GMD_DEFINED_PATH_5X5;
            IEnumerable<Vector3Int> path = MovementConsultant.FindShortestPath(Vector3Int.zero, new Vector3Int(1, 4, 0), gameMapData);

            Vector3Int[] expectedPath = new Vector3Int[]
            {
                new Vector3Int(0, 0, 0),
                new Vector3Int(1, 1, 0),
                new Vector3Int(2, 1, 0),
                new Vector3Int(3, 2, 0),
                new Vector3Int(2, 3, 0),
                new Vector3Int(1, 4, 0)
            };

            CollectionAssert.AreEqual(expectedPath, path);
        }

        [Test]
        public void In3x3Poptart_RetrievesMovementRequestsCorrectly()
        {
            GameMapData gameMapData = Warehouse.GMD_POPTART_OCCUPIED_TOP_MIDDLE;
            IEnumerable<MovementRequest> requests = MovementConsultant.GetAllMovements(gameMapData, gameMapData.GetUnitWithMinimumTime());

            IEnumerable<int> movementCosts = requests.Select(x => x.ActionPointCost);

            IEnumerable<int> expected = new int[] { 20, 20, 60, 24, 24, 48, 44, 44 };
            CollectionAssert.AreEquivalent(expected, movementCosts);
        }

        [Test]
        public void CorrectlyIncludesTiles()
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

            MovementConsultant.FindShortestPath(gameMapData[unitA], gameMapData[unitB], gameMapData, 
                new Vector3Int[] { gameMapData[unitA], gameMapData[unitB] });
        }
    }
}
