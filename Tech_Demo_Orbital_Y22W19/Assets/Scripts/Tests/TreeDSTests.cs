using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataStructures;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Tree
{
    public class TreeDSTests
    {
        [Test]
        public void FailsIllegalParenting()
        {
            ITree<int> tree = new Tree<int>();
            tree.Add(1);
            tree.Add(0);
            Assert.IsTrue(tree.Connect(1, 0));
            tree.Add(2);
            Assert.IsFalse(tree.Connect(2, 0));
            Assert.IsFalse(tree.IsConnected(2, 0));
            Assert.AreEqual(1, tree.GetParent(0));
        }

        [Test]
        public void CorrectlyShowsNoParent()
        {
            ITree<int?> tree = new Tree<int?>();

            int? one = 1;
            int? two = 2;

            tree.Add(one);
            tree.Add(two);

            tree.Connect(one, two);

            Assert.AreEqual(null, tree.GetParent(one));
        }

        [Test]
        public void CorrectlyShowsZeroAsParent()
        {
            ITree<int> tree = new Tree<int>();


            tree.Add(0);
            tree.Add(1);

            tree.Connect(0, 1);

            Assert.AreEqual(0, tree.GetParent(0));
        }

        [Test]
        public void CorrectlyInterpretsRemovalAsDisconnectedTree()
        {
            ITree<string> tree = new Tree<string>();

            tree.Add("a");
            tree.Add("b");
            tree.Add("c");
            tree.Add("d");

            tree.Connect("a", "b");
            tree.Connect("a", "c");
            tree.Connect("c", "d");

            tree.Remove("c");

            Assert.AreEqual(null, tree.GetParent("d"));
            CollectionAssert.AreEquivalent(new string[] { "b" }, tree.GetConnected("a"));
        }

        [Test]
        public void CorrectlyReparentsChild()
        {
            ITree<string> tree = new Tree<string>();

            tree.Add("a");
            tree.Add("b");
            tree.Add("c");
            tree.Add("d");

            tree.Connect("a", "b");
            tree.Connect("a", "c");
            tree.Connect("c", "d");

            Assert.AreEqual("c", tree.GetParent("d"));
            CollectionAssert.AreEquivalent(new string[] { "d" }, tree.GetConnected("c"));

            tree.Reparent("d", "a");

            Assert.AreEqual("a", tree.GetParent("d"));
            CollectionAssert.AreEquivalent(new string[] { "b", "c", "d" }, tree.GetConnected("a"));
            CollectionAssert.AreEquivalent(new string[] { }, tree.GetConnected("c"));
            Assert.IsFalse(tree.IsConnected("c", "d"));
        }
    }
}
