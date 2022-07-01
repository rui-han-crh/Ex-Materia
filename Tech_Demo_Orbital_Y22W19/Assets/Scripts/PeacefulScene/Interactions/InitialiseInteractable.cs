using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class InitialiseInteractable : MonoBehaviour
{
    [SerializeField]
    private Interactable interactable;

    public float time = 1;

    void Start()
    {
        Interactable self = GetComponent<Interactable>();
        StartCoroutine(LateInvoke(() => interactable.Interact(self), time));
    }

    private IEnumerator LateInvoke(Action func, float delay)
    {
        yield return new WaitForSeconds(delay);
        func.Invoke();  
    }
}
