using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheetBehaviour : MonoBehaviour
{
    [SerializeField]
    private Unit currentUnitShowing;
    [SerializeField]
    private BarFillBehaviour healthBar;
    [SerializeField]
    private BarFillBehaviour apBar;

    public Unit CurrentUnit => currentUnitShowing;

    public void SetCurrentUnitShowing(Unit unit)
    {
        currentUnitShowing = unit;
        healthBar.UpdateBarFillImage(unit.Health, unit.MaxHealth);
        apBar.UpdateBarFillImage(unit.ActionPointsLeft, unit.MaxActionPoints);
    }
}
