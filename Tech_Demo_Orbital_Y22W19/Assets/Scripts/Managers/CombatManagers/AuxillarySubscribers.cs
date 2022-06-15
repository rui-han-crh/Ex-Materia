using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Facades;
using CombatSystem.Consultants;
using CombatSystem.Entities;
using ColorLookUp;
using UnityEngine.Tilemaps;

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
    }
}
