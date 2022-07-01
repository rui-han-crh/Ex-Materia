using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForSecondsInteraction : Interaction
{
    [SerializeField]
    private float secondsToWait;

    public override void Interact()
    {
        IEnumerator Waiting()
        {
            yield return new WaitForSeconds(secondsToWait);
            OnEnd();
        }
        StartCoroutine(Waiting());
    }
}
