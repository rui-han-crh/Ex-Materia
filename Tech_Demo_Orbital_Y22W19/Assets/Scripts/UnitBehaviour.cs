using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitBehaviour : MonoBehaviour
{
    private Unit unit;

    [SerializeField]
    private int health = 100;

    [SerializeField]
    private int startingExhaustion = 1;

    [SerializeField]
    private int attack = 20;

    [SerializeField]
    private int defence = 10;

    [SerializeField]
    private Sprite characterHeadAvatar;

    [SerializeField]
    private int actionPointsPerTurn;

    [SerializeField]
    private Faction faction;

    private GameObject healthBarElement;

    private static Vector3 HEALTH_BAR_OFFSET = new Vector2(0, 0.9f);

    // Start is called before the first frame update
    private void Awake()
    {
        unit = new Unit(
            this, 
            name,
            health,
            faction, 
            characterHeadAvatar, 
            startingExhaustion,
            attack,
            defence,
            actionPointsPerTurn);
    }

    public Unit Unit => unit;

    public Faction Faction => faction;

    public void AssignHealthBarElement(GameObject healthBar)
    {
        healthBarElement = healthBar;
    }

    public void UpdateHealthBar()
    {
        healthBarElement.GetComponent<HealthBarBehaviour>().UpdateHealthBarImage(unit.Health, health);
    }

    private void Update()
    {
        if (healthBarElement != null) {
            healthBarElement.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(transform.position + HEALTH_BAR_OFFSET);
        }
    }
}
