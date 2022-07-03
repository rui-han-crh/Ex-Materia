using CombatSystem.Entities;
using Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField]
    private TMP_Text timeText;

    private int identity = -1;

    public GameObject BoundGameObject => boundGameObject;

    public int BoundIdentity 
    {
        get
        {
            if (identity == -1)
            {
                Debug.LogError("The identity for this object has not yet been set");
            }
            return identity;
        }
    }


    public void SetBoundGameObject(GameObject unitGO)
    {
        boundGameObject = unitGO;
        avatar.sprite = boundGameObject.GetComponent<UnitBehaviour>().CharacterAvatar;   
    }

    public void SetBoundIdentity(int identity)
    {
        if (this.identity != -1)
        {
            Debug.LogError($"The identity for this object has already been bound to {identity}");
            return;
        }

        this.identity = identity;
        avatar.sprite = UnitManager.Instance[identity].CharacterAvatar;
    }

    public void SetTimeText(int time)
    {
        timeText.text = time.ToString();
    }

    public void UpdateHealthBar(UnitOld unit)
    {
        healthBar.UpdateBarFillImage(unit.Health, unit.MaxHealth);
    }

    public void UpdateHealthBar(Unit unit)
    {
        healthBar.UpdateBarFillImage(unit.CurrentHealth, unit.MaxHealth);
    }
}
