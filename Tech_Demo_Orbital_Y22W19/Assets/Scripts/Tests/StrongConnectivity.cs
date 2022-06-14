using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataStructures;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DirectedGraphTests {
    public class StrongConnectivity
    {
        [Test]
        public void GraphTranspose()
        {
            IDirectedGraph<string> graph = new DirectedGraph<string>();

            string[] strings = { "a", "b", "c", "d", "e" };

            foreach (string s in strings)
            {
                graph.Add(s);
            }

            //      |---------v
            // a -> b -> c -> d

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("b", "d");
            graph.Connect("c", "d");
            graph.Connect("d", "e");

            IDirectedGraph<string> newGraph = graph.Transpose();

            //      V---------|
            // a <- b <- c <- d

            Assert.IsTrue(newGraph.IsConnected("b", "a"));
            Assert.IsTrue(newGraph.IsConnected("c", "b"));
            Assert.IsTrue(newGraph.IsConnected("d", "c"));
            Assert.IsTrue(newGraph.IsConnected("d", "b"));

            Assert.IsFalse(newGraph.IsConnected("c", "d"));
            Assert.IsFalse(newGraph.IsConnected("a", "b"));
            Assert.IsFalse(newGraph.IsConnected("b", "c"));
            Assert.IsFalse(newGraph.IsConnected("b", "d"));
        }

        [Test]
        public void GraphTranspose2()
        {
            IDirectedGraph<string> graph = new DirectedGraph<string>();

            string[] strings = { "a", "b", "c", "d", "e", "f" };

            foreach (string s in strings)
            {
                graph.Add(s);
            }

            // a --> b      f
            // ^    /      / ^ 
            //  \  v      v   \
            //    c  --> d --> e

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "a");

            graph.Connect("c", "d");

            graph.Connect("d", "e");
            graph.Connect("e", "f");
            graph.Connect("f", "d");

            IDirectedGraph<string> newGraph = graph.Transpose();

            // a <-- b      f
            //  \   ^      ^ \
            //   v /      /   v
            //    c  <-- d <-- e

            Assert.IsTrue(newGraph.IsConnected("a", "c"));
            Assert.IsTrue(newGraph.IsConnected("b", "a"));
            Assert.IsTrue(newGraph.IsConnected("c", "b"));
            Assert.IsTrue(newGraph.IsConnected("d", "c"));
            Assert.IsTrue(newGraph.IsConnected("d", "f"));
            Assert.IsTrue(newGraph.IsConnected("f", "e"));

            Assert.IsFalse(newGraph.IsConnected("c", "d"));
            Assert.IsFalse(newGraph.IsConnected("d", "e"));
            Assert.IsFalse(newGraph.IsConnected("e", "f"));
            Assert.IsFalse(newGraph.IsConnected("f", "d"));

            Assert.IsFalse(newGraph.IsConnected("a", "b"));
            Assert.IsFalse(newGraph.IsConnected("b", "c"));
            Assert.IsFalse(newGraph.IsConnected("c", "a"));
        }

        [Test]
        public void GraphStronglyConnectedCount()
        {
            IDirectedGraph<string> graph = new DirectedGraph<string>();

            string[] strings = { "a", "b", "c", "d", "e", "f", "g" };

            foreach (string s in strings)
            {
                graph.Add(s);
            }

            // a --> b      f
            // ^    /      / ^ 
            //  \  v      v   \
            //    c  --> d --> e     g

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "a");

            graph.Connect("c", "d");

            graph.Connect("d", "e");
            graph.Connect("e", "f");
            graph.Connect("f", "d");

            Assert.AreEqual(3, graph.GetStronglyConnectedComponents().Count());
        }

        [Test]
        public void GraphStronglyConnectedCountTotallyDisconnected()
        {
            IDirectedGraph<string> graph = new DirectedGraph<string>();

            string[] strings = { "a", "b", "c", "d", "e", "f", "g" };

            foreach (string s in strings)
            {
                graph.Add(s);
            }

            Assert.AreEqual(7, graph.GetStronglyConnectedComponents().Count());
        }

        [Test]
        public void GraphStronglyConnectedCountChain()
        {
            IDirectedGraph<string> graph = new DirectedGraph<string>();

            string[] strings = { "a", "b", "c", "d", "e", "f", "g" };

            foreach (string s in strings)
            {
                graph.Add(s);
            }

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");
            graph.Connect("d", "e");
            graph.Connect("e", "f");
            graph.Connect("f", "g");

            Assert.AreEqual(7, graph.GetStronglyConnectedComponents().Count());
        }

        [Test]
        public void GraphStronglyConnectedCountCircle()
        {
            IDirectedGraph<string> graph = new DirectedGraph<string>();

            string[] strings = { "a", "b", "c", "d", "e", "f", "g" };

            foreach (string s in strings)
            {
                graph.Add(s);
            }

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");
            graph.Connect("d", "e");
            graph.Connect("e", "f");
            graph.Connect("f", "g");
            graph.Connect("g", "a");

            Assert.AreEqual(1, graph.GetStronglyConnectedComponents().Count());
        }
    }
}
