using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DecisionTree
{
    public class Node
    {
        private readonly int score;
        private readonly MapActionRequest action;

        public int Score => score;
        public MapActionRequest Action => action;

        public Node(int score, MapActionRequest action)
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
