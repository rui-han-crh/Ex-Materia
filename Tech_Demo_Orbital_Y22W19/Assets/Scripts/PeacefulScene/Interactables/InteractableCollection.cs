using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCollection : MonoBehaviour
{
    private static InteractableCollection instance;
    public static InteractableCollection Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InteractableCollection>();
                Debug.Assert(instance != null, "There is no InteractableCollection in the scene, consider adding one");
            }
            return instance;
        }
    }

    private HashSet<Interactable> interactables = new HashSet<Interactable>();

    public HashSet<Interactable> Collection => interactables;

    private GameObject interactableIconsHolder;

    public GameObject InteractableIconsHolder => interactableIconsHolder;

    public GameObject InteractIcon;

    public static readonly Vector3 HEAD_OFFSET = new Vector3(0, 0.5f, 0);

    private Canvas canvas;

    public Canvas Canvas => canvas;

    private void Awake()
    {
        GameObject canvasGameObject = GameObject.Find("Canvas");
        Debug.Assert(canvasGameObject != null, "Canvas could not be found, is it called \"Canvas\" in the scene? " +
            "Ensure there are no trailing whitespaces in the name.");

        canvas = canvasGameObject.GetComponent<Canvas>();
        Debug.Assert(Canvas != null, "The Canvas GameObject in the scene has no Canvas component added to the it. Please add one.");

        interactableIconsHolder = new GameObject("InteractableIcons");
        interactableIconsHolder.transform.SetParent(canvasGameObject.transform);
        interactableIconsHolder.transform.SetAsFirstSibling();
    }
}
