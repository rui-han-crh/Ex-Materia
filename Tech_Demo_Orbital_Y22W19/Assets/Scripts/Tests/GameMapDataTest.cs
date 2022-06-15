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
using DataStructures;

public class GameMapDataTest
{

    UnitProperties propertiesA = new UnitProperties(maxHealth: 100, currentHealth: 100, attack: 20, defence: 20, range: 7, maxActionPoints: 150, currentActionPoints: 150);
    UnitProperties propertiesB = new UnitProperties(maxHealth: 103, currentHealth: 100, attack: 17, defence: 20, range: 7, maxActionPoints: 150, currentActionPoints: 150);
    UnitProperties propertiesC = new UnitProperties(maxHealth: 105, currentHealth: 99, attack: 17, defence: 20, range: 7, maxActionPoints: 150, currentActionPoints: 150);


    [Test]
    public void GameMapDataValidMoveUnit()
    {
        TileData tileData = new TileData(1, TileData.TileType.Ground);
        TileCensus census = new TileCensus();
        TileCensus tileCensus = census.Add(Vector3Int.zero, tileData)
                                    .Add(new Vector3Int(1, 0, 0), tileData)
                                    .Add(new Vector3Int(2, 0, 0), tileData);

        Unit unitA = Unit.CreateNewUnit(propertiesA);
        Unit unitB = Unit.CreateNewUnit(propertiesB);
        UnitCensus unitCensus = new UnitCensus();
        unitCensus = unitCensus.Add(new Vector3Int(1, 0, 0), unitA).Add(Vector3Int.zero, unitB);
        GameMapData gameMapData = new GameMapData(unitCensus, tileCensus);

        Assert.IsTrue(gameMapData.HasUnitAt(new Vector3Int(1, 0, 0)));



        MovementRequest request = new MovementRequest(unitA, new Vector3Int(2, 0, 0), 14, 14, MovementRequest.Outcome.Successful);

        GameMapData newData = gameMapData.MoveUnit(request);

        Assert.IsTrue(newData[unitA] == new Vector3Int(2, 0, 0));

        Assert.IsFalse(newData.HasUnitAt(new Vector3Int(1, 0, 0)));

        Assert.IsTrue(gameMapData.HasUnitAt(new Vector3Int(1, 0, 0)));
    }


    [Test]
    public void GameMapDataInvalidMoveUnit()
    {
        TileData tileData = new TileData(1, TileData.TileType.Ground);

        TileCensus census = new TileCensus();

        TileCensus tileCensus = census.Add(Vector3Int.zero, tileData)
                                    .Add(new Vector3Int(1, 0, 0), tileData)
                                    .Add(new Vector3Int(2, 0, 0), tileData)
                                    .Add(new Vector3Int(3, 0, 0), tileData);


        Unit unitA = Unit.CreateNewUnit(propertiesA);


        Unit unitB = Unit.CreateNewUnit(propertiesB);

        Unit unitC = Unit.CreateNewUnit(propertiesC);

        UnitCensus unitCensus = new UnitCensus();

        unitCensus = unitCensus.Add(new Vector3Int(1, 0, 0), unitA).Add(Vector3Int.zero, unitB);

        GameMapData gameMapData = new GameMapData(unitCensus, tileCensus);

        Assert.IsTrue(gameMapData.HasUnitAt(new Vector3Int(1, 0, 0)));


        MovementRequest request = new MovementRequest(unitA, new Vector3Int(2, 1, 0), 14, 14, MovementRequest.Outcome.Successful);
        Assert.Throws<KeyNotFoundException>(() => _ = gameMapData.MoveUnit(request));


        request = new MovementRequest(unitA, new Vector3Int(3, 0, 0), 14, 14, MovementRequest.Outcome.Successful);
        Assert.DoesNotThrow(() => _ = gameMapData.MoveUnit(request));


        request = new MovementRequest(unitA, new Vector3Int(1, 0, 0), 14, 14, MovementRequest.Outcome.Successful);
        Assert.Throws<ArgumentException>(() => _ = gameMapData.MoveUnit(request));

        request = new MovementRequest(unitC, new Vector3Int(3, 0, 0), 14, 14, MovementRequest.Outcome.Successful);
        Assert.Throws<KeyNotFoundException>(() => _ = gameMapData.MoveUnit(request));
    }

    [Test]
    public void GameMapDataInvalidMoveToCoveredTile()
    {
        TileData tileData = new TileData(1, TileData.TileType.Ground);

        TileData tileDataCovered = new TileData(1, TileData.TileType.HalfCover);

        TileCensus census = new TileCensus();

        TileCensus tileCensus = census.Add(Vector3Int.zero, tileData)
                                    .Add(new Vector3Int(1, 0, 0), tileData)
                                    .Add(new Vector3Int(2, 0, 0), tileData)
                                    .Add(new Vector3Int(3, 0, 0), tileDataCovered);

        Unit unitA = Unit.CreateNewUnit(propertiesA);

        Unit unitB = Unit.CreateNewUnit(propertiesB);

        Unit unitC = Unit.CreateNewUnit(propertiesC);

        UnitCensus unitCensus = new UnitCensus();

        unitCensus = unitCensus.Add(new Vector3Int(1, 0, 0), unitA).Add(Vector3Int.zero, unitB);

        GameMapData gameMapData = new GameMapData(unitCensus, tileCensus);

        Assert.IsTrue(gameMapData.HasUnitAt(new Vector3Int(1, 0, 0)));


        MovementRequest request = new MovementRequest(unitA, new Vector3Int(3, 0, 0), 14, 14, MovementRequest.Outcome.Successful);
        Assert.Throws<ArgumentException>(() => _ = gameMapData.MoveUnit(request));
    }

