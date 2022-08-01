using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class MasterVolumeSetterBehaviour : MonoBehaviour
{
    void Start()
    {
        GetComponent<Slider>().onValueChanged.AddListener(value => AudioListener.volume = value);
    }
}
