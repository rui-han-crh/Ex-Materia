using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttackReviewUIBehaviour : MonoBehaviour
{
    [SerializeField]
    private TMP_Text potentialDamageText;

    [SerializeField]
    private TMP_Text chanceToHitText;

    private static readonly int ONE_HUNDRED_PERCENT = 100;

    public void SetStats(int potentialDamageDealt, float chanceToHit)
    {
        potentialDamageText.text = potentialDamageDealt.ToString();
        chanceToHitText.text = (chanceToHit * ONE_HUNDRED_PERCENT).ToString("F2");
    }
}
