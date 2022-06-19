using CombatSystem.Entities;
using Managers;
using Managers.Subscribers;
using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;


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
        private CharacterStatsUIBehaviour[] actingUnitUI;

        [SerializeField]
        private CharacterStatsUIBehaviour[] opponentUnitUI;

        [SerializeField]
        private AttackReviewUIBehaviour[] attackReviewUI;


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
        }

        private void Start()
        {
            keyboardControls.Mouse.ShowCombatSelections.performed += _ =>
            {
                CombatSceneManager.Instance.StateReset();

                AuxillarySubscribers.SubscribeToCharacterMenu(
                    CombatSceneManager.Instance.CurrentActingUnitPosition,
                    FAST_FOCUS_DURATION,
                    DOLLY_DEGREES,
                    CharacterViewTransitionController);
                isPerformed = true;
                raycastBlocker.SetActive(true);
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
