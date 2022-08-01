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
        private TransitionController characterViewController;

        [SerializeField]
        private TransitionController attackReviewController;

        [SerializeField]
        private TransitionController queueController;

        [SerializeField]
        private GameObject raycastBlocker;

        [SerializeField]
        private WaitMenuBehaviour waitMenuUI;

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

        [Header("Selection Button References")]
        [SerializeField]
        private Button movementButton;

        [SerializeField]
        private Button attackButton;

        [SerializeField]
        private Button attackConfirmationButton;

        [SerializeField]
        private Button waitButton;

        [SerializeField]
        private Button overwatchButton;

        [SerializeField]
        private Button buttonForBasicSkill;

        [SerializeField]
        private Button buttonForUltimateSkill;

        [Header("Skill Button Icons")]
        [SerializeField]
        private Image basicSkillIcon;

        [SerializeField]
        private Image ultimateSkillIcon;

        public TransitionController CharacterViewTransitionController => characterViewController;
        public TransitionController AttackReviewTransitionController => attackReviewController;
        public TransitionController QueueTransitionController => queueController;


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
            if (instance != null)
            {
                Debug.LogError("CombatUIManager is designed as a singleton, yet more than one instance was found during runtime. " +
                    "Please remove any additional instances and ensure there is only one instance of CombatUIManager");
            }

            keyboardControls = new KeyboardControls();
            raycastBlocker.SetActive(false);

            GameObject[] loseInteractables = GameObject.FindGameObjectsWithTag("BattleLostInteractable");
            Debug.Assert(loseInteractables.Length == 1, 
                $"There were {loseInteractables.Length} gameobjects tagged with BattleLostInteractable, but there can only be 1.");
            loseInteractable = loseInteractables[0].GetComponent<Interactable>();


            GameObject[] winInteractables = GameObject.FindGameObjectsWithTag("BattleWonInteractable");
            Debug.Assert(winInteractables.Length == 1,
                $"There were {winInteractables.Length} gameobjects tagged with BattleWonInteractable, but there can only be 1.");
            winInteractable = winInteractables[0].GetComponent<Interactable>();

            movementButton.onClick.AddListener(() => CombatSceneManager.Instance.BeginMovement());

            attackButton.onClick.AddListener(() => CombatSceneManager.Instance.BeginCombat());

            attackConfirmationButton.onClick.AddListener(() => CombatSceneManager.Instance.SelectCommand(CombatSceneManager.CommandType.Attack));

            overwatchButton.onClick.AddListener(() => CombatSceneManager.Instance.SelectCommand(CombatSceneManager.CommandType.Overwatch));

            buttonForBasicSkill.onClick.AddListener(() => CombatSceneManager.Instance.SelectCommand(CombatSceneManager.CommandType.BasicSkill));

            buttonForUltimateSkill.onClick.AddListener(() => CombatSceneManager.Instance.SelectCommand(CombatSceneManager.CommandType.UltimateSkill));

            queueController ??= GameObject.Find("QueueView").GetComponent<TransitionController>();
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
                    waitMenuUI?.CancelMenu();
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
