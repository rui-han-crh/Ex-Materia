using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Reflection;
using System;
using DataStructures;
using System.Linq;

public class UndirectedGraphTest
{
    [Test]
    public void Instantiation()
    {
        IUndirectedGraph<Vector2> graph = new UndirectedGraph<Vector2>();
        graph.Add(new Vector2(1, 0));
        graph.Add(new Vector2(0, 0));

        Assert.AreEqual(2, graph.Count);
    }

    [Test]
    public void EdgeContractionButNotConnected()
    {
        IUndirectedGraph<Vector2> graph = new UndirectedGraph<Vector2>();
        graph.Add(new Vector2(0, 0));
        graph.Add(new Vector2(1, 0));
        graph.Add(new Vector2(0, 1));
        graph.Add(new Vector2(1, 1));

        graph.ContractEdgeBetween(new Vector2(1, 0), new Vector2(0, 1), new Vector2(0.5f, 0.5f));

        Assert.AreEqual(4, graph.Count);
    }

    [Test]
    public void EdgeContractionValid()
    {
        IUndirectedGraph<Vector2> graph = new UndirectedGraph<Vector2>();
        Vector2 a = new Vector2(0, 0);
        Vector2 b = new Vector2(1, 0);
        Vector2 c = new Vector2(0, 1);
        Vector2 d = new Vector2(1, 1);

        graph.Add(a);
        graph.Add(b);
        graph.Add(c);
        graph.Add(d);
        graph.Connect(a, b);
        graph.Connect(b, c);
        graph.Connect(c, d);

        graph = graph.ContractEdgeBetween(b, c, (b + c) / 2);

        Assert.AreEqual(3, graph.Count);

        Assert.IsTrue(graph.IsConnected(a, new Vector2(0.5f, 0.5f)));
        Assert.IsTrue(graph.IsConnected(d, new Vector2(0.5f, 0.5f)));
    }

    [Test]
    public void ConnectEdge()
    {
        IUndirectedGraph<Vector2> graph = new UndirectedGraph<Vector2>();
        Vector2 a = new Vector2(0, 0);
        Vector2 b = new Vector2(1, 0);
        Vector2 c = new Vector2(0, 1);
        Vector2 d = new Vector2(1, 1);

        graph.Add(a);
        graph.Add(b);
        graph.Add(c);
        graph.Add(d);
        graph.Connect(a, b);
        graph.Connect(a, c);
        graph.Connect(a, d);
        graph.Connect(b, d);

        Assert.AreEqual(2, graph.GetConnected(d).Count());
        Assert.AreEqual(3, graph.GetConnected(a).Count());
        Assert.AreEqual(2, graph.GetConnected(b).Count());
    }

    [Test]
    public void DisconnectEdge()
    {
        IUndirectedGraph<Vector2> graph = new UndirectedGraph<Vector2>();
        Vector2 a = new Vector2(0, 0);
        Vector2 b = new Vector2(1, 0);
        Vector2 c = new Vector2(0, 1);
        Vector2 d = new Vector2(1, 1);

        graph.Add(a);
        graph.Add(b);
        graph.Add(c);
        graph.Add(d);
        graph.Connect(a, b);
        graph.Connect(a, c);
        graph.Connect(a, d);
        graph.Connect(b, d);

        graph.Disconnect(d, a);

        Assert.IsFalse(graph.IsConnected(a, d));

        Assert.AreEqual(1, graph.GetConnected(d).Count());
        Assert.AreEqual(2, graph.GetConnected(a).Count());
        Assert.AreEqual(2, graph.GetConnected(b).Count());
    }

    [Test]
    public void RemoveWithoutDisconnecting()
    {
        IUndirectedGraph<Vector2> graph = new UndirectedGraph<Vector2>();
        Vector2 a = new Vector2(0, 0);
        Vector2 b = new Vector2(1, 0);

        graph.Add(a);
        graph.Add(b);

        graph.Connect(a, b);
        graph.Remove(b);

        Assert.AreEqual(0, graph.GetConnected(a).Count());
    }

    [Test]
    public void RemoveFromStar()
    {
        IUndirectedGraph<Vector2> graph = new UndirectedGraph<Vector2>();
        Vector2 a = new Vector2(0, 0);
        Vector2 b = new Vector2(1, 0);
        Vector2 c = new Vector2(0, 1);
        Vector2 d = new Vector2(1, 1);
        Vector2 e = new Vector2(0, -1);
        Vector2 f = new Vector2(-1, 0);
        Vector2 g = new Vector2(-1, -1);

        graph.Add(a);
        graph.Add(b);
        graph.Add(c);
        graph.Add(d);
        graph.Add(e);
        graph.Add(f);
        graph.Add(g);

        graph.Connect(b, a);
        graph.Connect(c, a);
        graph.Connect(d, a);
        graph.Connect(e, a);
        graph.Connect(f, a);
        graph.Connect(g, a);

        graph.Remove(a);

        Assert.IsFalse(graph.IsConnected(g, a));

        Assert.AreEqual(0, graph.GetConnected(b).Count());
        Assert.AreEqual(0, graph.GetConnected(c).Count());
        Assert.AreEqual(0, graph.GetConnected(d).Count());
        Assert.AreEqual(0, graph.GetConnected(e).Count());
        Assert.AreEqual(0, graph.GetConnected(f).Count());
        Assert.AreEqual(0, graph.GetConnected(g).Count());
    }

    [Test]
    public void ChildrenOfStar()
    {
        IUndirectedGraph<Vector2> graph = new UndirectedGraph<Vector2>();
        Vector2 a = new Vector2(0, 0);
        Vector2 b = new Vector2(0, 1);
        Vector2 c = new Vector2(0, 2);
        Vector2 d = new Vector2(1, 0);
        Vector2 e = new Vector2(1, 1);
        Vector2 f = new Vector2(1, 2);
        Vector2 g = new Vector2(2, 0);
        Vector2 h = new Vector2(2, 1);
        Vector2 i = new Vector2(2, 2);

        graph.Add(a);
        graph.Add(b);
        graph.Add(c);
        graph.Add(d);
        graph.Add(e);
        graph.Add(f);
        graph.Add(g);
        graph.Add(h);
        graph.Add(i);

        graph.Connect(a, b);
        graph.Connect(a, c);
        graph.Connect(a, e);

        graph.Connect(b, c);
        graph.Connect(b, d);
        graph.Connect(b, e);
        graph.Connect(b, f);

        graph.Connect(c, e);
        graph.Connect(c, f);

        graph.Connect(d, e);
        graph.Connect(d, g);
        graph.Connect(d, h);

        graph.Connect(e, f);
        graph.Connect(e, g);
        graph.Connect(e, h);
        graph.Connect(e, i);


        graph.Connect(f, h);
        graph.Connect(f, i);

        graph.Connect(g, h);

        graph.Connect(h, i);

        CollectionAssert.AreEquivalent(new Vector2[] { a, b, c, d, f, g, h, i }, graph.GetConnected(e));
        CollectionAssert.AreEquivalent(new Vector2[] { d, e, f, g, i }, graph.GetConnected(h));
    }
}
