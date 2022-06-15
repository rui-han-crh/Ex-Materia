using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Entities;

namespace Facades
{
    public class UnitFacade : MonoBehaviour
    {
        [SerializeField]
        private int maxHealth,
            currentHealth,
            range,
            attack,
            defence,
            risk,
            maxActionPoints,
            currentActionPoints;
    }
}
