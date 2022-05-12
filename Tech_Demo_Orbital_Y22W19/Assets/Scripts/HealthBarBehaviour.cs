using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class HealthBarBehaviour : MonoBehaviour
{
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private TMP_Text healthBarText;
    private float fullWidth;

    private void Awake()
    {
        fullWidth = healthBar.rectTransform.anchorMax.x - healthBar.rectTransform.anchorMin.x;
    }

    public void UpdateHealthBarImage(int currentHealth, int totalHealth)
    {
        healthBarText.text = $"{currentHealth} / {totalHealth}";
        Vector2 newAnchorMax = healthBar.rectTransform.anchorMax;
        newAnchorMax.x = fullWidth * ((float) currentHealth / totalHealth);
        healthBar.rectTransform.anchorMax = newAnchorMax;
    }
}