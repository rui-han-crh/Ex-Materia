using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public interface IUnitManager
{
    public UnitOld GetSelectedUnit();

    public Dictionary<Vector3Int, UnitOld> GetPositionsOfUnits();

    public bool IsOverUI();

    public bool IsRoutineEmpty();

    public Vector3Int GetPositionByUnit(UnitOld unit);

    public UnitOld GetUnitByPosition(Vector3Int position);

    public void UpdateUnitPosition(UnitOld unit, Vector3Int position);

    public void Enqueue(IEnumerator routine);
}