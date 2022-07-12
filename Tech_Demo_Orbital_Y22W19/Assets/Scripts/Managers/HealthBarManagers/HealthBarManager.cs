using CombatSystem.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class HealthBarManager : MonoBehaviour
    {
        private static HealthBarManager instance;
        public static HealthBarManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<HealthBarManager>();
                    Debug.Assert(instance != null, "There is no HealthBarManager in the scene, consider adding one");
                }
                return instance;
            }
        }

        [SerializeField]
        private GameObject healthBarPrefab;

        [SerializeField]
        private RectTransform healthBarCollection;

        private List<BarFillBehaviour> healthBars = new List<BarFillBehaviour>() { null };

        private void Awake()
        {
            Debug.Log(healthBarPrefab);
            if (healthBarCollection == null)
            {
                GameObject healthBarCollectionGameObject = GameObject.Find("Healthbar_Collection");
                if (healthBarCollectionGameObject == null)
                {
                    healthBarCollectionGameObject = new GameObject("Healthbar_Collection", typeof(RectTransform));
                    healthBarCollectionGameObject.transform.SetParent(InteractableCollection.Instance.Canvas.transform, false);
                    healthBarCollectionGameObject.transform.SetAsLastSibling();
                }

                healthBarCollection = healthBarCollectionGameObject.GetComponent<RectTransform>();
                healthBarCollection.anchorMin = Vector2.zero;
                healthBarCollection.anchorMax = Vector2.one;
                healthBarCollection.offsetMax = Vector2.zero;
                healthBarCollection.offsetMin = Vector2.zero;

                Debug.Assert(healthBarCollection != null, "Could not find a RectTransform on a GameObject called Healthbar_Collection. " +
                    "Either drag a Transform, as a child of the main Canvas, to gather all healthbar gameobjects onto, " +
                    "or rename the intended gameobject to \"Healthbar_Collection so the script can retrieve it.");
            }
        }

        public void InitialiseHealthBars(IEnumerable<Unit> units)
        {
            foreach (Unit unit in units) 
            {
                GameObject healthBar = Instantiate(healthBarPrefab, healthBarCollection);
                BarFillBehaviour barFillBehaviour = healthBar.GetComponent<BarFillBehaviour>();
                barFillBehaviour.SetParent(UnitManager.Instance.GetGameObjectOfUnit(unit).transform);
                healthBars.Add(barFillBehaviour);
            }
            UpdateHealthBars(units);
        }

        public BarFillBehaviour GetHealthBar(Unit unit)
        {
            return healthBars[unit.Identity];
        }

        public void DisableHealthBar(Unit unit, float delay)
        {
            StartCoroutine(DisableHealthBarEnumerator(unit, delay));
        }

        private IEnumerator DisableHealthBarEnumerator(Unit unit, float delay)
        {
            yield return new WaitForSeconds(delay);
            GetHealthBar(unit).gameObject.SetActive(false);
        }

        public void UpdateHealthBars(IEnumerable<Unit> units)
        {
            foreach (Unit unit in units)
            {
                BarFillBehaviour healthBar = GetHealthBar(unit);
                healthBar.UpdateBarFillImage(unit.CurrentHealth, unit.MaxHealth);
            }
        }
    }
}
