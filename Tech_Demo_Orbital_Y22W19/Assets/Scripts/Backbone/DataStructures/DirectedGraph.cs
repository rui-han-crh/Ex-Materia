using Algorithms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataStructures
{
    public class DirectedGraph<T> : Graph<T>, IDirectedGraph<T>
    {

        public DirectedGraph()
        {

        }

        public DirectedGraph(IEnumerable<T> items) : base(items)
        {

        }

        public IEnumerable<IEnumerable<T>> GetWeaklyConnectedComponents()
        {
            return GetConnectedComponents();
        }

        public IEnumerable<IEnumerable<T>> GetStronglyConnectedComponents()
        {
            return SearchAlgorithms.Kosaraju(this);
        }

        public bool IsStronglyConnected()
        {
            return GetStronglyConnectedComponents().Count() == 1;
        }

        public IDirectedGraph<T> Transpose()
        {
            DirectedGraph<T> graph = new DirectedGraph<T>(StoredItems.Keys);

            Dictionary<T, GraphNode> notVisited = new Dictionary<T, GraphNode>(StoredItems);

            while (notVisited.Count > 0)
            {
                SearchAlgorithms.DepthFirstSearch(notVisited.First().Value,
                                                    x => x.OutgoingNodes,
                                                    x => { notVisited.Remove(x.Data);
                                                        x.OutgoingNodes.ToList().ForEach(child => graph.Connect(child.Data, x.Data));
                                                    });
            }

            return graph;
        }

        public override bool CanWalkFromTo(T a, T b)
        {
            bool destinationReached = false;
            SearchAlgorithms.DepthFirstSearch(a, x => GetConnected(x), x => { if (x.Equals(b)) destinationReached = true; });
            return destinationReached;
        }
    }
}