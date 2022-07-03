using Algorithms.ShortestPathSearch;
using CombatSystem.Consultants;
using Facades;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Transitions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Tilemaps;
using TileType = CombatSystem.Entities.TileData.TileType;

public class TileCostTooltipBehaviour : MonoBehaviour
{
    private KeyboardControls keyboardControls;

    private Camera mainCamera;

    private Tilemap ground;

    private TileMapFacade tileMapFacade;

    private CombatSceneManager combatSceneManager;

    private CanvasGroup canvasGroup;

    [SerializeField]
    private TMP_Text tileNameText;

    [SerializeField]
    private TMP_Text tileCostText;

    [SerializeField]
    private TMP_Text pathCostText;

    [SerializeField]
    private TMP_Text coverTypeText;

    private Task mouseMovementTask;

    public bool IsPointerOverUI
    {
        get
        {
            // [Only works well while there is not PhysicsRaycaster on the Camera)
            //EventSystem eventSystem = EventSystem.current;
            //return (eventSystem != null && eventSystem.IsPointerOverGameObject());

            // [Works with PhysicsRaycaster on the Camera. Requires New Input System. Assumes mouse.)
            if (EventSystem.current == null || EventSystem.current.currentInputModule == null || Mouse.current == null)
            {
                return false;
            }
            RaycastResult lastRaycastResult = ((InputSystemUIInputModule)EventSystem.current.currentInputModule).GetLastRaycastResult(Mouse.current.deviceId);
            const int uiLayer = 5;
            return lastRaycastResult.gameObject != null && lastRaycastResult.gameObject.layer == uiLayer;
        }
    }


    public void OnEnable()
    {
        keyboardControls?.Enable();
    }

    public void OnDisable()
    {
        keyboardControls?.Disable();
    }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        tileMapFacade = TileMapFacade.Instance;
        combatSceneManager = CombatSceneManager.Instance;
        ground = tileMapFacade.groundTilemaps[0];
        mainCamera = Camera.main;
        keyboardControls = new KeyboardControls();
        keyboardControls.Mouse.MousePosition.performed += ctx => OnMouseMoved(ctx.ReadValue<Vector2>());
        keyboardControls.Mouse.LeftClick.performed += _ => OnMouseCancel();
    }

    private void OnMouseCancel()
    {
        mouseMovementTask?.Stop();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
    }

    private void OnMouseMoved(Vector2 mousePosition)
    {
        OnMouseCancel();

        if (IsPointerOverUI)
        {
            return;
        }

        transform.position = mousePosition;

        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        Vector3Int gridPosition = ground.WorldToCell(worldPosition);
        
        if (combatSceneManager.CurrentMap.Data.HasTile(gridPosition))
        {
            mouseMovementTask = new Task(UpdateTooltip(gridPosition));
        }
    }

    private IEnumerator UpdateTooltip(Vector3Int gridPosition)
    {
        yield return new WaitForSeconds(0.5f);

        tileNameText.text = combatSceneManager.CurrentMap.Data.GetTileName(gridPosition);
        TileType tileType = combatSceneManager.CurrentMap.Data.GetTileType(gridPosition);

        if (tileType != TileType.Ground)
        {
            tileCostText.text = "Unwalkable";
            pathCostText.text = "Unreachable";
        }
        else
        {
            tileCostText.text = combatSceneManager.CurrentMap.Data.GetTileCost(gridPosition).ToString();
            try
            {
                pathCostText.text = MovementConsultant.GetPathCost(
                    MovementConsultant.FindShortestPath(
                        combatSceneManager.CurrentActingUnitPosition,
                        gridPosition,
                        combatSceneManager.CurrentMap.Data
                        ),
                    combatSceneManager.CurrentMap.Data).ToString();
            }
            catch (Exception)
            {
                pathCostText.text = "Unreachable";
            }
        }

        coverTypeText.text = Enum.GetName(typeof(TileType), tileType);

        CanvasTransitions.Fade(canvasGroup, 0, 0.8f, 0.3f);
    }
}
