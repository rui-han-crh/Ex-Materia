using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
    public interface IGraphable<T> : ICollection<T>, IConnector<T>
    {
        public bool HasCycle();

        public int GetMaximumDegree();

        public int GetDiameter();

        public IEnumerable<IEnumerable<T>> GetConnectedComponents();

        public bool IsConnected();

        public bool CanWalkFromTo(T a, T b);
    }
}
