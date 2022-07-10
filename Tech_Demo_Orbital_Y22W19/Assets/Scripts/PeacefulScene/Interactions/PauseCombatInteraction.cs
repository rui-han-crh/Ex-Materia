using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCombatInteraction : Interaction
{
    [SerializeField]
    private bool isPaused;

    public override void Interact()
    {
        CombatSceneManager.Instance.SetIsPaused(isPaused);
        OnEnd();
    }
}
