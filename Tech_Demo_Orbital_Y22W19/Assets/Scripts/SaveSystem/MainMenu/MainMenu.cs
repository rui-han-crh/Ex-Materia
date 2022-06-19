using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] 
    private Button newGameButton;


    [SerializeField]
    private Button continueButton;

    public void Start()
    {
        if (!DataPersistenceManager.Instance.HasGameData())
        {
            continueButton.interactable = false;
        }

    }

    public void OnNewGameClicked()
    {
        //create a new game 
        DataPersistenceManager.Instance.NewGame();

        //load the gameplay scene, which also saves the scene 
        SceneManager.LoadSceneAsync("PeacefulScene");
    }


    public void OnContinueGameClicked()
    {
        DataPersistenceManager.Instance.LoadGame(); //Save has a build in check to see if there;s
        //existing game data!
        SceneManager.LoadScene(DataPersistenceManager.Instance.GetCurrentScene());
        
    }
}
