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

        public class TreeNode : GraphNode, IGraphableNode<T>
        {
            private TreeNode parent = null;
            public TreeNode Parent => parent;
            public TreeNode(T data) : base(data) { }

            public void SetParent(TreeNode newParent)
            {
                parent = newParent;
            }

            public void RemoveParent()
            {
                parent = null;
            }

            public T GetParent()
            {
                if (parent == null)
                {
                    return default(T);
                }
                return parent.Data;
            }


        }

        public Tree()
        {

        }

        public Tree(IEnumerable<T> items) : base(items)
        {

        }

        public T GetParent(T data)
        {
            TreeNode treeNode = (TreeNode) StoredItems[data];
            return treeNode.GetParent();

        }

        public override bool Connect(T a, T b)
        {
            if (!this.Contains(a) || !this.Contains(b))
            {
                return false;
            }

            //b must be an empty parent (default(T))
            if (!GetParent(b).Equals(default(T))) {
                return false; 
            }

    
            TreeNode graphNodeA = (TreeNode) StoredItems[a];
            TreeNode graphNodeB = (TreeNode) StoredItems[b];
            graphNodeB.SetParent(graphNodeA);
            return graphNodeA.Connect(graphNodeB);
        }

        public override bool Disconnect(T a, T b)
        {
            if (!this.Contains(a) || !this.Contains(b))
            {
                return false;
            }

            TreeNode graphNodeA = (TreeNode) StoredItems[a];
            TreeNode graphNodeB = (TreeNode) StoredItems[b];
            return graphNodeA.Disconnect(graphNodeB);

        }

        public override bool CanWalkFromTo(T a, T b)
        {
            bool destinationReached = false;
            SearchAlgorithms.DepthFirstSearch(a, x => GetConnected(x), x => { if (x.Equals(b)) destinationReached = true; });
            return destinationReached;
        }
        //Overriden methods for connect and disconnect 

        public override bool Remove(T a) 
        {
            if (!this.Contains(a))
            {
                return false;
            }
            //Update parents
            foreach (GraphNode graphNode in StoredItems.Values)
            {
                TreeNode treeNode = (TreeNode) graphNode;
                if (treeNode.GetParent().Equals(a)) 
                {
                    treeNode.RemoveParent();
                }
            }

            return base.Remove(a);
        }


    }
}
