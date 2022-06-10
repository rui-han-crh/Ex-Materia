using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
    public interface IParentable<T>
    {
        public bool AddParent(T parent);

        public bool RemoveParent(T parent);

        public T GetParent();
    }
}
