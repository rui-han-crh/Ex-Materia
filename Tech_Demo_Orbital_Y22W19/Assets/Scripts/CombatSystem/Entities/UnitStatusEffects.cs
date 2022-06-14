using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    [Flags]
    public enum UnitStatusEffects
    {
        // Set to powers of 2
        None = 0,
        Overwatch = 1,
        All = ~0
    }
}
