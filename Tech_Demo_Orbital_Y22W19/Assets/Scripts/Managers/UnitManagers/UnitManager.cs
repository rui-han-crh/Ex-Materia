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
        public UnitFacade[] unitFacades = new UnitFacade[0];

        private Unit[] units;

        private Tilemap ground;


        public UnitFacade this[int identity]
        {
            get => unitFacades[identity-1];
        }

        private void Awake()
        {

            units = new Unit[unitFacades.Length];

            if (unitFacades.Length == 0)
            {
                Debug.LogWarning("There were no units added to the UnitManager. Was this intended?");
            }

            for (int i = 0; i < unitFacades.Length; i++)
            {
                Unit unit = unitFacades[i].CreateUnit();
                units[i] = unit;
            }

            ground = TileManager.Instance.Ground;
        }

        public UnitCensus CreateUnitCensus()
        {
            Dictionary<Vector3Int, Unit> positionToUnitMapping = new Dictionary<Vector3Int, Unit>();

            for (int i = 0; i < units.Length; i++)
            {
                Vector3Int cellPosition = ground.WorldToCell(unitFacades[i].gameObject.transform.position);
                positionToUnitMapping.Add(cellPosition, units[i]);
                unitFacades[i].gameObject.transform.position = ground.CellToWorld(cellPosition);
            }

            return new UnitCensus(positionToUnitMapping);
        }

        public GameObject GetGameObjectOfUnit(Unit unit)
        {
            return unitFacades[unit.Identity - 1].gameObject;
        }

        public void RemoveUnit(Unit unit, float delay = 0)
        {
            StartCoroutine(DestroyUnitGameObject(unit, delay));
        }

        private IEnumerator DestroyUnitGameObject(Unit unit, float delay)
        {
            yield return new WaitForSeconds(delay);
            unitFacades[unit.Identity - 1].gameObject.SetActive(false);
        }
    }
}