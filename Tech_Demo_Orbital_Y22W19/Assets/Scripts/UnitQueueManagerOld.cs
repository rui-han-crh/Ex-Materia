using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

/// <summary>
/// A specific structure that managed Unit turns
/// 1. A unit is OBSERVED to take the next turn 
/// 2. It moves a certain amount and incurs a COST
/// 3. It is shifted to the correct place in the structure
/// 
/// Unit will ONLY be removed from the structure if dead.
/// </summary>
public class UnitQueueManagerOld : MonoBehaviour
{
    private static UnitQueueManagerOld instance;

    public static UnitQueueManagerOld Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UnitQueueManagerOld>();
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
    [SerializeField]
    private RectTransform waitSlider;

    private bool isPlayingAnimation;
    public bool IsPlayingAnimation => isPlayingAnimation;


    public void InitialiseQueue(GameObject[] units, Dictionary<string, UnitOld> nameUnitMapping)
    {
        foreach (GameObject unit in units)
        {
            GameObject characterHead = Instantiate(characterHeadPrefab, avatarHolder);
            characterHead.name = characterHeadPrefab.name + "_" + unit.name;
            characterHead.GetComponent<HeadAvatarBehaviour>().SetBoundGameObject(unit);
        }

        new Task(LateInvoke(UpdateUnitQueue, nameUnitMapping));

    }

    public void UpdateUnitQueue(Dictionary<string, UnitOld> nameUnitMapping)
    {
        int numberOfUnitsRemoved = RemoveDeadUnitsAvatar(nameUnitMapping);

        LinearAnimation linAnim = GetComponent<LinearAnimation>();

        List<LinearAnimation.LinearAnimationTarget> targets = new List<LinearAnimation.LinearAnimationTarget>();

        UnitOld[] orderedUnit = nameUnitMapping.Values.OrderBy(x => x.Time).ToArray();

        Dictionary<string, int> unitOrder = new Dictionary<string, int>();

        for (int i = 0; i< orderedUnit.Length; i++)
        {
            unitOrder.Add(orderedUnit[i].Name, i);
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

            if (!nameUnitMapping.ContainsKey(characterHeadAvatar.BoundGameObject.name))
            {
                continue;
            }

            UnitOld unit = nameUnitMapping[characterHeadAvatar.BoundGameObject.name];
            characterHeadAvatar.UpdateHealthBar(unit);

            int index = unitOrder[characterHeadAvatar.BoundGameObject.name];
            if (unit.Time == GameManagerOld.Instance.CurrentUnit.Time)
            {
                skippedUnitsForWaiting++;
            }

            targets.Add(new LinearAnimation.LinearAnimationTarget(character.gameObject,
                                                                minOrigin + index * displacement,
                                                                minOrigin + (index + 1) * displacement + Vector2.up,
                                                                1));
            character.gameObject.SetActive(false);
        }

        SetWaitSliderLength(minOrigin.x + displacement.x * skippedUnitsForWaiting, displacement.x * unitOrder.Count);

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
    /// <param name="nameUnitMapping"></param>
    /// <returns>The number of units avatars that have been removed.</returns>
    private int RemoveDeadUnitsAvatar(Dictionary<string, UnitOld> nameUnitMapping)
    {
        int numberOfUnitsRemoved = 0;
        foreach (Transform character in avatarHolder)
        {
            HeadAvatarBehaviour characterHeadAvatar = character.GetComponent<HeadAvatarBehaviour>();

            if (!nameUnitMapping.ContainsKey(characterHeadAvatar.BoundGameObject.name))
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
        while(targets.Any(x => x.AnimationIsRunning))
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
}
