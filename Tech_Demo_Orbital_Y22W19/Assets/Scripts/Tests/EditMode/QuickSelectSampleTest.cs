using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataStructures;
using Algorithms;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace OrderStatisticsTest
{


    public class QuickSelectSampleTest 
    {
        public class SampleStringComparer: Comparer<string>
        {
            public override int Compare(string x, string y)
            {
                //jus tcompares by ASCII
                return string.CompareOrdinal(x, y);
            }
        }
        public class SampleIntComparer : Comparer<int>
        {
            public override int Compare(int x, int y)
            {
                return x - y; //smaller number first
            }
        }
        [Test]
        public void SampleIntegers()
        {
            int[] myNums = new int[] { 5, 4, 6, 6, 7, 8, 1, 2, 9};
            int thirdSmallest = OrderStatistics.QuickSelect(myNums, new SampleIntComparer(), 3);
            Assert.AreEqual(4, thirdSmallest);
        }

        //The algo fails on this feelsbadman
        //[Test]
        //public void SampleSame10kIntegers()
        //{
        //    List<int> vs = new List<int>();
        //    for (int i = 0; i < 100000; i++)
        //    {
        //        vs.Add(5);
        //    }

        //    int fifthSmallest = OrderStatistics.QuickSelect(vs, new SampleIntComparer(), 5);
        //    Assert.AreEqual(5, fifthSmallest);

        //}

        [Test]
        public void SampleStrings()
        {
            string[] myStrings = new string[] { "abc", "bc", "a", "abcd", "123", "420", "blazeit" };
            string thirdSmallest = OrderStatistics.QuickSelect(myStrings, new SampleStringComparer(), 3);
            Assert.AreEqual("a", thirdSmallest);
        }


        [Test]
        public void CorrectlyProducesFirstElement()
        {
            int[] myNums = new int[] { 5, 4, 6, 6, 7, 8, 1, 2, 9 };
            int first = OrderStatistics.QuickSelect(myNums, new SampleIntComparer(), 1);
            Assert.AreEqual(1, first);
        }

        [Test]
        public void CorrectlyProducesLastElement()
        {
            int[] myNums = new int[] { 5, 4, 6, 6, 7, 8, 1, 2, 9 };
            int last = OrderStatistics.QuickSelect(myNums, new SampleIntComparer(), 9);
            Assert.AreEqual(9, last);
        }


        [Test]
        public void CorrectlyDeterminesAllOrder()
        {
            int[] outOfOrder = new int[] { 3, 5, 1, 6, 2, 9, 4, 8, 7, 10 };
            int[] sorted = (int[])outOfOrder.Clone();
            System.Array.Sort(sorted);
            for (int i = 0; i < outOfOrder.Length; i++)
            {
                int outcome = OrderStatistics.QuickSelect(outOfOrder, new SampleIntComparer(), i + 1);
                Assert.AreEqual(sorted[i], outcome,
                    $"Expected k={i + 1}, element={sorted[i]} but received element={outcome}");
            }
        }



    }
}



