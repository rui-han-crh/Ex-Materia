using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCameraInteraction : MonoBehaviour, IInteraction
{
    public event System.Action OnEnded = delegate { };

    [SerializeField]
    private Vector3 focusPosition;
    [SerializeField]
    private float timeToFocus;

    public void FlushEventHandlers()
    {
        OnEnded = delegate { };
    }

    public void Interact()
    {
        CameraController.Instance.FocusOn(focusPosition, timeToFocus);
        OnEnded();
        FlushEventHandlers();
    }
}
