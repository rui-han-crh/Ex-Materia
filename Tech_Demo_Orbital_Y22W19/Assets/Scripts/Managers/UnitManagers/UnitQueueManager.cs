using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using CombatSystem.Entities;
using UnityEngine.UI;
using Managers;

[RequireComponent(typeof(LinearAnimation))]
public class UnitQueueManager : MonoBehaviour
{
    private static UnitQueueManager instance;

    public static UnitQueueManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UnitQueueManager>();
                Debug.Assert(instance != null, "There is no UnitQueueManager in this scene, consider adding one");
            }
            return instance;
        }
    }


    [SerializeField]
    private Transform avatarHolder;
    [SerializeField]
    private Transform content;
    [SerializeField]
    private GameObject characterHeadPrefab;

    //Unused
    private RectTransform waitSlider;

    private bool isPlayingAnimation;
    public bool IsPlayingAnimation => isPlayingAnimation;


    public void InitialiseQueue(IEnumerable<Unit> units)
    {
        foreach (Unit unit in units)
        {
            GameObject characterHead = Instantiate(characterHeadPrefab, avatarHolder);
            characterHead.name = characterHeadPrefab.name + "_" + unit.Name;
            characterHead.GetComponent<HeadAvatarBehaviour>().SetBoundIdentity(unit.Identity);
        }

        new Task(LateInvoke(UpdateUnitQueue, units));

    }

    public void UpdateUnitQueue(IEnumerable<Unit> units)
    {
        Dictionary<int, Unit> uniqueIdentities = units.ToDictionary(unit => unit.Identity, unit => unit);

        int numberOfUnitsRemoved = RemoveDeadUnitsAvatar(uniqueIdentities);

        LinearAnimation linAnim = GetComponent<LinearAnimation>();

        List<LinearAnimation.LinearAnimationTarget> targets = new List<LinearAnimation.LinearAnimationTarget>();

        IEnumerable<Unit> orderedUnit = units.OrderBy(unit => unit.Time);

        Dictionary<int, int> unitIdentityOrder = new Dictionary<int, int>();

        for (int i = 0; i < orderedUnit.Count(); i++)
        {
            unitIdentityOrder[orderedUnit.ElementAt(i).Identity] = i;
        }

        RectTransform characterHeadRectTransform = avatarHolder.GetChild(0).GetComponent<RectTransform>();
        RectTransform contentTransform = content.GetComponent<RectTransform>();

        Vector2 size = contentTransform.sizeDelta;
        int childCount = avatarHolder.childCount - numberOfUnitsRemoved;
        size.x = characterHeadRectTransform.rect.width * childCount;

        contentTransform.sizeDelta = size;

        Vector2 minOrigin = Vector2.zero;
        Vector2 displacement = new Vector2(1.0f / childCount, 0);

        int skippedUnitsForWaiting = 0;

        foreach (Transform character in avatarHolder)
        {
            HeadAvatarBehaviour characterHeadAvatar = character.GetComponent<HeadAvatarBehaviour>();

            if (!uniqueIdentities.ContainsKey(characterHeadAvatar.BoundIdentity))
            {
                continue;
            }

            Unit unit = uniqueIdentities[characterHeadAvatar.BoundIdentity];

            characterHeadAvatar.UpdateHealthBar(unit);
            characterHeadAvatar.SetTimeText(unit.Time);

            int index = unitIdentityOrder[characterHeadAvatar.BoundIdentity];

            if (unit.Time == CombatSceneManager.Instance.CurrentActingUnit.Time)
            {
                skippedUnitsForWaiting++;
            }

            targets.Add(new LinearAnimation.LinearAnimationTarget(character.gameObject,
                                                                minOrigin + index * displacement,
                                                                minOrigin + (index + 1) * displacement + Vector2.up,
                                                                1));
            character.gameObject.SetActive(false);
        }

        //SetWaitSliderLength(minOrigin.x + displacement.x * skippedUnitsForWaiting, displacement.x * unitIdentityOrder.Count);

        isPlayingAnimation = true;
        linAnim.SetTargets(targets.ToArray());

        for (int i = 0; i < targets.Count; i++)
        {
            linAnim.ToggleUI(i);
        }

        AudioManager.Instance.PlayTrack("QueueUpdated");

        new Task(CheckAllTargetsStopped(targets));
    }

    /// <summary>
    /// From the queue describe the unit turns, removes the unit avatars that are no longer involved in
    /// the game map. The avatars will be removed on the next frame, when Update() is called.
    /// </summary>
    /// <param name="units"></param>
    /// <returns>The number of units avatars that have been removed.</returns>
    private int RemoveDeadUnitsAvatar(IDictionary<int, Unit> units)
    {
        int numberOfUnitsRemoved = 0;
        foreach (Transform character in avatarHolder)
        {
            HeadAvatarBehaviour characterHeadAvatar = character.GetComponent<HeadAvatarBehaviour>();

            if (!units.ContainsKey(characterHeadAvatar.BoundIdentity))
            {
                numberOfUnitsRemoved++;
                Destroy(characterHeadAvatar.gameObject);
                continue;
            }
        }
        return numberOfUnitsRemoved;
    }

    private IEnumerator CheckAllTargetsStopped(IEnumerable<LinearAnimation.LinearAnimationTarget> targets)
    {
        while (targets.Any(x => x.AnimationIsRunning))
        {
            yield return null;
        }
        isPlayingAnimation = false;
    }

    private IEnumerator LateInvoke<T>(Action<T> func, T args)
    {
        yield return new WaitForEndOfFrame();
        func.Invoke(args);
    }

    private void SetWaitSliderLength(float minX, float maxX)
    {
        waitSlider.anchorMin = new Vector2(minX, waitSlider.anchorMin.y);
        waitSlider.anchorMax = new Vector2(maxX, waitSlider.anchorMax.y);
    }

    //Unused
    public void AdjustSliderToUnitTime(Slider slider)
    {
        GameMap currentMap = CombatSceneManager.Instance.CurrentMap;
        IEnumerable<Unit> orderedUnitsByTime = currentMap.GetUnits(unit => true).OrderBy(unit => unit.Time);

        Unit currentUnit = CombatSceneManager.Instance.CurrentActingUnit;

        int numberOfUnits = orderedUnitsByTime.Count();

        float positionInQueue = numberOfUnits * slider.value;

        int numberOfUnitsPassed = Mathf.Min((int)(positionInQueue), numberOfUnits - 1);
        float excess = positionInQueue - numberOfUnitsPassed;

        int timeAfter = numberOfUnitsPassed == numberOfUnits - 1 ?
                            Mathf.Max(orderedUnitsByTime.Last().Time + 1, (currentUnit.MaxActionPoints - currentUnit.CurrentActionPoints) + currentUnit.Time) :
                            orderedUnitsByTime.ElementAt(numberOfUnitsPassed + 1).Time;

        int timeBefore = orderedUnitsByTime.ElementAt(numberOfUnitsPassed).Time;

        int offsetTime = timeBefore - currentUnit.Time;

        int differenceTime = Mathf.CeilToInt((timeAfter - timeBefore) * excess);

        int timeToWait = offsetTime + differenceTime;

        CombatSceneManager.Instance.SetTimeToWait(timeToWait);

        InformationUIManager.Instance.SetTimeAndAPRequiredText(timeToWait, 0);
    }
}
