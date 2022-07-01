using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static CameraController instance;
    public static CameraController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CameraController>();
                Debug.Assert(instance != null, "There was no CameraController in this scene, consider adding one");
            }
            return instance;
        }
    }


    private Camera mainCamera;
    KeyboardControls keyboardControls;

    [SerializeField]
    private float dragSensitivity = 0.05f;

    [SerializeField]
    private float scrollSensitivity = 1;

    private const float ZOOM_DRAG_RATIO = 15;

    public readonly float MIN_ZOOM = 2;
    public readonly float MAX_ZOOM = 7;

    private bool canDrag = false;
    private Task dollyTask;

    [SerializeField]
    private float focusSpeed = 10f;

    [SerializeField]
    private float rotationSpeed = 2f;

    private Task focusingCoroutine;

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
        if (focusingCoroutine != null)
        {
            focusingCoroutine.Stop();
        }
        focusingCoroutine = null;

        keyboardControls?.Disable();
    }

    private void Start()
    {
        keyboardControls.Mouse.MiddleButtonHold.performed += _ => OnEnableDrag();
        keyboardControls.Mouse.MiddleButtonHold.canceled += _ => OnDisableDrag();
        keyboardControls.Mouse.Scroll.performed += ctx => OnScroll(ctx.ReadValue<float>());
        mainCamera = Camera.main;
    }

    private void Update()
    {
        CheckCameraMovement();
    }

    private void OnEnableDrag()
    {
        canDrag = true;
        Cursor.visible = false;
    }

    private void OnDisableDrag()
    {
        canDrag = false;
        Cursor.visible = true;
    }

    private void CheckCameraMovement()
    {
        if (!canDrag)
        {
            return;
        }

        Cursor.visible = false;
        Vector3 mousePositionDelta = keyboardControls.Mouse.MousePositionDelta.ReadValue<Vector2>();
        mainCamera.transform.position -= (mainCamera.orthographicSize / ZOOM_DRAG_RATIO) * dragSensitivity * mousePositionDelta;
    }

    private void OnScroll(float scrollAmount)
    {
        mainCamera.orthographicSize -= scrollSensitivity * Mathf.Sign(scrollAmount);
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, MIN_ZOOM, MAX_ZOOM);
    }

    public void FocusOn(Vector3 positionToFocus, float? totalTime = null)
    {
        if (focusingCoroutine != null && focusingCoroutine.Running)
        {
            return;
        }

        positionToFocus.z = mainCamera.transform.position.z;

        IEnumerator Lerp()
        {
            Vector3 source = mainCamera.transform.position;
            float startTime = Time.time;
            float journeyLength = Vector3.Distance(source, positionToFocus);
            totalTime ??= journeyLength / focusSpeed;

            while (Vector3.Distance(mainCamera.transform.position, positionToFocus) > Mathf.Epsilon)
            {
                float differenceTime = Time.time - startTime;
                float fractionCovered = Mathf.Clamp01(differenceTime / totalTime.Value);
                mainCamera.transform.position = Vector3.Lerp(source, positionToFocus, fractionCovered);
                yield return null;
            }

            mainCamera.transform.position = positionToFocus;
        }

        focusingCoroutine = new Task(Lerp());
    }

    public void Dolly(float degrees)
    {
        if ((dollyTask?.Running).GetValueOrDefault(false))
        {
            dollyTask.Stop();
        }

        IEnumerator Rotate()
        {
            Quaternion source = mainCamera.transform.rotation;
            Quaternion destination = source * Quaternion.Euler(0, 0, degrees);
            float startTime = Time.time;
            float angleDifference = Quaternion.Angle(source, destination);

            while (Quaternion.Angle(mainCamera.transform.rotation, destination) > Mathf.Epsilon)
            {
                float differenceTime = Time.time - startTime;
                float fractionCovered = Mathf.Clamp01((differenceTime * rotationSpeed) / angleDifference);
                mainCamera.transform.rotation = Quaternion.Lerp(source, destination, fractionCovered);
                yield return null;
            }
            mainCamera.transform.rotation = destination;
        }

        dollyTask = new Task(Rotate());
    }

    public void ResetDolly()
    {
        float rotation = mainCamera.transform.rotation.eulerAngles.z;
        Dolly(-rotation % 360);
    }
}
