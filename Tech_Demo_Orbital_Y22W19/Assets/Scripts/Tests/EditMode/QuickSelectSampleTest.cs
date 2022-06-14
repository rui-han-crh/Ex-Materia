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
                return y - x; //smaller number first
            }
        }
        [Test]
        public void SampleIntegers()
        {
            int[] myNums = new int[] { 5, 4, 6, 6, 7, 8, 1, 2, 9};
            int thirdSmallest = OrderStatistics.QuickSelect(myNums, new SampleIntComparer(), 3);
            Assert.AreEqual(4, thirdSmallest);
        }

        [Test]
        public void SampleSame10kIntegers()
        {
            List<int> vs = new List<int>();
            for (int i = 0; i < 100000; i++)
            {
                vs.Add(5);
            }

            int fifthSmallest = OrderStatistics.QuickSelect(vs, new SampleIntComparer(), 5);
            Assert.AreEqual(5, fifthSmallest);

        }

        [Test]
        public void SampleStrings()
        {
            string[] myStrings = new string[] { "abc", "bc", "a", "abcd", "123", "420", "blazeit" };
            string thirdSmallest = OrderStatistics.QuickSelect(myStrings, new SampleStringComparer(), 3);
            Assert.AreEqual("abcd", thirdSmallest);
        }
    }
}



