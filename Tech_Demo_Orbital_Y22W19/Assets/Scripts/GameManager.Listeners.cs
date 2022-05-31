using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorLookUp;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public partial class GameManager : MonoBehaviour
{
    private int timeToWait;
    private void MovementSelectionListener()
    {
        if (isOverUI)
            return;

        Vector2 mousePosition = keyboardControls.Mouse.MousePosition.ReadValue<Vector2>();
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3Int gridPosition = groundTilemap.WorldToCell(mousePosition);

        TileDrawer.SetColorToTiles(tileHighlights, pathPositionsLastDrawn, tileHighlightIndicator.color);

        if (groundTilemap.HasTile(gridPosition))
        {
            currentTileSelected = gridPosition;
            lastActionCost = currentMap.FindShortestPathTo(gridPosition, out IEnumerable<Vector3Int> pathPositions);
            pathPositionsLastDrawn = pathPositions;

            InformationUIManager.Instance.APNeededUI.GetComponentInChildren<TMP_Text>().text = lastActionCost.ToString();
            InformationUIManager.Instance.TimeUI.GetComponentInChildren<TMP_Text>().text = 
                (Mathf.CeilToInt((float)lastActionCost / currentMap.CurrentUnit.Speed)).ToString();

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
        AttackRequest request = currentMap.IsAttackableAt(gridPosition);
        AttackStatus status = request.Status;

        switch (status)
        {
            case AttackStatus.Success:
                pathPositionsLastDrawn = request.TilesHit;
                TileDrawer.SetColorToTiles(tileHighlights, request.TilesHit, ColorPalette.LIGHT_GREEN_TRANSLUCENT);
                LineDrawer.DrawLineOnTileMap(groundTilemap, pathLine, new Vector3Int[] { request.TilesHit.First(), request.TilesHit.Last() });
                LineDrawer.ColorLine(pathLine, ColorPalette.DARK_GREEN);
                executeLastActionAllowed = true;
                break;

            case AttackStatus.IllegalTarget:
                StateReset();
                break;

            default:
                pathPositionsLastDrawn = request.TilesHit;
                TileDrawer.SetColorToTiles(tileHighlights, request.TilesHit, ColorPalette.LIGHT_RED_TRANSLUCENT);
                LineDrawer.DrawLineOnTileMap(groundTilemap, pathLine, new Vector3Int[] { request.TilesHit.First(), request.TilesHit.Last() });
                LineDrawer.ColorLine(pathLine, Color.red);
                executeLastActionAllowed = false;
                break;
        }
    }

    public void WaitListener(Slider slider)
    {
        IEnumerable<Unit> orderedUnitsByTime = currentMap.AllUnits.OrderBy(unit => unit.Time);
        int numberOfUnits = orderedUnitsByTime.Count();

        float positionInQueue = numberOfUnits * slider.value;
        int numberOfUnitsPassed = Mathf.Min((int)(positionInQueue), numberOfUnits - 1);
        float excess = positionInQueue - numberOfUnitsPassed;

        int timeAfter = numberOfUnitsPassed == numberOfUnits - 1 ? 
                            orderedUnitsByTime.Last().Time + 200 : 
                            orderedUnitsByTime.ElementAt(numberOfUnitsPassed + 1).Time;
        int timeBefore = orderedUnitsByTime.ElementAt(numberOfUnitsPassed).Time;

        int offsetTime = timeBefore - orderedUnitsByTime.First().Time;

        int differenceTime = Mathf.CeilToInt((timeAfter - timeBefore) * excess);

        timeToWait = offsetTime + differenceTime;

        InformationUIManager.Instance.TimeUI.GetComponentInChildren<TMP_Text>().text = timeToWait.ToString();
    }
}
