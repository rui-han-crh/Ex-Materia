using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Entities;
using System.Reflection;

public class UnitEntitiesTest
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
    public void UnitPropertyInitialisationTest()
    {
        Unit.ResetClass();

        Unit unit = Unit.CreateNewUnit(propertiesA);

        Assert.AreEqual(1, unit.Identity);
        Assert.AreEqual(100, unit.MaxHealth);
        Assert.AreEqual(100, unit.CurrentHealth);
        Assert.AreEqual(20, unit.Attack);
        Assert.AreEqual(20, unit.Defence);
        Assert.AreEqual(7, unit.Range);
        Assert.AreEqual(150, unit.MaxActionPoints);
        Assert.AreEqual(150, unit.CurrentActionPoints);
    }

    [Test]
    public void UnitIdentityIncrementingTest()
    {
        Unit.ResetClass();

        Unit unit = Unit.CreateNewUnit(propertiesA);

        Unit anotherUnit = Unit.CreateNewUnit(propertiesB);

        Assert.IsFalse(unit.Identity.Equals(anotherUnit.Identity));
        Assert.AreEqual(1, unit.Identity);
        Assert.AreEqual(2, anotherUnit.Identity);
    }

    [Test]
    public void UnitBoundedHealthTest()
    {
        Unit.ResetClass();

        UnitProperties properties = new UnitProperties(maxHealth: 100, currentHealth: 101, attack: 20, defence: 20, range: 7, maxActionPoints: 1, currentActionPoints: 0);
        Unit unit = Unit.CreateNewUnit(properties);

        Assert.AreEqual(1, unit.Identity);
        Assert.AreEqual(100, unit.CurrentHealth);

        UnitProperties anotherProperties = new UnitProperties(maxHealth: 100, currentHealth: -1, attack: 20, defence: 20, range: 7, maxActionPoints: 1, currentActionPoints: 0);
        Unit anotherUnit = Unit.CreateNewUnit(anotherProperties);

        Assert.AreEqual(2, anotherUnit.Identity);
        Assert.AreEqual(0, anotherUnit.CurrentHealth);
    }

    [Test]
    public void UnitChangeHealthTest()
    {
        Unit.ResetClass();

        UnitProperties properties = new UnitProperties(maxHealth: 100, currentHealth: 100, attack: 20, defence: 20, range: 7, maxActionPoints: 1, currentActionPoints: 0);
        Unit unit = Unit.CreateNewUnit(properties);

        for (int i = 0; i < 20; i++)
        {
            int change = Random.Range(-10, 110);
            unit = unit.ChangeHealth(change);
            Assert.AreEqual(Mathf.Clamp(change, 0, 100), unit.CurrentHealth);
        }
    }

    [Test]
    public void UnitChangeAttackTest()
    {
        Unit.ResetClass();

        UnitProperties properties = new UnitProperties(maxHealth: 100, currentHealth: 100, attack: 20, defence: 20, range: 7, maxActionPoints: 1, currentActionPoints: 0);
        Unit unit = Unit.CreateNewUnit(properties);

        for (int i = 0; i < 20; i++)
        {
            int change = Random.Range(-10, 110);
            unit = unit.ChangeAttack(change);
            Assert.AreEqual(change, unit.Attack);
        }
    }

    [Test]
    public void UnitChangeDefenceTest()
    {
        Unit.ResetClass();

        UnitProperties properties = new UnitProperties(maxHealth: 100, currentHealth: 100, attack: 20, defence: 20, range: 7, maxActionPoints: 1, currentActionPoints: 0);
        Unit unit = Unit.CreateNewUnit(properties);

        for (int i = 0; i < 20; i++)
        {
            int change = Random.Range(-10, 110);
            unit = unit.ChangeDefence(change);
            Assert.AreEqual(change, unit.Defence);
        }
    }

    [Test]
    public void UnitChangeRangeTest()
    {
        Unit.ResetClass();

        UnitProperties properties = new UnitProperties(maxHealth: 100, currentHealth: 100, attack: 20, defence: 20, range: 7, maxActionPoints: 1, currentActionPoints: 0);
        Unit unit = Unit.CreateNewUnit(properties);

        for (int i = 0; i < 20; i++)
        {
            int change = Random.Range(-10, 110);
            unit = unit.ChangeRange(change);
            Assert.AreEqual(Mathf.Max(0, change), unit.Range);
        }
    }

    [Test]
    public void UnitImmutabilityTest()
    {
        Unit.ResetClass();

        UnitProperties properties = new UnitProperties(maxHealth: 100, currentHealth: 100, attack: 20, defence: 20, range: 7, maxActionPoints: 1, currentActionPoints: 0);
        Unit unit = Unit.CreateNewUnit(properties);

        Assert.AreEqual(1, unit.Identity);

        Unit nextUnit = unit.ChangeHealth(unit.CurrentHealth - 10);

        Assert.AreEqual(1, nextUnit.Identity);
        Assert.AreEqual(90, nextUnit.CurrentHealth);
        Assert.AreEqual(100, unit.CurrentHealth);
    }

    [Test]
    public void UnitStatusEffectsTest()
    {
        Unit.ResetClass();

        UnitProperties properties = new UnitProperties(maxHealth: 100, currentHealth: 100, attack: 20, defence: 20, range: 7, maxActionPoints: 1, currentActionPoints: 0);
        Unit unit = Unit.CreateNewUnit(properties);

        Assert.AreEqual(1, unit.Identity);

        Unit nextUnit = unit.ApplyStatusEffect(UnitStatusEffects.Overwatch);

        Assert.IsTrue(nextUnit.HasStatusEffect(UnitStatusEffects.Overwatch));

        Unit nextNextUnit = nextUnit.RemoveStatusEffect(UnitStatusEffects.Overwatch);

        Assert.IsFalse(nextNextUnit.HasStatusEffect(UnitStatusEffects.Overwatch));
        Assert.IsTrue(nextNextUnit.HasStatusEffect(UnitStatusEffects.None));

        Assert.IsTrue(nextUnit.HasStatusEffect(UnitStatusEffects.Overwatch));

        Unit nextNextNextUnit = nextUnit.ApplyStatusEffect(UnitStatusEffects.All);

        Assert.IsTrue(nextNextNextUnit.HasStatusEffect(UnitStatusEffects.Overwatch));
    }
}
