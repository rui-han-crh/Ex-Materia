using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(LerpAnimation))]
public class OnHoverPullOutAnimation : MonoBehaviour
{
    private LerpAnimation lerpAnimation;

    private bool stateOn = false;

    private void Awake()
    {
        lerpAnimation = GetComponent<LerpAnimation>();
    }

    public static GameObject MouseOverObject
    {
        get
        {
            // [Only works well while there is not PhysicsRaycaster on the Camera)
            //EventSystem eventSystem = EventSystem.current;
            //return (eventSystem != null && eventSystem.IsPointerOverGameObject());

            // [Works with PhysicsRaycaster on the Camera. Requires New Input System. Assumes mouse.)
            if (EventSystem.current == null)
            {
                return null;
            }
            RaycastResult lastRaycastResult = ((InputSystemUIInputModule)EventSystem.current.currentInputModule).GetLastRaycastResult(Mouse.current.deviceId);
            const int uiLayer = 5;
            return (lastRaycastResult.gameObject != null && lastRaycastResult.gameObject.layer == uiLayer) ? lastRaycastResult.gameObject : null;
        }
    }


    private void Update()
    {
        if (MouseOverObject == gameObject && !stateOn)
        {
            stateOn = true;
            lerpAnimation.SetAnimationState(true);
        } 
        else if (MouseOverObject != gameObject && stateOn)
        {
            stateOn = false;
            lerpAnimation.SetAnimationState(false);
        }
    }
}
