using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Transitions
{
    public interface ITransition
    {
        bool IsActive { get; }

        void ToggleAnimation();

        void SetAnimationState(bool state);
    }
}
