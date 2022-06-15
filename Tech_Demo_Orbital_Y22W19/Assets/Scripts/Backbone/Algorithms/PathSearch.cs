using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataStructures;
using System;
using Priority_Queue;

namespace Algorithms.ShortestPathSearch
{
    public static class PathSearch
    {
        public static ITree<T?> AStar<T> (T source, T? destination, IGraphable<T> graph,
            Func<T, T, int> EdgeWeightFromTo,
            Func<T, int> Heuristic,
            int maxCost = int.MaxValue
            ) where T : struct
        {
            IPriorityQueue<T, int> priorityQueue = new SimplePriorityQueue<T, int>();

            IDictionary<T, int> gScore = new Dictionary<T, int>();
            IDictionary<T, int> fScore = new Dictionary<T, int>();

            ITree<T?> shortestPathTree = new Tree<T?>();
            shortestPathTree.Add(source);

            gScore[source] = 0;
            fScore[source] = Heuristic(source);

            priorityQueue.Enqueue(source, fScore[source]);

            while (priorityQueue.Count > 0)
            {
                T current = priorityQueue.Dequeue();

                if ((destination != null && current.Equals(destination)) || gScore[current] > maxCost)
                {
                    return shortestPathTree;
                }

                foreach (T neighbour in graph.GetConnected(current))
                {
                    int tentativeGScore = gScore[current] + EdgeWeightFromTo(current, neighbour);


                    if (!gScore.ContainsKey(neighbour))
                    {
                        shortestPathTree.Add(neighbour);
                        shortestPathTree.Connect(current, neighbour);

                        gScore[neighbour] = tentativeGScore;
                        fScore[neighbour] = tentativeGScore + Heuristic(neighbour);

                        priorityQueue.Enqueue(neighbour, fScore[neighbour]);
                    }
                    else if (tentativeGScore < gScore[neighbour])
                    {
                        shortestPathTree.Reparent(neighbour, current);

                        gScore[neighbour] = tentativeGScore;
                        fScore[neighbour] = tentativeGScore + Heuristic(neighbour);

                        priorityQueue.UpdatePriority(neighbour, fScore[neighbour]);
                    }
                }
            }

            return shortestPathTree;
        }

        public static ITree<T?> Dijkstra<T>(T source, IGraphable<T> graph, 
            Func<T, T, int> EdgeWeight, Predicate<T> Terminate) where T : struct
        {
            Dictionary<T, int> distance = new Dictionary<T, int>() { { source, 0 } };
            ITree<T?> shortestPathTree = new Tree<T?>();
            shortestPathTree.Add(source);

            IPriorityQueue<T, int> priorityQueue = new SimplePriorityQueue<T, int>();
            priorityQueue.Enqueue(source, distance[source]);

            while (priorityQueue.Count > 0)
            {
                T current = priorityQueue.Dequeue();

                if (Terminate(current))
                {
                    break;
                }

                foreach (T neighbour in graph.GetConnected(current))
                {
                    int alternative = distance[current] + EdgeWeight(current, neighbour);

                    if (!distance.ContainsKey(neighbour))
                    {
                        distance[neighbour] = alternative;
                        priorityQueue.Enqueue(neighbour, distance[neighbour]);

                        shortestPathTree.Add(neighbour);
                        shortestPathTree.Connect(current, neighbour);
                    }
                    else if (alternative < distance[neighbour])
                    {
                        distance[neighbour] = alternative;

                        priorityQueue.UpdatePriority(neighbour, distance[neighbour]);

                        shortestPathTree.Reparent(neighbour, current);
                    }
                }
            }

            return shortestPathTree;
        }
    }
}
