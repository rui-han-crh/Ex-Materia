using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DecisionTree
{
    public class Minimax
    {
        public static Node PerformSearch(GameMapOld map, int depth, int alpha, int beta)
        {
            if (depth == 0 || map.IsGameOver())
            {
                MapActionRequestOld lastAction = map.LastAction;
                return new Node(map.Evaluate(), lastAction);
            }

            bool isMaximisingPlayer = map.CurrentUnit.Faction == Faction.Friendly;

            MapActionRequestOld actionChange = map.LastAction;
            int extremeValue = isMaximisingPlayer ? int.MinValue : int.MaxValue;
            Node bestNode = new Node(extremeValue, actionChange);

            foreach (MapActionRequestOld childAction in map.GetOrderedMapActions())
            {
                GameMapOld resultantMap = childAction.GetNextMap();

                Node childNode = PerformSearch(resultantMap, depth - 1, alpha, beta);

                if (depth == 5)
                {
                    Debug.Log("This move is " + childAction + " Rating: " + childNode.Score);
                }
                if ((isMaximisingPlayer && (childNode.Score > bestNode.Score || bestNode.Score == extremeValue))
                    || (!isMaximisingPlayer && (childNode.Score < bestNode.Score || bestNode.Score == extremeValue))
                    )
                {
                    bestNode = new Node(childNode.Score, childAction);

                    if (isMaximisingPlayer)
                    {
                        alpha = Mathf.Max(alpha, childNode.Score);
                    }
                    else
                    {
                        beta = Mathf.Min(beta, childNode.Score);
                    }
                }

                if (beta <= alpha)
                {
                    break;
                }
            }
            return bestNode;
        }
    }
}
