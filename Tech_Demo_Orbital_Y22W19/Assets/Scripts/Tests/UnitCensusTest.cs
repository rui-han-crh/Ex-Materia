using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Entities;
using System.Reflection;
using System;
using Censuses;
public class UnitCensusTest
{
    private UnitProperties propertiesA = new UnitProperties(maxHealth: 100, 
                                                            currentHealth: 100, 
                                                            attack: 20, 
                                                            defence: 20, 
                                                            range: 7, 
                                                            maxActionPoints: 150, 
                                                            currentActionPoints: 150);

    private UnitProperties propertiesB = new UnitProperties(maxHealth: 103, 
                                                            currentHealth: 100, 
                                                            attack: 17, 
                                                            defence: 20, 
                                                            range: 7, 
                                                            maxActionPoints: 150, 
                                                            currentActionPoints: 150);

    [Test]
    public void UnitCensusAdd()
    {
        Unit.ResetClass();

        Unit unit = Unit.CreateNewUnit(propertiesA);

        UnitCensus census = new UnitCensus();
        UnitCensus newCensus = census.Add(Vector3Int.one, unit);

        Assert.IsTrue(newCensus[unit] == Vector3Int.one);

        Assert.Throws<KeyNotFoundException>(() => _ = census[unit]);
    }

    [Test]
    public void UnitCensusRemove()
    {
        Unit.ResetClass();

        Unit unit = Unit.CreateNewUnit(propertiesA);

        UnitCensus census = new UnitCensus();
        UnitCensus newCensus = census.Add(Vector3Int.one, unit);

        UnitCensus removedUnitCensus = newCensus.Remove(unit);

        Assert.IsTrue(newCensus[unit] == Vector3Int.one);
        Assert.IsTrue(!removedUnitCensus.Contains(unit));

        Assert.AreEqual(0, census.Count);
        Assert.AreEqual(1, newCensus.Count);
        Assert.AreEqual(0, removedUnitCensus.Count);
    }

    [Test]
    public void DuplicatedAddition()
    {
        Unit.ResetClass();

        Unit unitA = Unit.CreateNewUnit(propertiesA);

        Unit unitB = Unit.CreateNewUnit(propertiesB);

        UnitCensus census = new UnitCensus();
        UnitCensus censusWithA = census.Add(Vector3Int.one, unitA);

        UnitCensus censusWithAB = censusWithA.Add(new Vector3Int(1, 0, 0), unitB);

        Assert.AreEqual(2, censusWithAB.Count);
        Assert.AreEqual(1, censusWithA.Count);
        Assert.AreEqual(0, census.Count);

        Assert.IsFalse(censusWithA.Contains(unitB));

        Assert.Throws<ArgumentException>(() => _ = censusWithAB.Add(new Vector3Int(2, 1, 0), unitA));
        Assert.Throws<ArgumentException>(() => _ = censusWithAB.Add(new Vector3Int(1, 3, 0), unitB));
    }

    [Test]
    public void NonExistentRemoval()
    {
        Unit.ResetClass();

        Unit unitA = Unit.CreateNewUnit(propertiesA);

        Unit unitB = Unit.CreateNewUnit(propertiesB);

        UnitCensus census = new UnitCensus();

        Assert.AreEqual(0, census.Count);

        Assert.IsFalse(census.Contains(unitB));

        Assert.Throws<KeyNotFoundException>(() => _ = census.Remove(unitA));
        Assert.Throws<KeyNotFoundException>(() => _ = census.Remove(unitB));

        for (int i = 0; i < 10; i++) {
            Assert.Throws<KeyNotFoundException>(() => _ = census.Remove(new Vector3Int(UnityEngine.Random.Range(1, 10),
                                                                                    UnityEngine.Random.Range(1, 10),
                                                                                    UnityEngine.Random.Range(1, 10))));
        }
    }

    [Test]
    public void ChangePosition()
    {
        Unit.ResetClass();

        Unit unitA = Unit.CreateNewUnit(propertiesA);

        Unit unitB = Unit.CreateNewUnit(propertiesB);

        UnitCensus census = new UnitCensus();

        census = census.Add(Vector3Int.one, unitA).Add(Vector3Int.zero, unitB);

        census = census.MoveUnit(unitA, new Vector3Int(1, 2, 0));

        Assert.IsTrue(census[unitA] == new Vector3Int(1, 2, 0));
        Assert.IsFalse(census.Contains(new Vector3Int(1, 1, 1)));
    }

    [Test]
    public void ChangePositionOfModifiedUnit()
    {
        Unit.ResetClass();

        Unit unitA = Unit.CreateNewUnit(propertiesA);

        Unit unitB = Unit.CreateNewUnit(propertiesB);

        UnitCensus census = new UnitCensus();

        census = census.Add(Vector3Int.one, unitA).Add(Vector3Int.zero, unitB);

        unitA = unitA.ChangeHealth(12);

        census = census.MoveUnit(unitA, new Vector3Int(1, 2, 0));

        Assert.IsTrue(census[unitA] == new Vector3Int(1, 2, 0));
        Assert.IsFalse(census.Contains(new Vector3Int(1, 1, 1)));
    }

    [Test]
    public void SwapUnitTest()
    {
        Unit.ResetClass();

        Unit unitA = Unit.CreateNewUnit(propertiesA);

        Unit unitB = Unit.CreateNewUnit(propertiesB);

        UnitCensus census = new UnitCensus();

        census = census.Add(Vector3Int.one, unitA).Add(Vector3Int.zero, unitB);

        Unit unitAUpdated = unitA.ChangeHealth(56);

        UnitCensus newCensus = census.SwapUnit(unitAUpdated, unitAUpdated);

        Assert.AreEqual(56, newCensus[Vector3Int.one].CurrentHealth);

        Assert.AreNotEqual(56, census[Vector3Int.one].CurrentHealth);
    }

    [Test]
    public void UpdateUnitTest()
    {
        Unit.ResetClass();

        Unit unitA = Unit.CreateNewUnit(propertiesA);

        Unit unitB = Unit.CreateNewUnit(propertiesB);

        UnitCensus census = new UnitCensus();

        census = census.Add(Vector3Int.one, unitA).Add(Vector3Int.zero, unitB);

        Unit unitAUpdated = unitA.ChangeHealth(56);

        UnitCensus newCensusA = census.UpdateUnit(unitAUpdated);

        Unit unitBUpdated = unitB.ChangeActionPoints(10);

        UnitCensus newCensusAB = newCensusA.UpdateUnit(unitBUpdated);

        Assert.AreEqual(56, newCensusAB[Vector3Int.one].CurrentHealth);
        Assert.AreEqual(56, newCensusA[Vector3Int.one].CurrentHealth);

        Assert.AreEqual(10, newCensusAB[Vector3Int.zero].CurrentActionPoints);

        Assert.AreNotEqual(10, census[Vector3Int.zero].CurrentActionPoints);
        Assert.AreNotEqual(56, census[Vector3Int.one].CurrentHealth);
    }
}
