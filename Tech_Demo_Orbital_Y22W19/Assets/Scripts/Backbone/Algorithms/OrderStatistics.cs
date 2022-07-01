using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DataStructures;
using MathS = System.Math;

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
                int p_end = QuickSelectPartition(Array, start, end, comp);
                int p_start = PackDuplicates(Array, start, p_end);


                if (p_start <= k && k <= p_end)
                {
                    return Array[p_end];
                }
                else if (k < p_start)
                {
                    end = MathS.Min(p_start, end - 1);
                }
                else
                {
                    start = MathS.Max(start + 1, p_end + 1);
                }
            }
            return Array[start];
        }

        private static int QuickSelectPartition<T>(T[] Array, int start, int end, IComparer<T> cmp)
        {
            
            System.Random random = new System.Random();
            int pivotIndex = random.Next(start, end); //exclusive of end [start, end -1]
            T pivot = Array[pivotIndex];
            int low = start + 1;
            int high = end;

            //Swap pivotIndex and start
            Swap(Array, pivotIndex, start);

            while (low < high)
            {
                while (low < high && cmp.Compare(Array[low], pivot) <= 0) low++;
                while (low < high && (high >= end || cmp.Compare(Array[high], pivot) > 0)) high--;
                if (low < high) Swap(Array, low, high);
            }
            low -= 1;
            Swap(Array, start, low);
            return low;

        }

        //packduplicates packs all duplicates from [start pivot_index]
        private static int PackDuplicates<T> (T[] Array, int start, int pivot_index)
        {
            int index = start; 
            while (index < pivot_index)
            {
                if (Array[index].Equals(Array[pivot_index]))
                {
                    while (Array[pivot_index].Equals(Array[index]) && index < pivot_index) pivot_index--;
                    Swap(Array, pivot_index, index);
                }
                index++;
            }
            return pivot_index;
        }

        private static void Swap<T> (T[] array, int firstIndex, int secondIndex)
        {
            T temp = array[firstIndex];
            array[firstIndex] = array[secondIndex];
            array[secondIndex] = temp;
        }
        
    }
}