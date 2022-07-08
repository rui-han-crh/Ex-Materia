using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	private Transform camTransform;

	// How long the object should shake for.
	public float shakeDuration = 0f;

	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;

	private Vector3 originalPos;

	void Awake()
	{
		camTransform = transform;
	}

	public void Shake(float delay)
    {
		StartCoroutine(ShakeEnumerator(delay));
    }

	private IEnumerator ShakeEnumerator(float delay)
    {
		originalPos = camTransform.localPosition;
		float savedShakeDuration = shakeDuration;
		float finalShake = shakeAmount;

		yield return new WaitForSeconds(delay);
		float startTime = Time.time;

		while (shakeDuration > 0)
        {
			shakeAmount = Mathf.Lerp(0, finalShake, Mathf.Clamp01((Time.time - startTime) / savedShakeDuration));

			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
			shakeDuration -= Time.deltaTime * decreaseFactor;
			yield return null;
		}

		shakeDuration = savedShakeDuration;
		camTransform.localPosition = originalPos;
	}
}
