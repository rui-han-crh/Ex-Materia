using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCameraInteraction : Interaction
{

    [SerializeField]
    private Vector3 focusPosition;
    [SerializeField]
    private float timeToFocus;

    public override void Interact()
    {
        CameraController.Instance.FocusOn(focusPosition, timeToFocus);
        OnEnd();
        FlushEventHandlers();
    }
}
