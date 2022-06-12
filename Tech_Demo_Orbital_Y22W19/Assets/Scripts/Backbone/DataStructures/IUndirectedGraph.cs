using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
    public interface IUndirectedGraph<T> : IGraphable<T>
    {
        public IUndirectedGraph<T> ContractEdgeBetween(T a, T b, T c);


    }
}
