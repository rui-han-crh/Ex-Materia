using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
    public interface IGraphableNode<T>
    {
        public T GetParent();
    }
}
