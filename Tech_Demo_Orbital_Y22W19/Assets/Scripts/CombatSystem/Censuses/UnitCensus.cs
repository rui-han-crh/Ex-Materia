using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BidirectionalMap;
using Entities;
using BidirectionalMap.ExtensionMethods;
using System.Linq;
using System;
using System.Linq.Extensions;

namespace Censuses
{
    public class UnitCensus
    {
        private readonly BiMap<Vector3Int, Unit> census = new BiMap<Vector3Int, Unit>();

        public IEnumerable<Unit> Census => census.Reverse.Keys;

        public UnitCensus(IDictionary<Vector3Int, Unit> positionToUnitMapping)
        {
            census = new BiMap<Vector3Int, Unit>(positionToUnitMapping);
        }

        public UnitCensus(BiMap<Vector3Int, Unit> newCensus)
        {
            census = newCensus;
        }

        private UnitCensus(UnitCensus previousCensus)
        {
            this.census = previousCensus.census;
        }

        public UnitCensus()
        {

        }

        public Unit this[Vector3Int position]
        {
            get
            {
                return census.Forward[position];
            }
        }

        public int Count => census.Count();

        public Vector3Int this[Unit unit]
        {
            get
            {
                return census.Reverse[unit];
            }
        }

        public UnitCensus Add(Vector3Int position, Unit unit)
        {
            if (census.Forward.ContainsKey(position) || census.Reverse.ContainsKey(unit))
            {
                throw new ArgumentException($"{position} or {unit} already exists in the census");
            }

            BiMap<Vector3Int, Unit> newCensus = census.Clone();
            newCensus.Add(position, unit);
            return new UnitCensus(newCensus);
        }

        private void AddMutator(Vector3Int position, Unit unit)
        {
            if (census.Forward.ContainsKey(position) || census.Reverse.ContainsKey(unit))
            {
                throw new ArgumentException($"{position} or {unit} already exists in the census");
            }

            census.Add(position, unit);
        }

        public UnitCensus Remove(Vector3Int position)
        {
            if (!census.Forward.ContainsKey(position))
            {
                throw new KeyNotFoundException($"{position} does not exist in the census");
            }

            BiMap<Vector3Int, Unit> newCensus = census.Clone();
            newCensus.Remove(position);
            return new UnitCensus(newCensus);
        }

        private void RemoveMutator(Vector3Int position)
        {
            if (!census.Forward.ContainsKey(position))
            {
                throw new KeyNotFoundException($"{position} does not exist in the census");
            }

            census.Remove(position);
        }


        public UnitCensus Remove(Unit unit)
        {
            if (!census.Reverse.ContainsKey(unit))
            {
                throw new KeyNotFoundException($"{unit} does not exists in the census");
            }

            BiMap<Vector3Int, Unit> newCensus = census.Clone();
            newCensus.Remove(census.Reverse[unit]);
            return new UnitCensus(newCensus);
        }


        private void RemoveMutator(Unit unit)
        {
            if (!census.Reverse.ContainsKey(unit))
            {
                throw new KeyNotFoundException($"{unit} does not exists in the census");
            }

            census.Remove(census.Reverse[unit]);
        }


        public bool Contains(Vector3Int position)
        {
            return census.Forward.ContainsKey(position);
        }

        public bool Contains(Unit unit)
        {
            return census.Reverse.ContainsKey(unit);
        }

        public UnitCensus Clone()
        {
            return new UnitCensus(this);
        }

        public UnitCensus MoveUnit(Unit unit, Vector3Int position)
        {
            UnitCensus newCensus = Remove(unit);
            newCensus.AddMutator(position, unit);
            return newCensus;
        }

        public UnitCensus SwapUnit(Unit oldUnit, Unit newUnit)
        {
            Vector3Int position = this[oldUnit];
            UnitCensus newCensus = Remove(oldUnit);
            newCensus.AddMutator(position, newUnit);
            return newCensus;
        }

        public UnitCensus UpdateUnit(Unit unit)
        {

            Vector3Int position = this[unit];
            UnitCensus newCensus = Remove(unit);
            newCensus.AddMutator(position, unit);
            return newCensus;
        }

        public Unit GetUnitWithLeastTime()
        {
            return census.Reverse.Keys.MinBy(x => x.Time);
        }

    }
}
