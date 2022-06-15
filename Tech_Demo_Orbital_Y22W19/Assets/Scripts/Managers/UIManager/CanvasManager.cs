using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class CanvasManager : MonoBehaviour
    {
        private static CanvasManager instance;
        public static CanvasManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<CanvasManager>();
                    Debug.Assert(instance != null, "There is no CanvasManager in the scene, consider adding one");
                }
                return instance;
            }
        }

        public enum UIType
        {
            CharacterSheet = 0,
            OpponentSheet = 1,
            Queue = 2
        }

        private LinearAnimation linearAnimation;

        private void Awake()
        {
            linearAnimation = GetComponent<LinearAnimation>();
        }

        public void DeactivateUI(UIType type)
        {
            linearAnimation.UIToDeactivePosition((int)type);
        }

        public void ActivateUI(UIType type)
        {
            linearAnimation.UIToActivePosition((int)type);
        }

        public void ToggleUI(UIType type)
        {
            linearAnimation.ToggleUI((int)type);
        }
    }
}
