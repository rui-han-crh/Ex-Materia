using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDirectedGraph<T> : ICollection<T>, IConnector<T>, IGraph<T>
{
    public bool IsStronglyConnected();
    public IEnumerable<IEnumerable<T>> GetStronglyConnectedComponents();
    public IEnumerable<IEnumerable<T>> GetWeaklyConnectedComponents();
    public IDirectedGraph<T> Transpose();
}
