using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataStructures;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace UndirectedGraphTests {
    public class UndirectedGraphContractEdge
    {
        //    |------------------|
        //    C --- A --- B --- D
        //             \
        //              E
        //
        //
        // 
        // After contracting AB with F

        //    |-------------------|
        //    C ------- F ------- D
        //              |
        //              E

        [Test]
        public void SingleContract()
        {
            IUndirectedGraph<string> graph = new UndirectedGraph<string>();

            string[] strings = { "a", "b", "c", "d", "e" };

            foreach (string s in strings)
            {
                graph.Add(s);
            }

            graph.Connect("a", "c");
            graph.Connect("b", "d");
            graph.Connect("a", "e");
            graph.Connect("a", "b");
            graph.Connect("d", "c");

            IUndirectedGraph<string> newGraph = graph.ContractEdgeBetween("a", "b", "f");
            //check for new node 
            Assert.IsTrue(newGraph.Contains("f"));
            //check existing connections 
            Assert.IsTrue(newGraph.IsConnected("c", "f"));
            Assert.IsTrue(newGraph.IsConnected("d", "f"));
            Assert.IsTrue(newGraph.IsConnected("f", "e"));
            Assert.IsTrue(newGraph.IsConnected("c", "d"));

            //check no more connectsions 
            Assert.IsFalse(newGraph.IsConnected("c", "a"));
            Assert.IsFalse(newGraph.IsConnected("b", "a"));
            Assert.IsFalse(newGraph.IsConnected("b", "d"));

            //Check for graph containing B?

            Assert.IsFalse(newGraph.Contains("b"));
            Assert.IsFalse(newGraph.Contains("a"));
            

        }


    }
}
