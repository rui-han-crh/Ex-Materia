using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataStructures;
using Algorithms;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace AlgorithmsTest
{
    public class BFSOnDirectedGraphs
    {
        [Test]
        public void BFSTest()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g", "h", "i" };

            IDirectedGraph<string> directedGraph = new DirectedGraph<string>(strings);

            //                  a
            //           b           c
            //        d     e     f     g
            //                      h
            //                     i 

            directedGraph.Connect("a", "b");
            directedGraph.Connect("b", "d");
            directedGraph.Connect("b", "e");
            directedGraph.Connect("a", "c");
            directedGraph.Connect("c", "f");
            directedGraph.Connect("f", "h");
            directedGraph.Connect("h", "i");
            directedGraph.Connect("c", "g");

            HashSet<string> visited = new HashSet<string>();
            string output = "";

            int frontiers = SearchAlgorithms.BreadthFirstSearch("a", x => directedGraph.GetConnected(x),
                                                x => visited.Add(x), seq => output += string.Join(", ", seq) + " | ");

            Assert.AreEqual("a | b, c | d, e, f, g | h | i | ", output);
            Assert.AreEqual(5, frontiers);
            CollectionAssert.AreEquivalent(visited, strings);

        }

        [Test]
        public void BFSTestShallow()
        {
            string[] strings = { "a", "b", "c", "d" };

            IDirectedGraph<string> directedGraph = new DirectedGraph<string>(strings);

            directedGraph.Connect("a", "b");
            directedGraph.Connect("a", "c");
            directedGraph.Connect("a", "d");

            string output = "";

            int frontiers = SearchAlgorithms.BreadthFirstSearch("a", x => directedGraph.GetConnected(x),
                                                _ => { }, seq => output += string.Join(", ", seq) + " | ");

            Assert.AreEqual("a | b, c, d | ", output);
            Assert.AreEqual(2, frontiers);
        }

        [Test]
        public void BFSTestDeep()
        {
            string[] strings = { "a", "b", "c", "d" };

            IDirectedGraph<string> directedGraph = new DirectedGraph<string>(strings);

            directedGraph.Connect("a", "b");
            directedGraph.Connect("b", "c");
            directedGraph.Connect("c", "d");

            string output = "";

            int frontiers = SearchAlgorithms.BreadthFirstSearch("a", x => directedGraph.GetConnected(x),
                                                _ => { }, seq => output += string.Join(", ", seq) + " | ");

            Assert.AreEqual("a | b | c | d | ", output);
            Assert.AreEqual(4, frontiers);
        }

        [Test]
        public void BFSTestCycle()
        {
            string[] strings = { "a", "b", "c", "d" };

            IDirectedGraph<string> directedGraph = new DirectedGraph<string>(strings);

            directedGraph.Connect("a", "b");
            directedGraph.Connect("b", "c");
            directedGraph.Connect("c", "d");
            directedGraph.Connect("d", "a");

            string output = "";

            int frontiers = SearchAlgorithms.BreadthFirstSearch("a", x => directedGraph.GetConnected(x),
                                                _ => { }, seq => output += string.Join(", ", seq) + " | ");

            Assert.AreEqual("a | b | c | d | ", output);
            Assert.AreEqual(4, frontiers);
        }
    }
}