using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures
{
    public interface IConnectable<T>
    {
        public bool Connect(T other);

        public bool Disconnect(T other);

        public bool IsConnectedWith(T other);

        public IEnumerable<T> GetConnected();
    }
}
