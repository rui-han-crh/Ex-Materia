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
            k = (array.Count() - k) + 1;
            //Out of bounds check 
            if (k > array.Count())
            {
                k = array.Count();
            }

            if (k < 1)
            {
                k = 1;
            }

            T[] Array = array.ToArray();

            int maxid = -1;
            int start = 0;
            int end = array.Count() - 1;
            k -= 1;

            while (k != maxid)
            {
                if (end == start)
                {
                    maxid = end;
                    break;
                }
                maxid = start - 1;
                T pivot = Array[end];
                for (int i = start; i < end; ++i)
                {
                    if (comp.Compare(Array[i], pivot) >= 0 && ++maxid != i)
                    {
                        Swap(Array, maxid, i);
                    }
                }

                Array[end] = Array[++maxid];
                Array[maxid] = pivot;
                if (k < maxid)
                {
                    end = maxid - 1;
                }
                else
                {
                    start = maxid + 1;
                }
            }
            return Array[maxid];
        }


        //private static int QuickSelectPartition<T>(T[] array, int startIndex, int endIndex, IComparer<T> comp)
        //{
        //    T pivot = array[endIndex];
        //    int i = (startIndex - 1);
        //    for (int j  = startIndex; j < endIndex; j++)
        //    {
        //        if(comp.Compare(array[j], pivot) <= 0 ) //array[i]<= pivot
        //        {
        //            i += 1;
        //            move elements less behind pivotlocation
        //            Swap(array, i,j);
 
        //        }

        //    }
        //    i += 1;
        //    swap pivot to new pivotlocation;
        //    Swap(array, i, endIndex);
        //    return i;
        //}

        private static void Swap<T> (T[] array, int firstIndex, int secondIndex)
        {
            T temp = array[firstIndex];
            array[firstIndex] = array[secondIndex];
            array[secondIndex] = temp;
        }
        
    }
}