using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
    public interface IParentable<T>
    {
        public void SetParent(IParentable<T> node);

        public void RemoveParent();

        public IParentable<T> GetParent();
    }
}
