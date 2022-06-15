using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Facades;
using CombatSystem.Consultants;
using CombatSystem.Entities;
using ColorLookUp;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.EventSystems;

namespace Managers.Subscribers
{
    public static class AuxillarySubscribers
    {
        public static void SubscribeMovementSelection(InputAction mouseClick)
        {
            Vector2 mousePosition = mouseClick.ReadValue<Vector2>();
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            CombatSceneManager combatSceneManager = CombatSceneManager.Instance;

            Vector3Int gridPosition = combatSceneManager.WorldToCell(mousePosition);

            Tilemap indicatorMap = TileManager.Instance.IndicatorMap;

            TileDrawer.SetColorToTiles(indicatorMap, TileManager.Instance.Indicator.color);

            InformationUIManager.Instance.SetAllTextToDefault();

            GameMap currentMap = combatSceneManager.CurrentMap;
            Unit currentUnit = currentMap.CurrentActingUnit;

            if (!MovementConsultant.GetAllMovementPositions(currentMap.Data, currentUnit).Contains(gridPosition))
            {
                return;
            }

            IEnumerable<Vector3Int> shortestPath =
                MovementConsultant.FindShortestPath(
                    currentMap[currentUnit],
                    gridPosition,
                    currentMap.Data,
                    new Vector3Int[] { currentMap[currentUnit] }
                    );

            combatSceneManager.SetIndicatedTiles(shortestPath);

            int cost = MovementConsultant.GetPathCost(shortestPath, currentMap.Data);
            InformationUIManager.Instance.SetTimeAndAPRequiredText(cost, cost);

            TileDrawer.SetColorToTiles(indicatorMap, shortestPath, ColorPalette.LIGHT_BLUE_TRANSLUCENT);
            LineDrawer.DrawLineOnTileMap(indicatorMap, shortestPath);
            LineDrawer.ColorLine(Color.blue);
            combatSceneManager.SetLastActionAllowed(true);
        }

        public static void SubscribeCombatSelection(InputAction mouseClick)
        {
            CombatSceneManager combatSceneManager = CombatSceneManager.Instance;

            if (combatSceneManager.IsPointerOverUI)
            {
                return;
            }

            Vector2 mousePosition = mouseClick.ReadValue<Vector2>();
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector3Int gridPosition = combatSceneManager.WorldToCell(mousePosition);

            Tilemap indicatorMap = TileManager.Instance.IndicatorMap;

            TileDrawer.SetColorToTiles(
                indicatorMap,
                ColorPalette.YELLOW_TRANSLUCENT
                );

            if (!combatSceneManager.CurrentMap.Data.HasUnitAt(gridPosition) 
                || combatSceneManager.CurrentMap[gridPosition].Faction == combatSceneManager.CurrentActingUnit.Faction)
            {
                GlobalResourceManager.Instance.LineRenderer.positionCount = 0;
                CanvasManager.Instance.DeactivateUI(CanvasManager.UIType.OpponentSheet);
                combatSceneManager.SetLastActionAllowed(false);
                return;
            }

            Vector3Int currentUnitPosition = combatSceneManager.CurrentActingUnitPosition;

            AttackRequest request = CombatConsultant.SimulateAttack(
                combatSceneManager.CurrentActingUnit,
                combatSceneManager.CurrentMap[gridPosition],
                combatSceneManager.CurrentMap.Data,
                considerActionPoints: true
                );

            AttackRequest.Outcome status = request.Status;

            InformationUIManager.Instance.SetAllTextToDefault();

            switch (status)
            {
                case AttackRequest.Outcome.Successful:
                    combatSceneManager.SetIndicatedTiles(request.TilesHit);
                    combatSceneManager.SetSavedActionCost(request.ActionPointCost);
                    //int lastActionCost = request.ActionPointCost;

                    TileDrawer.SetColorToTiles(indicatorMap, request.TilesHit, ColorPalette.LIGHT_GREEN_TRANSLUCENT);
                    LineDrawer.DrawLineOnTileMap(indicatorMap, new Vector3Int[] { request.TilesHit.First(), request.TilesHit.Last() } );
                    LineDrawer.ColorLine(ColorPalette.DARK_GREEN);

                    CanvasManager.Instance.ActivateUI(CanvasManager.UIType.OpponentSheet);

                    InformationUIManager.Instance.SetChanceToHitText(request.ChanceToHit);
                    InformationUIManager.Instance.SetTimeAndAPRequiredText(request.ActionPointCost, request.ActionPointCost);
                    InformationUIManager.Instance.SetOpponentDetails(request.TargetUnit);
                    InformationUIManager.Instance.SetResultantDamageDealt(Mathf.Max(1, request.ActingUnit.Attack - request.TargetUnit.Defence));

                    combatSceneManager.SetLastActionAllowed(true);
                    break;

                case AttackRequest.Outcome.NotEnoughActionPoints: 
                case AttackRequest.Outcome.NoLineOfSight:
                    combatSceneManager.SetIndicatedTiles(request.TilesHit);
                    //int lastActionCost = request.ActionPointCost;

                    TileDrawer.SetColorToTiles(indicatorMap, request.TilesHit, ColorPalette.LIGHT_RED_TRANSLUCENT);
                    LineDrawer.DrawLineOnTileMap(indicatorMap, new Vector3Int[] { currentUnitPosition, gridPosition });
                    LineDrawer.ColorLine(Color.red);

                    CanvasManager.Instance.DeactivateUI(CanvasManager.UIType.OpponentSheet);

                    InformationUIManager.Instance.SetChanceToHitText(0);
                    InformationUIManager.Instance.SetOpponentDetails(request.TargetUnit);
                    InformationUIManager.Instance.SetResultantDamageDealt(0);


                    combatSceneManager.SetLastActionAllowed(false);
                    break;
            }
        }
    }
}
