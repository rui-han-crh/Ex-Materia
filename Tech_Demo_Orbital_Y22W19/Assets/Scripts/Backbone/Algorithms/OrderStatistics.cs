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
            //Out of bounds check 
            if (k > array.Count() || k < 0)
            {
                throw new IndexOutOfRangeException("k must be more than 0, less than array's size");
            }

            T[] tempCopy=  array.ToArray();
            int left = 0;
            int right = array.Count() - 1;
            while (left <= right)
            {
                int pivotIndex = QuickSelectPartition(tempCopy, left, right, comp);
                if (pivotIndex == k -1)
                {
                    return tempCopy[pivotIndex];
                }
                else if (pivotIndex > k - 1)
                {
                    right = pivotIndex--;
                }
                else
                {
                    left = pivotIndex++;
                }
            }
            throw new Exception("Unreachable Code");

        }

        private static int QuickSelectPartition<T>(T[] array, int startIndex, int endIndex, IComparer<T> comp)
        {
            T pivot = array[startIndex];
            int newStart = startIndex - 1;
            for (int i  = startIndex; i < endIndex; i++)
            {
                if(comp.Compare(array[i], pivot) <= 0 ) //array[i]<= pivot
                {
                    newStart += 1;
                    Swap(array, newStart, i); 
                }

            }
            newStart += 1;
            Swap(array, newStart, endIndex);
            return newStart + 1;
        }

        private static void Swap<T> (T[] array, int firstIndex, int secondIndex)
        {
            T temp = array[firstIndex];
            array[firstIndex] = array[secondIndex];
            array[secondIndex] = temp;
        }
        
    }
}