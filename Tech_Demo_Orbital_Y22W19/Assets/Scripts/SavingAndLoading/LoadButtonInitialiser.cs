using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadButtonInitialiser : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(SaveFunction);
    }

    private void SaveFunction()
    {
        SaveManager.DeserialiseFromFile();
        SaveFile.Load();
    }
}
