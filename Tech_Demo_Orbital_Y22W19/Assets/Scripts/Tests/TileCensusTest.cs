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


public class TileCensusTest
{
    [Test]
    public void TileCensusAdd()
    {
        TileData tileData = new TileData(1, TileData.TileType.Ground);

        TileCensus census = new TileCensus();

        TileCensus newCensus = census.Add(Vector3Int.zero, tileData);

        Assert.AreEqual(1, newCensus.Count);

        Assert.IsTrue(newCensus.Contains(Vector3Int.zero));

        Assert.IsTrue(newCensus[Vector3Int.zero] == tileData);

        Assert.AreEqual(0, census.Count);
    }

    [Test]
    public void TileCensusRemove()
    {
        TileData tileData = new TileData(1, TileData.TileType.Ground);

        TileCensus census = new TileCensus();

        TileCensus newCensus = census.Add(Vector3Int.zero, tileData);

        TileCensus removedCensus = newCensus.Remove(Vector3Int.zero);

        Assert.AreEqual(1, newCensus.Count);

        Assert.AreEqual(0, removedCensus.Count);

        Assert.IsTrue(!removedCensus.Contains(Vector3Int.zero));
    }

    [Test]
    public void DuplicatedPositionAdd()
    {
        TileData tileDataA = new TileData(1, TileData.TileType.Ground);
        TileData tileDataB = new TileData(1, TileData.TileType.Ground);

        TileCensus census = new TileCensus();

        TileCensus newCensusWithA = census.Add(Vector3Int.zero, tileDataA);
        Assert.Throws<ArgumentException>(() => _ = newCensusWithA.Add(Vector3Int.zero, tileDataB));
    }

    [Test]
    public void NonExistentRemove()
    {
        TileData tileDataA = new TileData(1, TileData.TileType.Ground);

        TileCensus census = new TileCensus();

        TileCensus newCensusWithA = census.Add(Vector3Int.zero, tileDataA);
        Assert.Throws<KeyNotFoundException>(() => _ = newCensusWithA.Remove(Vector3Int.one));
    }

    [Test]
    public void GetTileType()
    {
        TileData tileDataA = new TileData(1, TileData.TileType.Ground);
        TileData tileDataB = new TileData(1, TileData.TileType.FullCover);

        TileCensus census = new TileCensus();

        TileCensus newCensusWithA = census.Add(Vector3Int.zero, tileDataA);
        TileCensus newCensusWithAB = census.Add(Vector3Int.one, tileDataB);

        Assert.AreEqual(TileData.TileType.Ground, newCensusWithA.GetTileType(Vector3Int.zero));
        Assert.AreEqual(TileData.TileType.FullCover, newCensusWithAB.GetTileType(Vector3Int.one));

        Assert.Throws<KeyNotFoundException>(() => _ = newCensusWithA.GetTileType(Vector3Int.one));
    }

    [Test]
    public void GetTileCost()
    {
        TileData tileDataA = new TileData(1, TileData.TileType.Ground);
        TileData tileDataB = new TileData(int.MaxValue, TileData.TileType.FullCover);

        TileCensus census = new TileCensus();

        TileCensus newCensusWithA = census.Add(Vector3Int.zero, tileDataA);
        TileCensus newCensusWithAB = census.Add(Vector3Int.one, tileDataB);

        Assert.AreEqual(1, newCensusWithA.GetTileCost(Vector3Int.zero));
        Assert.AreEqual(int.MaxValue, newCensusWithAB.GetTileCost(Vector3Int.one));

        Assert.Throws<KeyNotFoundException>(() => _ = newCensusWithA.GetTileCost(Vector3Int.one));
    }
}
