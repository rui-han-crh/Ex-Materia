using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleBehaviour : MonoBehaviour
{
    private ModifySaveData modifySaveData;
    private Toggle toggle;

    void Start()
    {
        toggle = GetComponent<Toggle>();
        modifySaveData = GetComponent<ModifySaveData>();

        toggle.isOn = modifySaveData.HasValue() ? modifySaveData.FetchValue<bool>() : toggle.isOn;

        toggle.onValueChanged.AddListener(value => modifySaveData.SetBoolValue(value));
    }
}
