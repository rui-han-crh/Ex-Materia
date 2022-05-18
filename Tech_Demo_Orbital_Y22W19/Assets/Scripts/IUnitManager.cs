using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public interface IUnitManager
{
    public Unit GetSelectedUnit();

    public Dictionary<Vector3Int, Unit> GetPositionsOfUnits();

    public bool IsOverUI();

    public bool IsRoutineEmpty();

    public Vector3Int GetPositionByUnit(Unit unit);

    public Unit GetUnitByPosition(Vector3Int position);

    public void UpdateUnitPosition(Unit unit, Vector3Int position);

    public void Enqueue(IEnumerator routine);
}