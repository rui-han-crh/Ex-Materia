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

        public class TreeNode : GraphNode
        {

            public TreeNode Parent => IncomingNodes.Count == 0 ? null : (TreeNode)IncomingNodes.First();

            public TreeNode(T data) : base(data) 
            { 

            }
        }

        public Tree()
        {

        }

        public Tree(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }

        public T GetParent(T data)
        {
            if (!StoredItems.ContainsKey(data)) 
            {
                throw new KeyNotFoundException($"No mapping for {data} found in the tree");
            }

            TreeNode parent = ((TreeNode)StoredItems[data]).Parent;

            if (parent == null)
            {
                return default(T);
            }

            return parent.Data;
        }

        TreeNode currentRoot;

        public T Root => currentRoot.Data;

        public override void Add(T item)
        {
            TreeNode node = new TreeNode(item);

            currentRoot ??= node;

            StoredItems.Add(item, node);
        }

        public override bool Connect(T a, T b)
        {
            if (!this.Contains(a) || !this.Contains(b))
            {
                return false;
            }

            TreeNode graphNodeB = (TreeNode)StoredItems[b];

            if (graphNodeB.Parent != null) 
            {
                return false; 
            }

            TreeNode graphNodeA = (TreeNode) StoredItems[a];

            if (graphNodeB.Equals(currentRoot))
            {
                currentRoot = graphNodeA;
            }

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

        public void Reparent(T existingChild, T newParent)
        {
            TreeNode child = (TreeNode)StoredItems[existingChild];
            TreeNode oldParent = child.Parent;

            if (oldParent == null)
            {
                throw new KeyNotFoundException($"{child} has no parent");
            }

            Disconnect(oldParent.Data, existingChild);

            if (!StoredItems.ContainsKey(newParent))
            {
                Add(newParent);
            }

            GraphNode newParentNode = StoredItems[newParent];
            newParentNode.Connect(child);
        }

        public override bool Remove(T a)
        {
            if (!this.Contains(a))
            {
                return false;
            }

            TreeNode node = (TreeNode)StoredItems[a];

            if (node.Equals(currentRoot) && node.GetConnected().Count() > 0)
            {
                currentRoot = (TreeNode)node.GetConnected().First();
            }

            return base.Remove(a);
        }
    }
}
