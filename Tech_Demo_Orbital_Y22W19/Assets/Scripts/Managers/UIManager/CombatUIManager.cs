using CombatSystem.Effects;
using CombatSystem.Entities;
using CombatSystem.Facade;
using Managers;
using Managers.Subscribers;
using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class CombatUIManager : MonoBehaviour
    {
        public static readonly int DOLLY_DEGREES = -3;
        public static readonly float FAST_FOCUS_DURATION = 0.1f;

        private static CombatUIManager instance;
        public static CombatUIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<CombatUIManager>();
                    Debug.Assert(instance != null, "There was no CombatUIManager in the scene, consider adding one");
                }
                return instance;
            }
        }

        [SerializeField]
        private TransitionController characterViewTransitionController;

        [SerializeField]
        private TransitionController attackReviewTransitionController;

        [SerializeField]
        private TransitionController queueTransitionController;

        [SerializeField]
        private GameObject raycastBlocker;

        [SerializeField]
        private Interactable loseInteractable;

        [SerializeField]
        private Interactable winInteractable;

        [SerializeField]
        private CharacterStatsUIBehaviour[] actingUnitUI;

        [SerializeField]
        private CharacterStatsUIBehaviour[] opponentUnitUI;

        [SerializeField]
        private AttackReviewUIBehaviour[] attackReviewUI;

        [SerializeField]
        private Button buttonForBasicSkill;

        [SerializeField]
        private Button buttonForUltimateSkill;

        [SerializeField]
        private Image basicSkillIcon;

        [SerializeField]
        private Image ultimateSkillIcon;

        public TransitionController CharacterViewTransitionController => characterViewTransitionController;
        public TransitionController AttackReviewTransitionController => attackReviewTransitionController;
        public TransitionController QueueTransitionController => queueTransitionController;


        private KeyboardControls keyboardControls;

        private bool isPerformed = false;

        public void OnEnable()
        {
            keyboardControls.Enable();
        }

        public void OnDisable()
        {
            keyboardControls?.Disable();
        }

        private void Awake()
        {
            keyboardControls = new KeyboardControls();
            raycastBlocker.SetActive(false);
        }

        private void Start()
        {


            keyboardControls.Mouse.ShowCombatSelections.performed += _ =>
            {
                CombatSceneManager.Instance.UnsubscribeAllControls();

                AuxillarySubscribers.SubscribeToCharacterMenu(
                    CombatSceneManager.Instance.CurrentActingUnitPosition,
                    FAST_FOCUS_DURATION,
                    DOLLY_DEGREES,
                    CharacterViewTransitionController);
                isPerformed = true;
                raycastBlocker.SetActive(true);

                Unit currentUnit = CombatSceneManager.Instance.CurrentActingUnit;

                StatusEffect basicEffect = UnitStatusEffectsFacade.Instance.GetEffect(currentUnit.BasicSkillName);
                basicSkillIcon.sprite = basicEffect.Icon;

                if (currentUnit.CurrentSkillPoints < basicEffect.SkillPointCost)
                {
                    buttonForBasicSkill.interactable = false;
                } 
                else
                {
                    buttonForBasicSkill.interactable = true;
                }

                StatusEffect ultimateEffect = UnitStatusEffectsFacade.Instance.GetEffect(currentUnit.UltimateSkillName);

                ultimateSkillIcon.sprite = ultimateEffect.Icon;

                if (currentUnit.CurrentSkillPoints < ultimateEffect.SkillPointCost)
                {
                    buttonForUltimateSkill.interactable = false;
                }
                else
                {
                    buttonForUltimateSkill.interactable = true;
                }

            };

            keyboardControls.Mouse.ShowCombatSelections.canceled += _ =>
            {
                if (isPerformed)
                {
                    isPerformed = false;
                    AuxillarySubscribers.UnsubscribeToCharacterMenu(CharacterViewTransitionController);
                    raycastBlocker.SetActive(false);
                }
            };
        }

        public void ShowLoseScreen()
        {
            loseInteractable.GetComponent<InitialiseInteractable>().enabled = true;
        }

        public void ShowWinScreen()
        {
            winInteractable.GetComponent<InitialiseInteractable>().enabled = true;
        }

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

        public void UpdateAttackReviewInformation(int potentialDamageDealt, float chanceToHit)
        {
            foreach (AttackReviewUIBehaviour ui in attackReviewUI)
            {
                ui.SetStats(potentialDamageDealt, chanceToHit);
            }
        }

        public void SetAttackReviewVisibility(bool state)
        {
            AttackReviewTransitionController.SetAllTransitions(state);
            QueueTransitionController.SetAllTransitions(!state);
        }
    }
}
