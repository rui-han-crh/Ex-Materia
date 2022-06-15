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
        [SerializeField]
        private UnitFacade[] unitFacades;
        private Unit[] units;

        [SerializeField]
        private Tilemap ground;

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
    }
}