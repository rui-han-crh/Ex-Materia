using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class SceneTransitionManager : MonoBehaviour
{
    private static SceneTransitionManager instance;
    public static SceneTransitionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SceneTransitionManager>();
                Debug.Assert(instance != null, "There is no SceneTransitionManager in the scene, consider adding one");
            }
            return instance;
        }
    }

    private DialogueRunner dialogueRunner;

    private void Awake()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.AddCommandHandler("transitionScene", TransitionToScene);
        dialogueRunner.AddCommandHandler("unsetNextScene", UnsetSceneIndex);
    }

    public int? SceneBuildIndex
    {
        get;
        set;
    }


    public void UnsetSceneIndex()
    {
        SceneBuildIndex = null;
    }

    public void TransitionToScene()
    {
        Debug.Log("Called");
        if (SceneBuildIndex == null)
        {
            return;
        }

        SceneManager.LoadSceneAsync(SceneBuildIndex.Value);
    }
}
