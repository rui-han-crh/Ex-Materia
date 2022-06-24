using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
    public interface ITree<T> : IGraphable<T>
    {
        T GetParent(T data);

        T Root { get; }

        void Reparent(T existingChild, T newParent);
    }
}
