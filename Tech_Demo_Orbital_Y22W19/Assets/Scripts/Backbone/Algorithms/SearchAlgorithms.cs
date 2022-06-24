using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataStructures;

namespace Algorithms
{
    public static class SearchAlgorithms
    {
        public static void DepthFirstSearch<T>(T root, Func<T, IEnumerable<T>> GetChildren, Action<T> ThenDo)
        {
            DepthFirstSearch(root, GetChildren, ThenDo, _ => { });
        }

        public static void DepthFirstSearch<T>(T root, Func<T, IEnumerable<T>> GetChildren, Action<T> ThenDo, Action<T> Backtrack)
        {
            ICollection<T> visited = new HashSet<T>() { root };

            void DFS(T root)
            {
                ThenDo(root);

                IEnumerable<T> children = GetChildren(root);

                foreach (T child in children)
                {
                    if (visited.Contains(child))
                    {
                        continue;
                    }

                    visited.Add(child);
                    DFS(child);
                }

                Backtrack(root);
            }

            DFS(root);
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

            Stack<T> stack = new Stack<T>();

            while (allNodes.Count > 0)
            {
                DepthFirstSearch(allNodes.First(),
                                x => directedGraph.GetConnected(x).Where(x => allNodes.Contains(x)),
                                x => allNodes.Remove(x),
                                x => stack.Push(x)
                                );
            }
            Debug.Assert(stack.Count == directedGraph.Count(), $"Count was {stack.Count} when it should be {directedGraph.Count()}");

            List<HashSet<T>> stronglyConnectedComponents = new List<HashSet<T>>();

            while (stack.Count > 0)
            {
                T current = stack.Pop();
                if (allNodes.Contains(current))
                {
                    continue;
                }

                HashSet<T> components = new HashSet<T>();
                DepthFirstSearch(current,
                                x => transposedGraph.GetConnected(x).Where(x => stack.Contains(x)),
                                x => { allNodes.Add(x); components.Add(x); }
                            );
                stronglyConnectedComponents.Add(components);
            }

            return stronglyConnectedComponents;
        }
    }
}
