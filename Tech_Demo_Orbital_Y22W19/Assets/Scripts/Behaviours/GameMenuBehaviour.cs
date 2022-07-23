using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;

public class GameMenuBehaviour : MonoBehaviour
{
    private KeyboardControls keyboardControls;

    private bool menuOn;

    private CanvasGroup canvasGroup;

    public GameObject[] menusToLoad;

    private Dictionary<string, GameObject> menus = new Dictionary<string, GameObject>();

    [SerializeField]
    private GameObject currentMenuGameObject;

    private CanvasGroup menuCanvasGroup;

    private GameObject currentOpenMenu;

    private void Awake()
    {
        keyboardControls = new KeyboardControls();

        keyboardControls.Mouse.Tab.performed += _ => MenuScreenSetActive(!menuOn);

        canvasGroup = GetComponent<CanvasGroup>();

        menuCanvasGroup = currentMenuGameObject.GetComponent<CanvasGroup>();

        foreach (GameObject menu in menusToLoad)
        {
            menus.Add(menu.name, menu);
        }
    }

    public void OnEnable()
    {
        keyboardControls?.Enable();
    }

    public void OnDisable()
    {
        keyboardControls?.Disable();
    }

    public void MenuScreenSetActive(bool state)
    {
        menuOn = state;

        GetComponent<TransitionController>().SetAllTransitions(menuOn);

        if (menuOn)
        {
            MovementController.Instance?.OnDisable();
            canvasGroup.interactable = true;
            CanvasTransitions.Fade(canvasGroup, 0, 1, 0.5f);
        } 
        else
        {
            MovementController.Instance?.OnEnable();
            canvasGroup.interactable = false;
            CanvasTransitions.Fade(canvasGroup, 1, 0, 0.5f);
        }
    }

    public void LoadMenu(string menuName)
    {
        if (!menus.ContainsKey(menuName))
        {
            Debug.LogWarning($"There was no menu called {menuName}, did you add it in GameMenuBehaviour?");
            return;
        }

        currentMenuGameObject.GetComponent<FadeAnimation>().SetAnimationState(true);
        menuCanvasGroup.interactable = true;

        MenuScreenSetActive(false);

        currentOpenMenu = Instantiate(menus[menuName], currentMenuGameObject.transform);
        currentOpenMenu.transform.SetAsFirstSibling();
    }

    public void CloseCurrentMenu()
    {
        currentMenuGameObject.GetComponent<FadeAnimation>().SetAnimationState(false);
        menuCanvasGroup.interactable = false;

        if (currentOpenMenu != null)
            Destroy(currentOpenMenu);
    }
}
