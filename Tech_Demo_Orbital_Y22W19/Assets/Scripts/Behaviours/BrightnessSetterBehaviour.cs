using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BrightnessSetterBehaviour : MonoBehaviour
{
    private ColorAdjustments colorAdjustments;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Start()
    {
        FindObjectOfType<Volume>().profile.TryGet(out colorAdjustments);

        slider.value = colorAdjustments.postExposure.value;
        slider.onValueChanged.AddListener(value => colorAdjustments.postExposure.value = value);
    }
}
