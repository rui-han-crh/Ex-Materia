using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    KeyboardControls keyboardControls;
    private bool canDrag = false;

    private const float SCROLL_MULTIPLIER = 0.2f;

    [SerializeField]
    private float dragSensitivity = 0.05f;

    private float scrollCountsLeft;

    [SerializeField]
    private float scrollSensitivity = 20;
    private const float ZOOM_DRAG_RATIO = 15;
    private const float SCROLL_CONSTANT = 120;
    public const float MIN_ZOOM = 1;
    public const float MAX_ZOOM = 8;

    private void Awake()
    {
        keyboardControls = new KeyboardControls();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        keyboardControls.Enable();
    }

    private void OnDisable()
    {
        keyboardControls?.Disable();
    }

    private void Start()
    {
        keyboardControls.Mouse.MiddleButtonHold.performed += _ => { canDrag = true; Cursor.visible = false; };
        keyboardControls.Mouse.MiddleButtonHold.canceled += _ => { canDrag = false; Cursor.visible = true; };
        keyboardControls.Mouse.Scroll.performed += ctx => scrollCountsLeft += ctx.ReadValue<float>();
        mainCamera = Camera.main;
    }


    private void Drag()
    {
        Vector3 mousePositionDelta = keyboardControls.Mouse.MousePositionDelta.ReadValue<Vector2>();
        mainCamera.transform.position -= mousePositionDelta * dragSensitivity * (mainCamera.orthographicSize / ZOOM_DRAG_RATIO);
    }

    private void Update()
    {
        if (canDrag)
        {
            Drag();
        }
        
        if (Mathf.Abs(scrollCountsLeft) > 0.1f)
        {
            mainCamera.orthographicSize -= Mathf.Sign(scrollCountsLeft) * scrollSensitivity;
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, MIN_ZOOM, MAX_ZOOM);
            scrollCountsLeft -= Mathf.Sign(scrollCountsLeft) * SCROLL_CONSTANT * scrollSensitivity;
        } else
        {
            scrollCountsLeft = 0;
        }
    }
}
