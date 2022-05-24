using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorLookUp;
using TMPro;
using System.Linq;

public partial class GameManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text apNeededTextMovement;
    [SerializeField]
    private TMP_Text timeNeededTextMovement;

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
            int cost = currentMap.FindShortestPathTo(gridPosition, out IEnumerable<Vector3Int> pathPositions);
            pathPositionsLastDrawn = pathPositions;

            apNeededTextMovement.text = cost.ToString();
            timeNeededTextMovement.text = (Mathf.CeilToInt((float)cost / currentMap.CurrentUnit.Speed)).ToString();

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
}
