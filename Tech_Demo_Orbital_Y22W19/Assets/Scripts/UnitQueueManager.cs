using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// A specific structure that managed Unit turns
/// 1. A unit is OBSERVED to take the next turn 
/// 2. It moves a certain amount and incurs a COST
/// 3. It is shifted to the correct place in the structure
/// 
/// Unit will ONLY be removed from the structure if dead.
/// </summary>
public class UnitQueueManager
{
    private Unit[] unitArray;
    private Dictionary<Unit, int> unitIndexMap;
    private int count = 0;

    private Unit currentUnit;

    public UnitQueueManager(Unit[] unitArray)
    {
        this.unitArray = unitArray;
        count = unitArray.Length;
        Array.Sort(unitArray);

        unitIndexMap = new Dictionary<Unit, int>();
        for (int i = 0; i < unitArray.Length; i++)
        {
            unitIndexMap[unitArray[i]] = i;
        }
    }

    public int ActiveUnitCount => count;

    public IReadOnlyCollection<Unit> AllUnits => Array.AsReadOnly(unitArray);

    public int GetUnitIndex(Unit unit)
    {
        return unitIndexMap[unit];
    }

    public Unit GetCurrentUnit()
    {
        currentUnit = unitArray[0];
        return currentUnit;
    }

    public void UpdateQueue()
    {
        Array.Sort(unitArray);
        currentUnit = unitArray[0];
        for (int i = 0; i < unitArray.Length; i++)
        {
            unitIndexMap[unitArray[i]] = i;
        }
    }

    //public void Remove(Unit unit)
    //{
    //    if (!unitIndexMap.ContainsKey(unit))
    //    {
    //        throw new KeyNotFoundException("Unit does not exist in the queue");
    //    }
    //    unitArray[unitIndexMap[unit]] = null;
    //    unitIndexMap.Remove(unit);
    //    UpdateQueue();
    //}
}
