using Algorithms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataStructures
{
    public class Tree<T> : Graph<T>, ITree<T>
    {

        public Tree()
        {
            //In this tree, I actually don't need a TreeNode 
            // I can just use getConnected (outgoing nodes) to return me an
            // Ienumerable!
        }

        public Tree(IEnumerable<T> items) : base(items)
        {

        }

        public bool HasExactlyOneParent(T data)
        {
            if (data == null || !this.Contains(data))
            {
                return false;
            }
            int numConnected = 0;
            foreach (GraphNode parent in StoredItems[data].GetConnected())
            {
                numConnected++;
            }
            if (numConnected == 1)
            {
                return true;
            }
            return false;


        }

        public bool HasExactlyNoParent(T data)
        {
            if (data == null || !this.Contains(data))
            {
                return false;
            }
            int numConnected = 0;
            foreach (GraphNode parent in StoredItems[data].GetConnected())
            {
                numConnected++;
            }
            if (numConnected == 0)
            {
                return true;
            }
            return false;


        }

        /**
         * GetParent should only have at most one child
         * There are a bunch of defaults I don't like 
         */

        public T GetParent(T data)
        {
            if (!this.HasExactlyOneParent(data))
            {
                return default(T);
            }
            GraphNode dataNode = StoredItems[data];
            foreach (GraphNode SingleParent in dataNode.GetConnected())
            {
                return SingleParent.Data;
            }
            return default(T);
        }

        public override bool Connect(T a, T b)
        {
            if (!this.Contains(a) || !this.Contains(b))
            {
                return false;
            }
            //b must have exactly 0 parents
            // must a have a parent?

            if (!this.HasExactlyNoParent(b))
            {
                return false;
            }

            GraphNode graphNodeA = StoredItems[a];
            GraphNode graphNodeB = StoredItems[b];
            return graphNodeA.Connect(graphNodeB);
        }

        public override bool Disconnect(T a, T b)
        {
            if (!this.Contains(a) || !this.Contains(b))
            {
                return false;
            }

            GraphNode graphNodeA = StoredItems[a];
            GraphNode graphNodeB = StoredItems[b];
            return graphNodeA.Disconnect(graphNodeB);

        }

        public override bool CanWalkFromTo(T a, T b)
        {
            bool destinationReached = false;
            SearchAlgorithms.DepthFirstSearch(a, x => GetConnected(x), x => { if (x.Equals(b)) destinationReached = true; });
            return destinationReached;
        }


    }
}
