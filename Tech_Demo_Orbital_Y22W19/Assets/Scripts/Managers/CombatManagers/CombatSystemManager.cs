using CombatSystem.Facade;
using Facades;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystemManager : MonoBehaviour
{
    private UnitManager unitManager;
    private TileMapFacade tileMapFacade;
    private UnitStatusEffectsFacade unitStatusEffectsFacade;

    private GameObject combatSystemHolder;
    private CombatSceneManager combatSceneManager;

    public bool autocreateCombatSystemView = false;

    public GameObject combatSystemViewPrefab;

    public GameObject combatSystemView;


    private void Awake()
    {
        unitManager = FindObjectOfType<UnitManager>();
        Debug.Assert(unitManager != null, "There exists no UnitManager in this scene. It is required for the Combat System, please add one");
        tileMapFacade = FindObjectOfType<TileMapFacade>();
        Debug.Assert(tileMapFacade != null, "There exists no TileMapFacade in this scene. It is required for the Combat System, please add one");
        unitStatusEffectsFacade = FindObjectOfType<UnitStatusEffectsFacade>();
        Debug.Assert(unitStatusEffectsFacade != null, 
            "There exists no UnitStatusEffectsFacade in this scene. It is required for the Combat System, please add one");

        combatSystemHolder = new GameObject("CombatSystem");
        combatSystemHolder.transform.SetParent(transform);

        combatSceneManager = combatSystemHolder.AddComponent<CombatSceneManager>();
        combatSceneManager.Initialise(unitManager, tileMapFacade);

        if (autocreateCombatSystemView)
        {
            combatSystemView = Instantiate(combatSystemViewPrefab, InteractableCollection.Instance.Canvas.transform);
            combatSystemView.transform.SetAsFirstSibling();
        }
    }

    public void PauseGame(bool isPaused)
    {
        CombatSceneManager.Instance.SetIsPaused(isPaused);
    }

    public void AllowGameOver(bool allow)
    {
        CombatSceneManager.Instance.SetGameOverAllowed(allow);
    }
}
