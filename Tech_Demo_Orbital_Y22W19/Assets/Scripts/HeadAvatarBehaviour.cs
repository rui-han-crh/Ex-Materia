using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadAvatarBehaviour : MonoBehaviour
{
    [SerializeField]
    private Image avatar;
    [SerializeField]
    private BarFillBehaviour healthBar;
    [SerializeField]
    private GameObject boundGameObject;

    public GameObject BoundGameObject => boundGameObject;

    public void SetBoundGameObject(GameObject unitGO)
    {
        boundGameObject = unitGO;
        avatar.sprite = boundGameObject.GetComponent<UnitBehaviour>().CharacterAvatar;   
    }

    public void UpdateHealthBar(Unit unit)
    {
        healthBar.UpdateBarFillImage(unit.Health, unit.MaxHealth);
    }
}
