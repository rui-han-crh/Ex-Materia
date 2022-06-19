using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transitions
{
    public class TransitionController : MonoBehaviour
    {
        [SerializeField]
        [RequireInterface(typeof(ITransition))]
        private MonoBehaviour[] transitions;


        public void PlayAnimations()
        {
            foreach (ITransition transition in transitions)
            {
                transition.ToggleAnimation();
            }
        }

        public void SetAllTransitions(bool state)
        {
            foreach (ITransition transition in transitions)
            {
                transition.SetAnimationState(state);
            }
        }

        public void PlaySingleAnimation(int index)
        {
            ((ITransition)transitions[index]).ToggleAnimation();
        }
    }
}
