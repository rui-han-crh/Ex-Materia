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
    public class DFSOnDirectedGraphs
    {
        [Test]
        public void DFSTest()
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

            string output = "";

            SearchAlgorithms.DepthFirstSearch("a", x => directedGraph.GetConnected(x),
                                                x => output += x + ", " );

            Assert.AreEqual("a, b, d, e, c, f, h, i, g, ", output);

        }


        [Test]
        public void DFSTestCycle()
        {
            string[] strings = { "a", "b", "c", "d" };

            IDirectedGraph<string> directedGraph = new DirectedGraph<string>(strings);

            directedGraph.Connect("a", "b");
            directedGraph.Connect("b", "c");
            directedGraph.Connect("c", "d");
            directedGraph.Connect("d", "a");

            string output = "";

            SearchAlgorithms.DepthFirstSearch("a", x => directedGraph.GetConnected(x),
                                                x => output += x + ", ");

            Assert.AreEqual("a, b, c, d, ", output);
        }

        [Test]
        public void DFSTestDoubleCycle()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g", "h" };

            IDirectedGraph<string> directedGraph = new DirectedGraph<string>(strings);

            //     b --> c  h <- g <-- f
            //     ^     |  |          ^
            //     |     v  v          |
            //     a <---- d --------> e
            //

            directedGraph.Connect("a", "b");
            directedGraph.Connect("b", "c");
            directedGraph.Connect("c", "d");
            directedGraph.Connect("d", "a");
            directedGraph.Connect("d", "e");
            directedGraph.Connect("e", "f");
            directedGraph.Connect("f", "g");
            directedGraph.Connect("g", "h");
            directedGraph.Connect("h", "i");
            directedGraph.Connect("i", "d");

            string output = "";

            SearchAlgorithms.DepthFirstSearch("a", x => directedGraph.GetConnected(x),
                                                x => output += x + ", ");

            Assert.AreEqual("a, b, c, d, e, f, g, h, ", output);
        }

        [Test]
        public void DFSTestNonreachable()
        {
            string[] strings = { "a", "b", "c" };

            IDirectedGraph<string> directedGraph = new DirectedGraph<string>(strings);

            // a -> b <- c

            directedGraph.Connect("a", "b");
            directedGraph.Connect("c", "b");

            string output = "";

            SearchAlgorithms.DepthFirstSearch("a", x => directedGraph.GetConnected(x),
                                                x => output += x + ", ");

            Assert.AreEqual("a, b, ", output);

            output = "";

            SearchAlgorithms.DepthFirstSearch("c", x => directedGraph.GetConnected(x),
                                                x => output += x + ", ");

            Assert.AreEqual("c, b, ", output);

        }
    }
}