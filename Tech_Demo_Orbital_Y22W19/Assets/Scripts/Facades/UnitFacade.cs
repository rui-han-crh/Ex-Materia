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

        [SerializeField]
        private Unit.UnitFaction faction;

        [SerializeField]
        private Sprite avatar;

        public Sprite CharacterAvatar => avatar;

        public Unit CreateUnit()
        {
            UnitProperties properties = new UnitProperties(
                maxHealth,
                currentHealth,
                defence,
                attack,
                range,
                maxActionPoints,
                currentActionPoints,
                risk);

            return Unit.CreateNewUnit(gameObject.name, properties, 0, faction);
        }
    }
}
