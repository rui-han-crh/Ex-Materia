using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void OnNewGameClicked()
    {
        //create a new game 
        DataPersistenceManager.Instance.NewGame();

        //load the gameplay scene, which also saves the scene 
        SceneManager.LoadSceneAsync("PeacefulScene");
    }


    public void OnContinueGameClicked()
    {
        DataPersistenceManager.Instance.SaveGame();
        SceneManager.LoadSceneAsync("PeacefulScene");
        
    }
}
