using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
    public interface IDirectable<T>
    {
        public bool AddIncoming(T incoming);

        public bool RemoveIncoming(T incoming);

        public bool AddOutgoing(T outgoing);

        public bool RemoveOutgoing(T outgoing);
    }
}
