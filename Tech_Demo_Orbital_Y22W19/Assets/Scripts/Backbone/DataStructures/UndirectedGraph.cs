using Algorithms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;




namespace DataStructures
{
    public class UndirectedGraph<T> : Graph<T>, IUndirectedGraph<T>
    {

        public UndirectedGraph()
        {

        }

        public UndirectedGraph(IEnumerable<T> items) : base(items)
        {

        }






        public override bool Connect(T a, T b)
        {
            if (!this.Contains(a) || !this.Contains(b))
            {
                return false;
            }

            GraphNode graphNodeA = StoredItems[a];
            GraphNode graphNodeB = StoredItems[b];
            return graphNodeA.Connect(graphNodeB) && graphNodeB.Connect(graphNodeA);
        }

        public override bool Disconnect(T a, T b)
        {
            if (!this.Contains(a) || !this.Contains(b))
            {
                return false;
            }

            GraphNode graphNodeA = StoredItems[a];
            GraphNode graphNodeB = StoredItems[b];
            return graphNodeA.Disconnect(graphNodeB) && graphNodeB.Disconnect(graphNodeA);
        }


        /**
         * for this method to work
         * c must be a completely new item!
         */

        public IUndirectedGraph<T> ContractEdgeBetween(T a, T b, T c)
        {

            //if there is no edge btwn a/b --> return itself
            //if c is not new return itself 
            //if either a or b don't exist --> return itself

            if ( !IsConnected(a, b) || this.Contains(c) || !this.Contains(a) || !this.Contains(b))
            {
                return this; 
            }

            IUndirectedGraph<T> graph = new UndirectedGraph<T>(StoredItems.Keys);
            foreach (T item in StoredItems.Keys)
            {
                foreach (T item2 in StoredItems.Keys)
                {
                    if (this.IsConnected(item, item2))
                    {
                        graph.Connect(item, item2);
                    }
                }
            }
            graph.Add(c); //--> there shouldn't be any connections to C

            //O(V^2) cloning of graph
            //Connect all neighbours from both B and A  (no c)
            IEnumerable<T> connectedToB = GetConnected(b);
            IEnumerable<T> connectedToA = GetConnected(a);  


            foreach (T item in connectedToB)
            {
                graph.Connect(c, item);
            }

            foreach (T item in connectedToA)
            {
                graph.Connect(c, item);
            }

            //Then, disconnect all neighbours of B
            foreach(T item in connectedToB)
            {
                graph.Disconnect(b, item);
            }

            foreach(T item in connectedToA)
            {
                graph.Disconnect(a, item);
            }

            graph.Remove(a);
            graph.Remove(b);
            //Now, b has no outgoing edges!
            //originally edges from A go to B!
            return graph;
        }

        public override bool CanWalkFromTo(T a, T b)
        {
            bool destinationReached = false;
            SearchAlgorithms.DepthFirstSearch(a, x => GetConnected(x), x => { if (x.Equals(b)) destinationReached = true; });
            return destinationReached;
        }

    }

}