using CombatSystem.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class CharacterInformationManager : MonoBehaviour
    {
        private static CharacterInformationManager instance;
        public static CharacterInformationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<CharacterInformationManager>();
                    Debug.Assert(instance != null, "There was no CharacterInformationManager in the scene, consider adding one");
                }
                return instance;
            }
        }


        [SerializeField]
        public CharacterStatsUIBehaviour[] actingUnitUI;

        [SerializeField]
        public CharacterStatsUIBehaviour[] opponentUnitUI;

        public void UpdateCurrentActingUnitInformation()
        {
            foreach (CharacterStatsUIBehaviour ui in actingUnitUI)
            {
                ui.SetUnitStats(CombatSceneManager.Instance.CurrentActingUnit);
            }
        }

        public void UpdateOpponentUnitInformation(Unit opponent)
        {
            foreach (CharacterStatsUIBehaviour ui in opponentUnitUI)
            {
                ui.SetUnitStats(opponent);
            }
        }

    }
}
