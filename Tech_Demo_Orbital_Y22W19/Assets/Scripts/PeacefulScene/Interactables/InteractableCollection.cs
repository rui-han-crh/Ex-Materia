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


    public CanvasGroup DialogueSystemCanvasGroup;

    public GameObject InteractIcon;

    public static readonly Vector3 HEAD_OFFSET = new Vector3(0, 0.5f, 0);

    public Canvas Canvas;

    private void Awake()
    {
        DialogueSystemCanvasGroup.alpha = 0;
        DialogueSystemCanvasGroup.interactable = false;
    }
}