    [Test]
    public void GameMapDataValidMoveOnChangedUnit()
    {
        TileData tileData = new TileData(1, TileData.TileType.Ground);
        TileCensus census = new TileCensus();
        TileCensus tileCensus = census.Add(Vector3Int.zero, tileData)
                                    .Add(new Vector3Int(1, 0, 0), tileData)
                                    .Add(new Vector3Int(2, 0, 0), tileData);

        Unit unitA = Unit.CreateNewUnit(propertiesA);
        Unit unitB = Unit.CreateNewUnit(propertiesB);
        UnitCensus unitCensus = new UnitCensus();
        unitCensus = unitCensus.Add(new Vector3Int(1, 0, 0), unitA).Add(Vector3Int.zero, unitB);
        GameMapData gameMapData = new GameMapData(unitCensus, tileCensus);


        Unit unitAChanged = unitA.ApplyStatusEffect(UnitStatusEffects.Overwatch).ChangeAttack(42);

        MovementRequest request = new MovementRequest(unitAChanged, new Vector3Int(2, 0, 0), 14, 14, MovementRequest.Outcome.Successful);

        GameMapData newData = gameMapData.MoveUnit(request);

        Assert.IsTrue(newData[unitA] == new Vector3Int(2, 0, 0));
        Assert.IsTrue(newData[unitAChanged] == new Vector3Int(2, 0, 0));

        unitAChanged = unitAChanged.ChangeActionPoints(55);

        MovementRequest anotherRequest = new MovementRequest(unitAChanged, new Vector3Int(1, 0, 0), 14, 14, MovementRequest.Outcome.Successful);

        GameMapData anotherData = newData.MoveUnit(anotherRequest);

        Assert.IsTrue(anotherData[unitA] == new Vector3Int(1, 0, 0));
        Assert.IsTrue(anotherData[unitAChanged] == new Vector3Int(1, 0, 0));
    }

    [Test]
    public void OnWaitUnit()
    {
        TileData tileData = new TileData(1, TileData.TileType.Ground);
        TileCensus census = new TileCensus();
        TileCensus tileCensus = census.Add(Vector3Int.zero, tileData)
                                    .Add(new Vector3Int(1, 0, 0), tileData)
                                    .Add(new Vector3Int(2, 0, 0), tileData);

        Unit unitA = Unit.CreateNewUnit(propertiesA);
        Unit unitB = Unit.CreateNewUnit(propertiesB);

        int unitBOriginalTime = unitB.Time;

        UnitCensus unitCensus = new UnitCensus();
        unitCensus = unitCensus.Add(new Vector3Int(1, 0, 0), unitA).Add(Vector3Int.zero, unitB);
        GameMapData gameMapData = new GameMapData(unitCensus, tileCensus);

        WaitRequest waitRequest = new WaitRequest(unitB, 120, 125);

        GameMapData dataAfterWait = gameMapData.WaitUnit(waitRequest);


        Assert.AreEqual(unitBOriginalTime, unitB.Time);
        Assert.AreEqual(120, dataAfterWait[Vector3Int.zero].Time);
        Assert.AreEqual(150, dataAfterWait[Vector3Int.zero].CurrentActionPoints);

        Assert.IsTrue(dataAfterWait.GetUnitWithMinimumTime().Equals(unitA));
    }

    [Test]
    public void OnOverwatchUnit()
    {
        TileData tileData = new TileData(1, TileData.TileType.Ground);
        TileCensus census = new TileCensus();
        TileCensus tileCensus = census.Add(Vector3Int.zero, tileData)
                                    .Add(new Vector3Int(1, 0, 0), tileData)
                                    .Add(new Vector3Int(2, 0, 0), tileData);

        Unit unitA = Unit.CreateNewUnit(propertiesA);
        Unit unitB = Unit.CreateNewUnit(propertiesB);


        UnitCensus unitCensus = new UnitCensus();
        unitCensus = unitCensus.Add(new Vector3Int(1, 0, 0), unitA).Add(Vector3Int.zero, unitB);
        GameMapData gameMapData = new GameMapData(unitCensus, tileCensus);

        OverwatchRequest overwatchRequest = new OverwatchRequest(unitA);

        GameMapData dataAfterWait = gameMapData.OverwatchUnit(overwatchRequest);


        Assert.IsTrue(dataAfterWait[new Vector3Int(1, 0, 0)].HasStatusEffect(UnitStatusEffects.Overwatch));
        Assert.AreEqual(OverwatchRequest.TIME_SPENT, dataAfterWait[new Vector3Int(1, 0, 0)].Time);
    }

    [Test]
    public void CorrectlyRetrievesTileCosts()
    {
        GameMapData gameMapData = new GameMapData(Warehouse.UNIT_CENSUS_A, Warehouse.TILE_CENSUS_POPTART_3X3);

        Assert.AreEqual(50, gameMapData.GetTileCost(new Vector3Int(1, 1, 0)));
    }

    [Test]
    public void CorrectlyConvertsToUndirectedGraph()
    {
        GameMapData gameMapData = Warehouse.GMD_EMPTY_POPTART;

        IUndirectedGraph<Vector3Int> undirectedGraph = gameMapData.ToUndirectedGraph();

        Assert.AreEqual(9, undirectedGraph.Count);
    }

    [Test]
    public void CorrectlyConvertsToUndirectedGraphWithInclusionZone()
    {
        GameMapData gameMapData = Warehouse.GMD_POPTART_OCCUPIED_TOP_MIDDLE;
        Unit actingUnit = gameMapData.GetUnitWithMinimumTime();

        IUndirectedGraph<Vector3Int> undirectedGraph = gameMapData.ToUndirectedGraph(new HashSet<Vector3Int> { gameMapData[actingUnit] });

        Assert.AreEqual(9, undirectedGraph.Count);
    }
}
