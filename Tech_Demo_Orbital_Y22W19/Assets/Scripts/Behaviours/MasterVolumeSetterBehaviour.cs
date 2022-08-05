using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class MasterVolumeSetterBehaviour : MonoBehaviour
{
    private Slider volumeSlider;
    void Start()
    {
        volumeSlider = GetComponent<Slider>();

        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(value => AudioListener.volume = value);
    }
}
