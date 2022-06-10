using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConnector<T>
{
    public bool Connect(T a, T b);

    public bool Disconnect(T a, T b);

    public bool IsConnected(T a, T b);

    public IEnumerable<T> GetConnected(T item);
}
