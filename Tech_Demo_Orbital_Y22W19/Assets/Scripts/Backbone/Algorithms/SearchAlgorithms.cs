using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Algorithms
{
    public class SearchAlgorithms
    {
        public static void DepthFirstSearch<T>(T root, Func<T, IEnumerable<T>> GetChildren, Action<T> ThenDo)
        {
            ThenDo(root);

            IEnumerable<T> children = GetChildren(root);

            if (children.Count() == 0)
            {
                return;
            }

            foreach (T child in children)
            {
                DepthFirstSearch(child, GetChildren, ThenDo);
            }
        }

        public static int BreadthFirstSearch<T>(T root, Func<T, IEnumerable<T>> GetChildren, Action<T> ThenDo)
        {
            return BreadthFirstSearch(root, GetChildren, ThenDo, _ => { });
        }

        public static int BreadthFirstSearch<T>(T root, Func<T, IEnumerable<T>> GetChildren, 
                                                Action<T> ThenDo, Action<IEnumerable<T>> ThenDoAtEveryFrontier)
        {
            Queue<T> pollingQueue = new Queue<T>(new T[] { root });
            Queue<T> offerQueue = new Queue<T>();
            HashSet<T> visited = new HashSet<T>() { root };

            int frontiers = 0;

            while (pollingQueue.Count > 0)
            {
                ThenDoAtEveryFrontier(pollingQueue.ToArray());
                frontiers++;

                while (pollingQueue.Count > 0)
                {
                    T current = pollingQueue.Dequeue();

                    ThenDo(current);

                    foreach (T child in GetChildren(current))
                    {
                        if (visited.Contains(child))
                        {
                            continue;
                        }

                        visited.Add(child);
                        offerQueue.Enqueue(child);
                    }
                }
                Queue<T> temp = pollingQueue;
                pollingQueue = offerQueue;
                offerQueue = temp;
            }

            return frontiers;
        }

        public static IEnumerable<IEnumerable<T>> Kosaraju<T>(IDirectedGraph<T> directedGraph)
        {
            IDirectedGraph<T> transposedGraph = directedGraph.Transpose();

            HashSet<T> allNodes = new HashSet<T>(directedGraph.ToArray());

            Queue<T> queue = new Queue<T>();

            while (allNodes.Count > 0)
            {
                DepthFirstSearch(allNodes.First(),
                                x => directedGraph.GetConnected(x).Where(x => allNodes.Contains(x)),
                                x => { allNodes.Remove(x); queue.Enqueue(x); }
                                );
            }

            List<HashSet<T>> stronglyConnectedComponents = new List<HashSet<T>>();

            while (queue.Count > 0)
            {
                T current = queue.Dequeue();
                if (allNodes.Contains(current))
                {
                    continue;
                }

                HashSet<T> components = new HashSet<T>();
                DepthFirstSearch(current,
                                x => transposedGraph.GetConnected(x).Where(x => !allNodes.Contains(x)),
                                x => { allNodes.Add(x); components.Add(x); }
                            );
                stronglyConnectedComponents.Add(components);
            }

            return stronglyConnectedComponents;
        }
    }
}
