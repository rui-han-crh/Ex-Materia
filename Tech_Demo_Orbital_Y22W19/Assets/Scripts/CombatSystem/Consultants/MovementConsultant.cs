using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Algorithms.ShortestPathSearch;
using DataStructures;
using CombatSystem.Entities;
using Algorithms;
using System.Linq;

namespace CombatSystem.Consultants
{
    public class MovementConsultant
    {
        private static readonly IEnumerable<Vector3Int> EMPTY = new Vector3Int[0];

        public static IEnumerable<Vector3Int> FindShortestPath(Vector3Int source, Vector3Int destination, GameMapData gameMapData)
        {
            return FindShortestPath(source, destination, gameMapData, new Vector3Int[] { source, destination });
        }

        public static IEnumerable<Vector3Int> FindShortestPath(Vector3Int source, Vector3Int destination, GameMapData gameMapData,
            IEnumerable<Vector3Int> inclusionZone)
        {
            IUndirectedGraph<Vector3Int> graph = gameMapData.ToUndirectedGraph(inclusionZone);

            ITree<Vector3Int?> shortestPathTree = PathSearch.AStar(
                source,
                destination,
                graph,
                (current, neighbour) => (int)(Vector3.Distance(current, neighbour) * 10)
                    + gameMapData.GetTileCost(neighbour),
                x => (int)(Vector3.Distance(x, destination) * 10)
                );

            Stack<Vector3Int> stack = new Stack<Vector3Int>();

            Vector3Int? current = destination;
            
            do
            {
                stack.Push(current.Value);
                current = shortestPathTree.GetParent(current);
            } while (current.HasValue);

            return stack;

        }

        public static IEnumerable<Vector3Int> FindShortestPath(Vector3Int source, Vector3Int destination, GameMapData gameMapData,
            IUndirectedGraph<Vector3Int> graph)
        {

            ITree<Vector3Int?> shortestPathTree = PathSearch.AStar(
                source,
                destination,
                graph,
                (current, neighbour) => (int)(Vector3.Distance(current, neighbour) * 10)
                    + gameMapData.GetTileCost(neighbour),
                x => (int)(Vector3.Distance(x, destination) * 10)
                );

            Stack<Vector3Int> stack = new Stack<Vector3Int>();

            if (!shortestPathTree.Contains(destination))
            {
                return new Vector3Int[0];
            }

            Vector3Int? current = destination;

            do
            {
                stack.Push(current.Value);
                current = shortestPathTree.GetParent(current);
            } while (current.HasValue);

            return stack;

        }

        public static IEnumerable<MovementRequest> GetAllMovements(GameMapData gameMapData, Unit currentActingUnit)
        {
            IUndirectedGraph<Vector3Int> graph = gameMapData.ToUndirectedGraph(
                new HashSet<Vector3Int>() { gameMapData[currentActingUnit] });

            Debug.Log($"Position of acting unit {gameMapData[currentActingUnit]}");
            Debug.Log($"Contains (-8, -20) ? {graph.Contains(new Vector3Int(-8, -20, 0))}");

            ITree<Vector3Int?> shortestPathTree = PathSearch.AStar(
                gameMapData[currentActingUnit],
                null,
                graph,
                (current, neighbour) => (int)(Vector3.Distance(current, neighbour) * 10)
                    + gameMapData.GetTileCost(neighbour),
                x => 0,
                currentActingUnit.CurrentActionPoints
                );

            int EdgeWeightFromParentTo(Vector3Int? x)
            {
                return (int)(Vector3.Distance(shortestPathTree.GetParent(x).GetValueOrDefault(x.Value), x.Value) * 10)
                    + gameMapData.GetTileCost(x.Value);
            }

            int currentActionPointCost = 0;
            List<MovementRequest> movementRequests = new List<MovementRequest>();

            SearchAlgorithms.DepthFirstSearch(
                shortestPathTree.Root,
                x => shortestPathTree.GetConnected(x),
                x => 
                    {
                        if (x.Equals(shortestPathTree.Root))
                        {
                            return;
                        }

                        currentActionPointCost += EdgeWeightFromParentTo(x);
                        movementRequests.Add(
                                new MovementRequest(
                                    currentActingUnit,
                                    x.Value,
                                    currentActionPointCost,
                                    currentActionPointCost,
                                    MovementRequest.Outcome.Successful)
                                );
                    },
                x => currentActionPointCost -= EdgeWeightFromParentTo(x)
                );

            return movementRequests;
        }

        public static HashSet<Vector3Int> GetAllMovementPositions(GameMapData gameMapData, Unit currentActingUnit)
        {
            IUndirectedGraph<Vector3Int> graph = gameMapData.ToUndirectedGraph(
                new HashSet<Vector3Int>() { gameMapData[currentActingUnit] });

            ITree<Vector3Int?> shortestPathTree = PathSearch.AStar(
                gameMapData[currentActingUnit],
                null,
                graph,
                (current, neighbour) => (int)(Vector3.Distance(current, neighbour) * 10)
                    + gameMapData.GetTileCost(neighbour),
                x => 0,
                currentActingUnit.CurrentActionPoints
                );

            return new HashSet<Vector3Int>(shortestPathTree.Where(v => v != null).Select(v => v.Value));
        }

        public static int GetPathCost(IEnumerable<Vector3Int> path, GameMapData data)
        {
            int cost = 0;
            Vector3Int? cameFrom = null;
            foreach (Vector3Int location in path)
            {
                if (cameFrom == null)
                {
                    cameFrom = location;
                    continue;
                }

                cost += data.GetTileCost(location) + (int)(Vector3Int.Distance(cameFrom.Value, location) * 10);
                cameFrom = location;
            }
            return cost;
        }
    }
}
