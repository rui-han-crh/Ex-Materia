using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Entities
{
    public struct TileData
    {
        public enum TileType
        {
            None,
            Ground,
            Restricted,
            HalfCover,
            FullCover
        }

        public int Cost
        {
            get;
            private set;
        }

        public TileType Type
        {
            get;
            private set;
        }

        public TileData(int cost, TileType type) : this()
        {
            Cost = cost;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            return obj is TileData data && Equals(data);
        }

        public bool Equals(TileData other)
        {
            return other.Cost == Cost && other.Type == Type;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(TileData lhs, TileData rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(TileData lhs, TileData rhs)
        {
            return !lhs.Equals(rhs);
        }

    }
}
