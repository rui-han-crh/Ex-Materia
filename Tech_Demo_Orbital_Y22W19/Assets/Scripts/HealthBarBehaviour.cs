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
    private Transform parentFollowing;
    private Camera mainCamera;

    private void Awake()
    {
        fullWidth = healthBar.rectTransform.anchorMax.x - healthBar.rectTransform.anchorMin.x;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (parentFollowing != null)
        {
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(parentFollowing.transform.position + GameManager.HEALTH_BAR_WORLD_OFFSET);
            GetComponent<RectTransform>().position = screenPoint;
        }
    }

    public void SetParent(Transform parent)
    {
        parentFollowing = parent;
    }

    public void UpdateHealthBarImage(int currentHealth, int totalHealth)
    {
        healthBarText.text = $"{currentHealth} / {totalHealth}";
        Vector2 newAnchorMax = healthBar.rectTransform.anchorMax;
        newAnchorMax.x = fullWidth * ((float) currentHealth / totalHealth);
        healthBar.rectTransform.anchorMax = newAnchorMax;
    }
}