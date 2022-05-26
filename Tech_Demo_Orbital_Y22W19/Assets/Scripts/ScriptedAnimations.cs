using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScriptedAnimations : MonoBehaviour
{
    private static readonly float INTERPOLATION_CONSTANT = 5f;

    [SerializeField]
    private Transform[] allButtonTransforms;

    [SerializeField]
    private GameObject informationSpace;
    private CanvasGroup informationSpaceCanvasGroup;

    private GameObject cancelButton;

    private Dictionary<GameObject, Vector2[]> buttonOriginalAnchors = new Dictionary<GameObject, Vector2[]>();
    private Dictionary<GameObject, bool> isButtonSelected = new Dictionary<GameObject, bool>();

    private void OnEnable()
    {
        foreach (KeyValuePair<GameObject, Vector2[]> buttonAnchor in buttonOriginalAnchors)
        {
            RectTransform rect = buttonAnchor.Key.GetComponent<RectTransform>();
            rect.anchorMin = buttonAnchor.Value[0];
            rect.anchorMax = buttonAnchor.Value[1];
        }
    }

    private void Start()
    {
        informationSpaceCanvasGroup = informationSpace.GetComponent<CanvasGroup>();

        foreach (Transform buttonTransform in allButtonTransforms)
        {
            Vector2 minAnchor = buttonTransform.GetComponent<RectTransform>().anchorMin;
            Vector2 maxAnchor = buttonTransform.GetComponent<RectTransform>().anchorMax;
            buttonOriginalAnchors[buttonTransform.gameObject] = new Vector2[] { minAnchor, maxAnchor };

            isButtonSelected[buttonTransform.gameObject] = false;
        }
    }
    public void ToggleUI()
    {
        IEnumerator Lerp(GameObject buttonGameObject, bool horizontally, bool toActivePosition)
        {
            buttonGameObject.SetActive(true);
            RectTransform buttonRectTransform = buttonGameObject.GetComponent<RectTransform>();

            if (horizontally)
            {
                isButtonSelected[buttonGameObject] = toActivePosition;
            }

            Vector2 minAnchorSource;
            Vector2 minAnchorDestination;

            Vector2 maxAnchorSource;
            Vector2 maxAnchorDestination;

            if (horizontally)
            {
                if (toActivePosition)
                {
                    minAnchorSource = buttonOriginalAnchors[buttonGameObject][0];
                    minAnchorDestination = new Vector2(0, minAnchorSource.y);

                    maxAnchorSource = buttonOriginalAnchors[buttonGameObject][1];
                    maxAnchorDestination = new Vector2(maxAnchorSource.x - minAnchorSource.x, maxAnchorSource.y);
                }
                else
                {
                    minAnchorDestination = buttonOriginalAnchors[buttonGameObject][0];
                    minAnchorSource = buttonRectTransform.anchorMin;

                    maxAnchorDestination = buttonOriginalAnchors[buttonGameObject][1];
                    maxAnchorSource = buttonRectTransform.anchorMax;
                }
            } 
            else
            {
                if (toActivePosition)
                {
                    minAnchorSource = buttonOriginalAnchors[buttonGameObject][0];
                    maxAnchorSource = buttonOriginalAnchors[buttonGameObject][1];

                    minAnchorDestination = new Vector2(minAnchorSource.x, minAnchorSource.y - maxAnchorSource.y);
                    maxAnchorDestination = new Vector2(maxAnchorSource.x, 0);
                }
                else
                {
                    minAnchorDestination = buttonOriginalAnchors[buttonGameObject][0];
                    minAnchorSource = buttonRectTransform.anchorMin;

                    maxAnchorDestination = buttonOriginalAnchors[buttonGameObject][1];
                    maxAnchorSource = buttonRectTransform.anchorMax;
                }
            }

            float speed = horizontally ?
                Mathf.Abs(minAnchorDestination.x - minAnchorSource.x) * INTERPOLATION_CONSTANT:
                Mathf.Abs(minAnchorDestination.y - minAnchorSource.y) * INTERPOLATION_CONSTANT;

            float startTime = Time.time;
            float journeyLength = Vector3.Distance(minAnchorSource, minAnchorDestination);
            while (Vector2.Distance(buttonRectTransform.anchorMin, minAnchorDestination) > Mathf.Epsilon 
                    && Vector2.Distance(buttonRectTransform.anchorMax, maxAnchorDestination) > Mathf.Epsilon)
            {
                float distanceCovered = (Time.time - startTime) * speed;
                float fractionOfJourney = Mathf.Min(1, distanceCovered / journeyLength);
                buttonRectTransform.anchorMin = Vector2.Lerp(minAnchorSource, minAnchorDestination, fractionOfJourney);
                buttonRectTransform.anchorMax = Vector2.Lerp(maxAnchorSource, maxAnchorDestination, fractionOfJourney);
                yield return null;
            }
            buttonRectTransform.anchorMin = minAnchorDestination;
            buttonRectTransform.anchorMax = maxAnchorDestination;
            if (toActivePosition && !horizontally)
                buttonGameObject.SetActive(false);
        }

        IEnumerator Fade(CanvasGroup component, bool opaque)
        {
            component.gameObject.SetActive(true);
            float source = component.alpha;
            float destination = opaque ? 1 : 0;

            float startTime = Time.time;
            float journeyLength = Mathf.Abs(destination - source);
            while (Mathf.Abs(component.alpha - destination) > Mathf.Epsilon)
            {
                float distanceCovered = (Time.time - startTime) * INTERPOLATION_CONSTANT;
                float fractionOfJourney = Mathf.Min(1, distanceCovered / journeyLength);
                component.alpha = Mathf.Lerp(source, destination, fractionOfJourney);
                yield return null;
            }
            component.alpha = destination;
            component.gameObject.SetActive(opaque);
        }

        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;

        StopAllCoroutines();

        if (!isButtonSelected.ContainsKey(selectedGameObject))
        {
            selectedGameObject = selectedGameObject.transform.parent.gameObject;
        }

        StartCoroutine(Lerp(selectedGameObject, true, !isButtonSelected[selectedGameObject]));
        StartCoroutine(Fade(informationSpaceCanvasGroup, isButtonSelected[selectedGameObject]));
        foreach (Transform otherButton in allButtonTransforms)
        {
            if (otherButton.gameObject.Equals(selectedGameObject) || otherButton.gameObject.Equals(cancelButton))
            {
                continue;
            }

            StartCoroutine(Lerp(otherButton.gameObject, false, isButtonSelected[selectedGameObject]));
        }
    }
}
