using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ExtensionMethods;

public class BloomControllerInteraction : Interaction
{
    private Bloom bloom;
    public float duration;
    public float finalIntensity;

    private void Awake()
    {
        Volume volume = GetComponent<Volume>();
        volume.profile.TryGet(out bloom);
        if (bloom != null)
        {
            bloom.SetAllOverridesTo(true);
        }
    }

    public override void Interact()
    {
        IEnumerator Change()
        {
            float startTime = Time.time;
            float initialValue = bloom.intensity.value;

            while (!MathExtensions.Approximately(bloom.intensity.value, finalIntensity, 0.01f))
            {
                bloom.intensity.value = Mathf.Lerp(initialValue, finalIntensity, Mathf.Clamp01((Time.time - startTime) / duration));
                yield return null;
            }
            bloom.intensity.value = finalIntensity;
        }
        Task t = new Task(Change());
        t.Finished += _ => OnEnd();
    }
}
