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

    private AsyncOperation pendingSceneLoad;

    private void Awake()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.AddCommandHandler("transitionScene", TransitionToScene);
        dialogueRunner.AddCommandHandler("unsetNextScene", UnsetNextScene);
    }

    public int? SceneBuildIndex
    {
        get;
        private set;
    }

    public void PrepareScene(int sceneIndex)
    {
        SceneBuildIndex = sceneIndex;
    }


    public void UnsetNextScene()
    {
        if (SceneBuildIndex == null)
        {
            return;
        }

        SceneBuildIndex = null;
    }

    public void TransitionToScene()
    {
        Debug.Log("Scene transition requested");
        if (SceneBuildIndex == null)
        {
            Debug.LogError("No next scene loaded");
            return;
        }

        pendingSceneLoad = SceneManager.LoadSceneAsync(SceneBuildIndex.Value);
        
    }
}
