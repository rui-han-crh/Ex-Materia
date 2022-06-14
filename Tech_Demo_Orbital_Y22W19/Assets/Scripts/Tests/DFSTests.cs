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
    public class DFSTests
    {
        [Test]
        public void DFSBacktrack()
        {
            string[] strings = { "a", "b", "c", "d", "e", "f", "g", "h", "i" };

            ITree<string> tree = new Tree<string>(strings);

            //                  a
            //           b           c
            //        d     e     f     g
            //                      h
            //                     i 

            tree.Connect("a", "b");
            tree.Connect("b", "d");
            tree.Connect("b", "e");
            tree.Connect("a", "c");
            tree.Connect("c", "f");
            tree.Connect("f", "h");
            tree.Connect("h", "i");
            tree.Connect("c", "g");

            string output = "";

            SearchAlgorithms.DepthFirstSearch("a", x => tree.GetConnected(x),
                                                x => { }, x => output += $"{x}, " );

            Assert.AreEqual("d, e, b, i, h, f, g, c, a, ", output);

        }

        [Test]
        public void DFSBacktrackWithNullable()
        {
            int?[] integers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            ITree<int?> tree = new Tree<int?>(integers);

            //                  1
            //           2           5
            //        3     4     6     9
            //                              7
            //                            8 

            tree.Connect(1, 2);
            tree.Connect(1, 5);
            tree.Connect(2, 3);
            tree.Connect(2, 4);
            tree.Connect(5, 6);
            tree.Connect(5, 9);
            tree.Connect(9, 7);
            tree.Connect(7, 8);

            string output = "";

            SearchAlgorithms.DepthFirstSearch(tree.Root, x => tree.GetConnected(x),
                                                x => { }, x => output += $"{x}, ");

            Assert.AreEqual("3, 4, 2, 6, 8, 7, 9, 5, 1, ", output);

        }
    }
}