using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataStructures;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace UndirectedGraphTests
{
    public class UndirectedGraphInitialisations
    {
        [Test]
        public void GraphAdd()
        {
            ICollection<Vector3Int> graph = new UndirectedGraph<Vector3Int>();

            graph.Add(new Vector3Int(2, 4, 5));
            graph.Add(new Vector3Int(3, 3, 3));
            graph.Add(new Vector3Int(4, 4, 5));

            Assert.IsFalse(graph.Contains(Vector3Int.one));
            Assert.IsTrue(graph.Contains(new Vector3Int(3, 3, 3)));
            Assert.IsTrue(graph.Contains(new Vector3Int(2, 4, 5)));
            Assert.IsTrue(graph.Contains(new Vector3Int(4, 4, 5)));
        }

        [Test]
        public void GraphRemove()
        {
            ICollection<Vector3Int> graph = new UndirectedGraph<Vector3Int>();

            graph.Add(new Vector3Int(3, 3, 3));
            graph.Add(new Vector3Int(4, 4, 5));
            graph.Remove(new Vector3Int(3, 3, 3));

            Assert.IsFalse(graph.Contains(Vector3Int.one));
            Assert.IsFalse(graph.Contains(new Vector3Int(3, 3, 3)));
            Assert.IsTrue(graph.Contains(new Vector3Int(4, 4, 5)));
        }

        [Test]
        public void GraphConnect()
        {
            ICollection<int> graph = new UndirectedGraph<int>();

            int a = 3;
            int b = 5;

            graph.Add(a);
            graph.Add(b);

            // Did A successfully connect with B?
            Assert.IsTrue(((IConnector<int>)graph).Connect(a, b));

            // Is A now connected with B?
            Assert.IsTrue(((IConnector<int>)graph).IsConnected(a, b));

            // In a doubly connected Graph, A should be connected with B
            Assert.IsTrue(((IConnector<int>)graph).IsConnected(b, a));

            // B should not be able to connect to A anymore
            Assert.IsFalse(((IConnector<int>)graph).Connect(b, a));
        }

        [Test]
        public void GraphConnectAndRemove()
        {
            ICollection<int> graph = new UndirectedGraph<int>();

            int a = 3;
            int b = 5;

            graph.Add(a);
            graph.Add(b);

            // Did A successfully connect with B?
            Assert.IsTrue(((IConnector<int>)graph).Connect(a, b));

            // Is A now connected with B? And otherwise
            Assert.IsTrue(((IConnector<int>)graph).IsConnected(a, b));
            Assert.IsTrue(((IConnector<int>)graph).IsConnected(b, a));

            // Can B be successfully connected with A? --> No!
            Assert.IsFalse(((IConnector<int>)graph).Connect(b, a));

            // Is B now successfully connected with A?
            Assert.IsTrue(((IConnector<int>)graph).IsConnected(b, a));

            // Remove A
            graph.Remove(a);


            //Does A still exist in the graph?
            Assert.IsFalse(graph.Contains(a));

            // Does B still exist in the graph?
            Assert.IsTrue(graph.Contains(b));

            // Is B still connected with A?
            // B shouldn't be connected to A
            Assert.IsFalse(((IConnector<int>)graph).IsConnected(b, a));
            Assert.IsFalse(((IConnector<int>)graph).IsConnected(a, b));
        }



        [Test]
        public void GraphConnectAndDisconnect()
        {
            ICollection<string> graph = new UndirectedGraph<string>();

            string alpha = "ALPHA";
            string beta = "BETA";

            graph.Add(alpha);
            graph.Add(beta);

            //I can connect alpha and beta
            Assert.IsTrue(((IConnector<string>)graph).Connect(alpha, beta));

            //I can disconnect beta and alpha 
            Assert.IsTrue(((IConnector<string>)graph).Disconnect(beta, alpha));

            //But then, I shouldn't be able to disconnect alpha and beta
            //their double bond is broken 
            Assert.IsFalse(((IConnector<string>)graph).Disconnect(alpha, beta));


            //Ensure everything else is correct
            Assert.IsTrue(graph.Contains(beta));
            Assert.IsTrue(graph.Contains(alpha));
        }


        //No need to test for dual directions?


        //[Test]
        //public void GraphConnectAndDisconnectDualDirections()
        //{
        //    ICollection<string> graph = new DirectedGraph<string>();

        //    string alpha = "ALPHA";
        //    string beta = "BETA";

        //    graph.Add(alpha);
        //    graph.Add(beta);

        //    Assert.IsTrue(((IConnector<string>)graph).Connect(alpha, beta));
        //    Assert.IsTrue(((IConnector<string>)graph).Connect(beta, alpha));

        //     A <-> B
        //    Assert.IsTrue(((IConnector<string>)graph).IsConnected(beta, alpha));
        //    Assert.IsTrue(((IConnector<string>)graph).IsConnected(alpha, beta));

        //    Assert.IsTrue(((IConnector<string>)graph).Disconnect(alpha, beta));
        //     A <- B

        //    Assert.IsFalse(((IConnector<string>)graph).IsConnected(alpha, beta));
        //    Assert.IsTrue(((IConnector<string>)graph).IsConnected(beta, alpha));

        //}

        [Test]
        public void GraphConnectAndDisconnect2()
        {
            ICollection<string> graph = new UndirectedGraph<string>();

            string alpha = "ALPHA";
            string beta = "BETA";
            string delta = "DELTA";

            graph.Add(alpha);
            graph.Add(beta);
            graph.Add(delta);

            // A -> B
            Assert.IsTrue(((IConnector<string>)graph).Connect(alpha, beta));
            // A -> B -> D
            Assert.IsTrue(((IConnector<string>)graph).Connect(beta, delta));

            // A    B -> D
            Assert.IsTrue(((IConnector<string>)graph).Disconnect(alpha, beta));
            Assert.IsTrue(((IConnector<string>)graph).IsConnected(beta, delta));

        }

        [Test]
        public void GraphConnectAndDisconnect3()
        {
            ICollection<string> graph = new DirectedGraph<string>();

            string alpha = "ALPHA";
            string beta = "BETA";
            string delta = "DELTA";

            graph.Add(alpha);
            graph.Add(beta);
            graph.Add(delta);

            // A -> B
            Assert.IsTrue(((IConnector<string>)graph).Connect(alpha, beta));
            // A -> B -> D
            Assert.IsTrue(((IConnector<string>)graph).Connect(beta, delta));
            // A -> B -> D -> A
            Assert.IsTrue(((IConnector<string>)graph).Connect(delta, alpha));

            // A    B -> D -> A
            Assert.IsTrue(((IConnector<string>)graph).Disconnect(alpha, beta));
            Assert.IsTrue(((IConnector<string>)graph).IsConnected(beta, delta));
            Assert.IsTrue(((IConnector<string>)graph).IsConnected(delta, alpha));
            //Additional Test to see if disconnection works both ways 
            Assert.IsFalse(((IConnector<string>)graph).IsConnected(beta, alpha));
        }

        //Checkpoint

        [Test] 
        public void GraphConnectMultipleChildren()
        {
            ICollection<string> graph = new UndirectedGraph<string>();

            string alpha = "ALPHA";
            string beta = "BETA";
            string delta = "DELTA";
            string gamma = "GAMMA";

            graph.Add(alpha);
            graph.Add(beta);
            graph.Add(delta);
            graph.Add(gamma);

            // A -> B
            Assert.IsTrue(((IConnector<string>)graph).Connect(alpha, beta));
            // A -> B && A --> D 
            Assert.IsTrue(((IConnector<string>)graph).Connect(alpha, delta));
            //A -> B && A --> D %% A --> G
            Assert.IsTrue(((IConnector<string>)graph).Connect(alpha, gamma));


            //Check if all are connected
            IConnector<string> connector = (IConnector<string>)graph;
            Assert.IsTrue(connector.IsConnected(alpha, gamma));
            Assert.IsTrue(connector.IsConnected(beta, alpha));
            Assert.IsTrue(connector.IsConnected(alpha, delta));

            string[] strings = new string[] { "BETA", "DELTA", "GAMMA" };

            IEnumerable<string> children = connector.GetConnected(alpha);

            foreach (string s in strings)
            {
                CollectionAssert.Contains(children, s);
            }

            graph.Remove(alpha);
            Assert.IsFalse(connector.IsConnected(gamma, alpha));
            //no funny connections between children 
            Assert.IsFalse(connector.IsConnected(beta, delta));
        }

        [Test]
        public void GraphPairwiseTypeTest()
        {
            ICollection<KeyValuePair<Vector2Int, int>> graph = new DirectedGraph<KeyValuePair<Vector2Int, int>>();

            KeyValuePair<Vector2Int, int> a = new KeyValuePair<Vector2Int, int>(new Vector2Int(0, 0), 0);
            KeyValuePair<Vector2Int, int> b = new KeyValuePair<Vector2Int, int>(new Vector2Int(0, 1), 1);
            KeyValuePair<Vector2Int, int> c = new KeyValuePair<Vector2Int, int>(new Vector2Int(0, 2), 2);
            KeyValuePair<Vector2Int, int> d = new KeyValuePair<Vector2Int, int>(new Vector2Int(1, 0), 1);
            KeyValuePair<Vector2Int, int> e = new KeyValuePair<Vector2Int, int>(new Vector2Int(1, 1), 1);
            KeyValuePair<Vector2Int, int> f = new KeyValuePair<Vector2Int, int>(new Vector2Int(1, 2), 2);
            KeyValuePair<Vector2Int, int> g = new KeyValuePair<Vector2Int, int>(new Vector2Int(2, 0), 2);
            KeyValuePair<Vector2Int, int> h = new KeyValuePair<Vector2Int, int>(new Vector2Int(2, 1), 2);
            KeyValuePair<Vector2Int, int> l = new KeyValuePair<Vector2Int, int>(new Vector2Int(2, 2), 3);

            KeyValuePair<Vector2Int, int>[] kvps = new KeyValuePair<Vector2Int, int>[] { a, b, c, d, e, f, g, h, l };
            Dictionary<Vector2Int, KeyValuePair<Vector2Int, int>> keyValuePairs = new Dictionary<Vector2Int, KeyValuePair<Vector2Int, int>>();
            foreach (KeyValuePair<Vector2Int, int> kv in kvps)
            {
                keyValuePairs.Add(kv.Key, kv);
            }

            ///////////////
            //// c | f | l
            //// b | e | h
            //// a | d | g
            //////////////////

            for (int i = 0; i < kvps.Length; i++)
            {
                graph.Add(kvps[i]);
            }


            Assert.AreEqual(9, graph.Count);

            foreach (KeyValuePair<Vector2Int, int> keyValuePair in graph)
            {
                Vector2Int current = keyValuePair.Key;

                for (float angle = 0; angle < Mathf.PI * 2; angle += Mathf.PI / 4)
                {
                    Vector2Int neighbourCoordinates = new Vector2Int(
                            Mathf.RoundToInt(current.x + Mathf.Sin(angle)),
                            Mathf.RoundToInt(current.y + Mathf.Cos(angle))
                            );

                    if (keyValuePairs.ContainsKey(neighbourCoordinates))
                    {
                        Assert.IsTrue(graph.Contains(keyValuePair));
                        Assert.IsTrue(graph.Contains(keyValuePairs[neighbourCoordinates]));

                        ((IConnector<KeyValuePair<Vector2Int, int>>)graph).Connect(keyValuePair, keyValuePairs[neighbourCoordinates]);
                    }
                }
            }

            IConnector<KeyValuePair<Vector2Int, int>> connector = (IConnector<KeyValuePair<Vector2Int, int>>)graph;

            Assert.IsTrue(connector.IsConnected(a, b));
            Assert.IsTrue(connector.IsConnected(a, d));
            Assert.IsTrue(connector.IsConnected(d, e));
            Assert.IsTrue(connector.IsConnected(e, h));
            Assert.IsTrue(connector.IsConnected(h, e));
            Assert.IsTrue(connector.IsConnected(h, l));
            Assert.IsTrue(connector.IsConnected(h, g));
            Assert.IsTrue(connector.IsConnected(c, e));
            Assert.IsTrue(connector.IsConnected(e, d));
            Assert.IsTrue(connector.IsConnected(f, c));
            Assert.IsTrue(connector.IsConnected(a, e));
            Assert.IsTrue(connector.IsConnected(e, l));

            Assert.IsFalse(connector.IsConnected(l, l));
            Assert.IsFalse(connector.IsConnected(l, a));
            Assert.IsFalse(connector.IsConnected(a, c));
            Assert.IsFalse(connector.IsConnected(c, g));
            Assert.IsFalse(connector.IsConnected(f, d));
            Assert.IsFalse(connector.IsConnected(d, f));

            CollectionAssert.AreEquivalent(connector.GetConnected(e),
                new KeyValuePair<Vector2Int, int>[] { a, b, c, d, f, g, h, l });

            CollectionAssert.AreEquivalent(connector.GetConnected(l),
                new KeyValuePair<Vector2Int, int>[] { f, e, h });
        }
    }
}