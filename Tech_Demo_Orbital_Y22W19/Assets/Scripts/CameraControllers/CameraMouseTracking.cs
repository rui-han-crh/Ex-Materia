using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseTracking : MonoBehaviour
{
    private KeyboardControls keyboardControls;
    private Vector3 originPosition;

    public float movementMagnitude = 1f;
    public float averageGradient = 1;

    private void OnEnable()
    {
        keyboardControls?.Enable();
    }

    private void OnDisable()
    {
        keyboardControls?.Disable();
        transform.position = originPosition;
    }

    private void Awake()
    {
        keyboardControls = new KeyboardControls();

        keyboardControls.Mouse.MousePosition.performed += ctx => MouseTrack(ctx.ReadValue<Vector2>());

        originPosition = transform.position;
    }

    private void MouseTrack(Vector2 mousePosition)
    {
        float halfScreenWidth = Screen.width / 2;
        float halfScreenHeight = Screen.height / 2;

        Vector3 delta = mousePosition - new Vector2(halfScreenWidth, halfScreenHeight);

        delta.x /= halfScreenWidth;
        delta.y /= halfScreenHeight;

        Vector3 movement =
            new Vector2(
                MathExtensions.InverseLogit(delta.x, 1, averageGradient) * movementMagnitude, 
                MathExtensions.InverseLogit(delta.y, 1, averageGradient) * movementMagnitude
                );

        transform.position = originPosition + movement;
    }

    public void BringCameraToOrigin()
    {
        new Task(MoveCameraToOrigin());
    }

    private IEnumerator MoveCameraToOrigin()
    {
        float startTime = Time.time;
        
        Vector3 sourcePosition = transform.position;
        float journeyDistance = Vector3.Distance(originPosition, transform.position);

        while (Vector3.Distance(originPosition, transform.position) > Mathf.Epsilon)
        {
            float currentTime = Time.time;
            transform.position = Vector3.Lerp(sourcePosition, originPosition, Mathf.Clamp01((currentTime - startTime) * 0.5f) / journeyDistance);
            yield return null;
        }

        transform.position = originPosition;
    }
}
