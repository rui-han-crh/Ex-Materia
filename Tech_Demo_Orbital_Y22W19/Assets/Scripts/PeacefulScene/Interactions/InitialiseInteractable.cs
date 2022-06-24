using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class InitialiseInteractable : MonoBehaviour
{
    [SerializeField]
    public Interactable interactable;

    void Start()
    {
        Interactable self = GetComponent<Interactable>();
        StartCoroutine(LateInvoke(() => interactable.Interact(self), 1));
    }

    private IEnumerator LateInvoke(Action func, float delay)
    {
        yield return new WaitForSeconds(delay);
        func.Invoke();  
    }
}
