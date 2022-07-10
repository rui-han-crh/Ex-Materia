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
        dialogueRunner?.AddCommandHandler("transitionScene", TransitionToScene);
        dialogueRunner?.AddCommandHandler("unsetNextScene", UnsetNextScene);
        dialogueRunner?.AddCommandHandler("changeScene", (string sceneName) => ChangeScene(sceneName));
    }

    private void Start()
    {
        SaveFile.Load();
    }

    public string SceneName
    {
        get;
        private set;
    }

    public void PrepareScene(string sceneName)
    {
        SceneName = sceneName;
    }


    public void UnsetNextScene()
    {
        if (SceneName == null)
        {
            return;
        }

        SceneName = null;
    }

    public void TransitionToScene()
    {
        Debug.Log("Scene transition requested");
        if (SceneName == null)
        {
            Debug.LogError("No next scene loaded");
            return;
        }

        StartCoroutine(DelayedSceneSwap(SceneName));
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(DelayedSceneSwap(sceneName));
    }

    private IEnumerator DelayedSceneSwap(string sceneName)
    {
        yield return new WaitForEndOfFrame();
        SaveFile.Save();
        SceneManager.LoadSceneAsync(sceneName);
    }
}
