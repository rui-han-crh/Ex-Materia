using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorLookUp;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public partial class GameManagerOld : MonoBehaviour
{
    private static readonly int OPPONENT_UI_INDEX = 1;

    private int timeToWait;
    private void MovementSelectionListener()
    {
        if (isOverUI)
            return;

        Vector2 mousePosition = keyboardControls.Mouse.MousePosition.ReadValue<Vector2>();
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3Int gridPosition = groundTilemap.WorldToCell(mousePosition);

        TileDrawer.SetColorToTiles(tileHighlights, pathPositionsLastDrawn, blockSelectorTile.color);

        InformationUIManager.Instance.SetAllTextToDefault();

        if (groundTilemap.HasTile(gridPosition))
        {
            currentTileSelected = gridPosition;
            lastActionCost = currentMap.FindShortestPathTo(gridPosition, out IEnumerable<Vector3Int> pathPositions);
            pathPositionsLastDrawn = pathPositions;

            InformationUIManager.Instance.SetTimeAndAPRequiredText(lastActionCost);

            TileDrawer.SetColorToTiles(tileHighlights, pathPositions, ColorPalette.LIGHT_BLUE_TRANSLUCENT);
            LineDrawer.DrawLineOnTileMap(tileHighlights, pathLine, pathPositions, currentMap.CurrentUnitPosition);
            LineDrawer.ColorLine(pathLine, Color.blue);
            executeLastActionAllowed = true;
        }
    }

    private void CombatSelectionListener()
    {
        if (isOverUI)
            return;

        Vector2 mousePosition = keyboardControls.Mouse.MousePosition.ReadValue<Vector2>();
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3Int gridPosition = groundTilemap.WorldToCell(mousePosition);

        TileDrawer.SetColorToTiles(tileHighlights, pathPositionsLastDrawn, ColorPalette.YELLOW_TRANSLUCENT);
        AttackRequestOld request = currentMap.IsAttackableAt(gridPosition);
        AttackStatus status = request.Status;

        InformationUIManager.Instance.SetAllTextToDefault();

        switch (status)
        {
            case AttackStatus.Success:
                pathPositionsLastDrawn = request.TilesHit;
                lastActionCost = request.ActionPointCost;

                TileDrawer.SetColorToTiles(tileHighlights, request.TilesHit, ColorPalette.LIGHT_GREEN_TRANSLUCENT);
                LineDrawer.DrawLineOnTileMap(groundTilemap, pathLine, new Vector3Int[] { request.TilesHit.First(), request.TilesHit.Last() });
                LineDrawer.ColorLine(pathLine, ColorPalette.DARK_GREEN);

                canvasLinearAnimation.UIToActivePosition(OPPONENT_UI_INDEX);

                InformationUIManager.Instance.SetChanceToHitText(request.ChanceToHit);
                InformationUIManager.Instance.SetTimeAndAPRequiredText(request.ActionPointCost);
                InformationUIManager.Instance.SetOpponentDetails(request.TargetUnit);
                InformationUIManager.Instance.SetResultantDamageDealt(Mathf.Max(1, request.ActingUnit.Attack - request.TargetUnit.Defence));

                executeLastActionAllowed = true;
                break;

            case AttackStatus.IllegalTarget:
                canvasLinearAnimation.UIToDeactivePosition(OPPONENT_UI_INDEX);
                gameState = GameState.AwaitCombat;
                break;


            default:
                pathPositionsLastDrawn = request.TilesHit;
                lastActionCost = request.ActionPointCost;

                TileDrawer.SetColorToTiles(tileHighlights, request.TilesHit, ColorPalette.LIGHT_RED_TRANSLUCENT);
                LineDrawer.DrawLineOnTileMap(groundTilemap, pathLine, new Vector3Int[] { request.TilesHit.First(), request.TilesHit.Last() });
                LineDrawer.ColorLine(pathLine, Color.red);

                canvasLinearAnimation.UIToActivePosition(OPPONENT_UI_INDEX);

                InformationUIManager.Instance.SetChanceToHitText(0);
                InformationUIManager.Instance.SetOpponentDetails(request.TargetUnit);
                InformationUIManager.Instance.SetResultantDamageDealt(0);

                executeLastActionAllowed = false;
                break;
        }
    }

    public void WaitListener(Slider slider)
    {
        IEnumerable<UnitOld> orderedUnitsByTime = currentMap.AllUnits
                                                        .OrderBy(unit => unit.Time)
                                                        .SkipWhile(x => x.Time == currentMap.CurrentUnit.Time);

        int numberOfUnits = orderedUnitsByTime.Count();

        float positionInQueue = numberOfUnits * slider.value;

        int numberOfUnitsPassed = Mathf.Min((int)(positionInQueue), numberOfUnits - 1);
        float excess = positionInQueue - numberOfUnitsPassed;

        int timeAfter = numberOfUnitsPassed == numberOfUnits - 1 ? 
                            Mathf.Max(orderedUnitsByTime.Last().Time + 1, CurrentUnit.ActionPointsUsed + CurrentUnit.Time): 
                            orderedUnitsByTime.ElementAt(numberOfUnitsPassed + 1).Time;

        int timeBefore = orderedUnitsByTime.ElementAt(numberOfUnitsPassed).Time;

        int offsetTime = timeBefore - currentMap.CurrentUnit.Time;

        int differenceTime = Mathf.CeilToInt((timeAfter - timeBefore) * excess);

        timeToWait = offsetTime + differenceTime;

        InformationUIManager.Instance.SetTimeAndAPRequiredText(timeToWait, 0);
    }
}
