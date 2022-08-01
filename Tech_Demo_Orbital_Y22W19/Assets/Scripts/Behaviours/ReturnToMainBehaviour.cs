using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnToMainBehaviour : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            SaveFile.Save();
            SaveManager.SerialiseToFile();

            FindObjectOfType<SceneTransitionManager>().ChangeScene("MainMenu");
        });
    }
}
