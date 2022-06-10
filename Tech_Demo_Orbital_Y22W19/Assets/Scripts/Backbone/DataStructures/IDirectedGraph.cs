using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
    public interface IDirectedGraph<T> : IGraphable<T>
    {
        public bool IsStronglyConnected();
        public IEnumerable<IEnumerable<T>> GetStronglyConnectedComponents();
        public IEnumerable<IEnumerable<T>> GetWeaklyConnectedComponents();
        public IDirectedGraph<T> Transpose();
    }
}
