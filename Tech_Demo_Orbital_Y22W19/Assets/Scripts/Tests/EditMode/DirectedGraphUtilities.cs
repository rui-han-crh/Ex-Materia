using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataStructures;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DirectedGraphTests
{
    public class DirectedGraphUtilities
    {
        [Test]
        public void GraphDiameterLine()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g" };
            IGraph<string> graph = new DirectedGraph<string>(strings);

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");
            graph.Connect("d", "e");
            graph.Connect("e", "f");
            graph.Connect("f", "g");

            Assert.AreEqual(6, graph.GetDiameter());
        }

        [Test]
        public void GraphDiameterCircle()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g" };
            IGraph<string> graph = new DirectedGraph<string>(strings);

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");
            graph.Connect("d", "e");
            graph.Connect("e", "f");
            graph.Connect("f", "g");
            graph.Connect("g", "a");

            Assert.AreEqual(6, graph.GetDiameter());
        }

        [Test]
        public void GraphDiameterCircleUndirected()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g" };
            IGraph<string> graph = new DirectedGraph<string>(strings);

            graph.Connect("a", "b");
            graph.Connect("b", "a");

            graph.Connect("b", "c");
            graph.Connect("c", "d");

            graph.Connect("c", "d");
            graph.Connect("d", "c");

            graph.Connect("d", "e");
            graph.Connect("e", "d");

            graph.Connect("e", "f");
            graph.Connect("f", "e");

            graph.Connect("f", "g");
            graph.Connect("g", "f");

            graph.Connect("g", "a");
            graph.Connect("a", "g");

            Assert.AreEqual(3, graph.GetDiameter());
        }

        [Test]
        public void GraphCycleSimple()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g" };
            IGraph<string> graph = new DirectedGraph<string>(strings);

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");
            graph.Connect("d", "e");
            graph.Connect("e", "f");
            graph.Connect("f", "g");
            graph.Connect("g", "a");

            Assert.IsTrue(graph.HasCycle());
        }

        [Test]
        public void GraphCycleFalse()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g" };
            IGraph<string> graph = new DirectedGraph<string>(strings);

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");
            graph.Connect("d", "e");
            graph.Connect("e", "f");
            graph.Connect("f", "g");

            Assert.IsFalse(graph.HasCycle());
        }

        [Test]
        public void GraphCycleDouble()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g" };
            IGraph<string> graph = new DirectedGraph<string>(strings);

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");
            graph.Connect("d", "a");
            graph.Connect("d", "e");
            graph.Connect("e", "f");
            graph.Connect("f", "g");
            graph.Connect("g", "d");

            Assert.IsTrue(graph.HasCycle());
        }

        [Test]
        public void GraphCycleNineShape()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g" };
            IGraph<string> graph = new DirectedGraph<string>(strings);

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");
            graph.Connect("d", "e");
            graph.Connect("e", "f");
            graph.Connect("f", "g");
            graph.Connect("g", "d");

            Assert.IsTrue(graph.HasCycle());
        }

        [Test]
        public void GraphCycleDisconnected()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g" };
            IGraph<string> graph = new DirectedGraph<string>(strings);

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");

            graph.Connect("e", "f");
            graph.Connect("f", "g");
            graph.Connect("g", "e");

            Assert.IsTrue(graph.HasCycle());
        }

        [Test]
        public void GraphCycleDisconnectedMany()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k" };
            IGraph<string> graph = new DirectedGraph<string>(strings);

            graph.Connect("j", "d");
            graph.Connect("d", "j");

            Assert.IsTrue(graph.HasCycle());
        }

        [Test]
        public void GraphCycleTotallyDisconnected()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k" };
            IGraph<string> graph = new DirectedGraph<string>(strings);


            Assert.IsFalse(graph.HasCycle());
        }

        [Test]
        public void GraphStarDegree()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k" };
            IGraph<string> graph = new DirectedGraph<string>(strings);

            graph.Connect("a", "b");
            graph.Connect("a", "c");
            graph.Connect("a", "d");
            graph.Connect("a", "e");
            graph.Connect("a", "f");
            graph.Connect("a", "g");

            graph.Connect("h", "i");
            graph.Connect("j", "k");
            graph.Connect("k", "l");
            graph.Connect("l", "j");

            Assert.AreEqual(6, graph.GetMaximumDegree());
        }

        [Test]
        public void GraphLineDegree()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k" };
            IGraph<string> graph = new DirectedGraph<string>(strings);

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");
            graph.Connect("d", "e");
            graph.Connect("e", "f");
            graph.Connect("f", "g");
            graph.Connect("g", "h");
            graph.Connect("h", "i");
            graph.Connect("i", "j");
            graph.Connect("j", "k");



            Assert.AreEqual(1, graph.GetMaximumDegree());
        }

        [Test]
        public void GraphConnectivityWeak()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k" };
            IDirectedGraph<string> graph = new DirectedGraph<string>(strings);

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");
            graph.Connect("d", "e");
            graph.Connect("e", "f");
            graph.Connect("f", "g");
            graph.Connect("g", "h");
            graph.Connect("h", "i");
            graph.Connect("i", "j");
            graph.Connect("j", "k");



            Assert.IsTrue(graph.IsConnected());
            Assert.IsFalse(graph.IsStronglyConnected());
        }

        [Test]
        public void GraphConnectivityDisconnected()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k" };
            IDirectedGraph<string> graph = new DirectedGraph<string>(strings);

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");
            graph.Connect("d", "e");

            graph.Connect("f", "g");
            graph.Connect("g", "h");
            graph.Connect("h", "i");
            graph.Connect("i", "j");
            graph.Connect("j", "k");



            Assert.IsFalse(graph.IsConnected());
            Assert.IsFalse(graph.IsStronglyConnected());
            Assert.AreEqual(2, graph.GetWeaklyConnectedComponents().Count());

            List<HashSet<string>> weakComponents = new List<HashSet<string>>();
            weakComponents.Add(new HashSet<string>() { "a", "b", "c", "d", "e" });
            weakComponents.Add(new HashSet<string>() { "f", "g", "h", "i", "j", "k" });
            CollectionAssert.AreEquivalent(weakComponents, graph.GetWeaklyConnectedComponents());
        }

        [Test]
        public void GraphConnectivityStrong()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k" };
            IDirectedGraph<string> graph = new DirectedGraph<string>(strings);

            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");
            graph.Connect("d", "e");
            graph.Connect("e", "f");
            graph.Connect("f", "g");
            graph.Connect("g", "h");
            graph.Connect("h", "i");
            graph.Connect("i", "j");
            graph.Connect("j", "k");
            graph.Connect("k", "a");

            Assert.IsTrue(graph.IsStronglyConnected());
        }
    }
}
