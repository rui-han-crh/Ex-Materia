using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveButtonInitialiser : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(SaveFunction);
    }

    private void SaveFunction()
    {
        SaveFile.Save();
        SaveManager.SerialiseToFile();
    }

}
