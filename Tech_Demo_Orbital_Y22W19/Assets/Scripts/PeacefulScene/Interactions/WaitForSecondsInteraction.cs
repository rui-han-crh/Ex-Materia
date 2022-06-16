using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForSecondsInteraction : MonoBehaviour, IInteraction
{
    public event Action OnEnded;

    [SerializeField]
    private float secondsToWait;

    public void Interact()
    {
        IEnumerator Waiting()
        {
            yield return new WaitForSeconds(secondsToWait);
            InvokeDelegate();
        }
        StartCoroutine(Waiting());
    }

    private void InvokeDelegate()
    {
        OnEnded();
        FlushEventHandlers();
    }

    public void FlushEventHandlers()
    {
        OnEnded = delegate { };
    }

}
