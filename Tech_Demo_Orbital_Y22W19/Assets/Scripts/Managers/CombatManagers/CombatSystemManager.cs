using CombatSystem.Facade;
using Facades;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystemManager : MonoBehaviour
{
    private UnitManager unitManager;
    private TileManager tileManager;

    private GameObject combatSystemHolder;
    private CombatSceneManager combatSceneManager;

    [SerializeField]
    private GameObject combatSystemView;

    public UnitStatusEffectsFacade statusEffectsDatabase;

    [SerializeField]
    private bool allowGameOverState;


    private void Awake()
    {
        unitManager = FindObjectOfType<UnitManager>();
        Debug.Assert(unitManager != null, "There exists no UnitManager in this scene. It is required for the Combat System, please add one");

        tileManager = FindObjectOfType<TileManager>();
        Debug.Assert(tileManager != null, "There exists no TileManager in this scene. It is required for the Combat System, please add one");

        combatSystemHolder = new GameObject("CombatSystem");
        combatSystemHolder.transform.SetParent(transform);

        bool state = allowGameOverState;
        Debug.Log($"state {state}");
        combatSceneManager = combatSystemHolder.AddComponent<CombatSceneManager>();

        combatSceneManager.SetGameOverAllowed(state);

        combatSceneManager.Initialise(unitManager, tileManager);

        statusEffectsDatabase = Instantiate(statusEffectsDatabase);
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
