using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGraph<T> : ICollection<T>, IConnector<T>
{
    public bool HasCycle();

    public int GetMaximumDegree();

    public int GetDiameter();

    public bool IsConnected();
}
