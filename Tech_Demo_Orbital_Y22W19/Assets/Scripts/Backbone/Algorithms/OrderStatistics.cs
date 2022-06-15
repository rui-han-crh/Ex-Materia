using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataStructures;

namespace Algorithms
{
    public static class OrderStatistics
    {
        /*QuickSelect Returns the kth largest element!
         *From a custom comparer
         */

        public static T QuickSelect<T>(IEnumerable<T> array, IComparer<T> comp, int k)
        {
            if (k > array.Count())
            {
                throw new Exception("k is too big!");
            }
            k -= 1;
            int start = 0;
            int end = array.Count();

            T[] Array = array.ToArray();

            while (end - start > 1)
            {
                int p = QuickSelectPartition(Array, start, end, comp);
                if (p == k)
                {
                    return Array[p];
                }
                else if (k < p)
                {
                    end = p;
                }
                else
                {
                    start = p + 1;
                }
            }
            return Array[start];
        }

        private static int QuickSelectPartition<T>(T[] Array, int start, int end, IComparer<T> cmp)
        {
            
            System.Random random = new System.Random();
            int pivotIndex = random.Next(start, end); //exclusive of end [start, end -1]
            T pivot = Array[pivotIndex];
            int low = 1;
            int high = Array.Length;

            //Swap pivotIndex and start
            Swap(Array, pivotIndex, start);

            while (low < high)
            {
                while (low < high && cmp.Compare(Array[low], pivot) <= 0) low++;
                while (low < high && (high >= Array.Length || cmp.Compare(Array[high], pivot) > 0)) high--;
                if (low < high) Swap(Array, low, high);
            }
            low -= 1;
            Swap(Array, start, low);
            return low;

        }

        private static void Swap<T> (T[] array, int firstIndex, int secondIndex)
        {
            T temp = array[firstIndex];
            array[firstIndex] = array[secondIndex];
            array[secondIndex] = temp;
        }
        
    }
}