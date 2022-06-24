using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DecisionTree
{
    public class Node
    {
        private readonly int score;
        private readonly MapActionRequestOld action;

        public int Score => score;
        public MapActionRequestOld Action => action;

        public Node(int score, MapActionRequestOld action)
        {
            this.score = score;
            this.action = action;
        }

        public override string ToString()
        {
            return $"{action}, Rating: {score}";
        }
    }
}
