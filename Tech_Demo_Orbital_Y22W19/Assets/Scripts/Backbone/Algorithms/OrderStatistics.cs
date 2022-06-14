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
                return default(T);
            }


            int adjustedIndex = array.Count() - k - 1;
            List<T> PartiallySortedArray = array.ToList(); //I'll be swapping this in place.
            int startIndex = 0;
            var endIndex = array.Count() - 1;
            var pivotIndex = adjustedIndex;
            System.Random rng = new System.Random();
            while (endIndex > startIndex)
            {
                //update pivotIndex
                pivotIndex = QuickSelectPartition(PartiallySortedArray, startIndex, endIndex, pivotIndex, comp);
                if (pivotIndex == adjustedIndex)
                {
                    break;
                }
                if (pivotIndex > adjustedIndex)
                {
                    endIndex = pivotIndex - 1;
                }
                else
                {
                    startIndex = pivotIndex + 1;
                }
                pivotIndex = rng.Next(startIndex, endIndex);
            }
            return PartiallySortedArray[adjustedIndex];
        }

        private static int QuickSelectPartition<T>(List<T> array, int startIndex, int endIndex, int pivotIndex, IComparer<T> comp)
        {
            T pivotValue = array[pivotIndex];
            //I can't seem to pass this by reference 
            Swap(array, startIndex, endIndex);
            for (int i = startIndex; i < endIndex; i++)
            {
                if (comp.Compare(array[i], pivotValue) <= 0 )
                {
                    Swap(array, i, startIndex);
                    startIndex++;
                }

            }
            Swap(array, endIndex, startIndex);
            return startIndex;
        }

        private static void Swap<T> (List<T> array, int firstIndex, int secondIndex)
        {
            T temp = array[firstIndex];
            array[firstIndex] = array[secondIndex];
            array[secondIndex] = temp;
        }
        
    }
}