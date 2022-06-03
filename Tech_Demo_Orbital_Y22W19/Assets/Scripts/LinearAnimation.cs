using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LinearAnimation : MonoBehaviour
{
    [Serializable]
    public class LinearAnimationTarget
    {
        [SerializeField]
        private GameObject gameObject;
        [SerializeField]
        private float animationSpeed = 1;
        [SerializeField]
        private Vector2 activeMinAnchor;
        [SerializeField]
        private Vector2 activeMaxAnchor;
        [SerializeField]
        private Vector2 deactiveMinAnchor;
        [SerializeField]
        private Vector2 deactiveMaxAnchor;


        private bool animationIsRunning;

        public GameObject GameObject => gameObject;
        public Vector2 ActiveMinAnchor => activeMinAnchor;
        public Vector2 ActiveMaxAnchor => activeMaxAnchor;
        public Vector2 DeactiveMinAnchor => deactiveMinAnchor;
        public Vector2 DeactiveMaxAnchor => deactiveMaxAnchor;
        public bool AnimationIsRunning { 
            get { return animationIsRunning; } 
            set { animationIsRunning = value; } 
        }
        public float AnimationSpeed => animationSpeed;

        public LinearAnimationTarget(GameObject gO, Vector2 destinationMinAnchor, Vector2 destinationMaxAnchor, float speed)
        {
            gameObject = gO;
            this.activeMinAnchor = destinationMinAnchor;
            this.activeMaxAnchor = destinationMaxAnchor;
            this.animationSpeed = speed;
        }
    }

    [SerializeField]
    private LinearAnimationTarget[] targets;

    public void SetTargets(LinearAnimationTarget[] inputTargets)
    {
        targets = inputTargets;
    }

    public void ToggleUI(int targetIndex)
    {
        
        IEnumerator Lerp(LinearAnimationTarget target, bool toActivePosition)
        {
            target.AnimationIsRunning = true;
            target.GameObject.SetActive(true);
            RectTransform ui = target.GameObject.GetComponent<RectTransform>();

            Vector2 minAnchorSource = ui.anchorMin;
            Vector2 minAnchorDestination;

            Vector2 maxAnchorSource = ui.anchorMax;
            Vector2 maxAnchorDestination;

            if (toActivePosition)
            {
                minAnchorDestination = target.ActiveMinAnchor;

                maxAnchorDestination = target.ActiveMaxAnchor;
            }
            else
            {
                minAnchorDestination = target.DeactiveMinAnchor;

                maxAnchorDestination = target.DeactiveMaxAnchor;
            }

            float startTime = Time.time;
            float journeyLength = Vector3.Distance(minAnchorSource, minAnchorDestination);
            while (Vector2.Distance(ui.anchorMin, minAnchorDestination) > Mathf.Epsilon 
                    && Vector2.Distance(ui.anchorMax, maxAnchorDestination) > Mathf.Epsilon)
            {
                float distanceCovered = (Time.time - startTime) * target.AnimationSpeed;
                float fractionOfJourney = Mathf.Min(1, distanceCovered / journeyLength);
                ui.anchorMin = Vector2.Lerp(minAnchorSource, minAnchorDestination, fractionOfJourney);
                ui.anchorMax = Vector2.Lerp(maxAnchorSource, maxAnchorDestination, fractionOfJourney);
                yield return null;
            }
            ui.anchorMin = minAnchorDestination;
            ui.anchorMax = maxAnchorDestination;
            
            if (!toActivePosition)
                target.GameObject.SetActive(false);
            target.AnimationIsRunning = false;
        }

        LinearAnimationTarget target = targets[targetIndex];
        if (!target.AnimationIsRunning)
            StartCoroutine(Lerp(target, !target.GameObject.activeInHierarchy));
    }

    public void UIToActivePosition(int index)
    {
        if (targets[index].GameObject.activeInHierarchy)
        {
            return;
        }

        ToggleUI(index);
    }

    public void UIToDeactivePosition(int index)
    {
        if (!targets[index].GameObject.activeInHierarchy)
        {
            return;
        }

        ToggleUI(index);
    }

}
