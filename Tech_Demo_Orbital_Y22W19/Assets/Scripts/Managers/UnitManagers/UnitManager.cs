using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facades;
using CombatSystem.Entities;
using CombatSystem.Censuses;
using UnityEngine.Tilemaps;


namespace Managers
{
    public class UnitManager : MonoBehaviour
    {
        private static UnitManager instance;
        public static UnitManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UnitManager>();
                }
                return instance;
            }
        }

        [SerializeField]
        private UnitFacade[] unitFacades;
        private Unit[] units;

        [SerializeField]
        private Tilemap ground;

        public UnitFacade this[int identity]
        {
            get => unitFacades[identity-1];
        }

        private void Awake()
        {
            units = new Unit[unitFacades.Length];
            for (int i = 0; i < unitFacades.Length; i++)
            {
                Unit unit = unitFacades[i].CreateUnit();
                units[i] = unit;
            }
        }

        public UnitCensus CreateUnitCensus()
        {
            Dictionary<Vector3Int, Unit> positionToUnitMapping = new Dictionary<Vector3Int, Unit>();

            for (int i = 0; i < units.Length; i++)
            {
                positionToUnitMapping.Add(ground.WorldToCell(unitFacades[i].gameObject.transform.position), units[i]);
            }

            return new UnitCensus(positionToUnitMapping);
        }

        public GameObject GetGameObjectOfUnit(Unit unit)
        {
            return unitFacades[unit.Identity - 1].gameObject;
        }

        public void RemoveUnit(Unit unit, float delay = 0)
        {
            if (delay == 0)
            {
                Destroy(unitFacades[unit.Identity - 1].gameObject);
            } else
            {
                StartCoroutine(DestroyUnitGameObject(unit, delay));
            }
        }

        private IEnumerator DestroyUnitGameObject(Unit unit, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(unitFacades[unit.Identity - 1].gameObject);
        }
    }
}