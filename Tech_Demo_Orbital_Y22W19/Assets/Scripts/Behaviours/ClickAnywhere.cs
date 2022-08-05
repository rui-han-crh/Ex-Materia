using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class ClickAnywhere : MonoBehaviour
{
    [SerializeField]
    private UnityEvent unityEvent;

    private KeyboardControls keyboardControls;

    public bool IsPointerOverUI
    {
        get
        {
            // [Only works well while there is not PhysicsRaycaster on the Camera)
            //EventSystem eventSystem = EventSystem.current;
            //return (eventSystem != null && eventSystem.IsPointerOverGameObject());

            // [Works with PhysicsRaycaster on the Camera. Requires New Input System. Assumes mouse.)
            if (EventSystem.current == null)
            {
                return false;
            }
            RaycastResult lastRaycastResult = ((InputSystemUIInputModule)EventSystem.current.currentInputModule).GetLastRaycastResult(Mouse.current.deviceId);
            const int uiLayer = 5;
            return lastRaycastResult.gameObject != null && lastRaycastResult.gameObject.layer == uiLayer;
        }
    }

    public void OnEnable()
    {
        keyboardControls?.Enable();
    }

    public void OnDisable()
    {
        keyboardControls?.Disable();
    }

    private void Awake()
    {
        keyboardControls = new KeyboardControls();

        keyboardControls.Mouse.LeftClick.performed += _ => Click();
    }

    private void Click()
    {
        if (!IsPointerOverUI)
        {
            unityEvent?.Invoke();
        }
    }
}
