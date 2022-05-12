using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MinHeap<T> : IPriorityQueue<T> where T : IComparable<T>
{
    private List<T> elements;
    private Dictionary<T, int> indexMap = new Dictionary<T,int>();

    public MinHeap(int size)
    {
        elements = new List<T>(size);
    }

    public MinHeap()
    {
        elements = new List<T>();
    }

    private int GetLeftChildIndex(int elementIndex)
    {
        return 2 * elementIndex + 1;
    }

    private int GetRightChildIndex(int elementIndex)
    {
        return 2 * elementIndex + 2;
    }

    private int GetParentIndex(int elementIndex)
    {
        return (elementIndex - 1) / 2;
    }

    private bool HasLeftChild(int elementIndex)
    {
        return GetLeftChildIndex(elementIndex) < elements.Count;
    }

    private bool HasRightChild(int elementIndex)
    {
        return GetRightChildIndex(elementIndex) < elements.Count;
    }

    private bool IsRoot(int elementIndex)
    {
        return elementIndex == 0;
    }

    private T GetLeftChild(int elementIndex)
    {
        return elements[GetLeftChildIndex(elementIndex)];
    }

    private T GetRightChild(int elementIndex)
    {
        return elements[GetRightChildIndex(elementIndex)];
    }

    private T GetParent(int elementIndex)
    {
        return elements[GetParentIndex(elementIndex)];
    }

    public T Peek() 
    { 
        if (elements.Count == 0)
        {
            return default;
        }

        return elements[0]; 
    }

    public void Build(T[] items)
    {
        MinHeap<T> newHeap = Heapify(items);
        elements = newHeap.elements;
        indexMap = newHeap.indexMap;
        // destroy the newHeap, do not alias
    }

    public static MinHeap<T> Heapify(T[] items)
    {
        // TODO: Implement the actual heapify, I'm just inserting n elements now
        MinHeap<T> heap = new MinHeap<T>(items.Length);
        foreach (T item in items)
        {
            heap.Add(item);
        }
        return heap;
    }

    private void Swap(int firstIndex, int secondIndex)
    {
        T item = elements[firstIndex];
        elements[firstIndex] = elements[secondIndex];
        elements[secondIndex] = item;

        indexMap[elements[firstIndex]] = firstIndex;
        indexMap[elements[secondIndex]] = secondIndex;
    }

    public bool IsEmpty()
    {
        return elements.Count == 0;
    }

    public void Clear()
    {
        elements.Clear();
        indexMap.Clear();
    }

    public void Remove(T element)
    {
        if (!Contains(element))
        {
            throw new KeyNotFoundException($"{element} does not exist in the heap to be removed");
        }
        
        int elementIndex = indexMap[element];
        Swap(elementIndex, elements.Count - 1);

        Debug.Assert(indexMap[elements[elementIndex]] == elementIndex, "indexMap and array desynchronised on remove");
        indexMap.Remove(element);

        elements.RemoveAt(elements.Count - 1);

        BubbleDown(elementIndex);
    }

    public T Extract()
    {
        if (elements.Count == 0)
        {
            throw new IndexOutOfRangeException();
        }

        T result = elements[0];
        indexMap.Remove(elements[0]);

        elements[0] = elements[elements.Count - 1];

        elements.RemoveAt(elements.Count - 1);
        if (elements.Count > 0)
        {
            indexMap[elements[0]] = 0;
        }

        BubbleDown();
        return result;
    }

    public bool Contains(T element)
    {
        return indexMap.ContainsKey(element);
    }

    public void DecreaseKey(T element)
    {
        if (!Contains(element))
        {
            throw new KeyNotFoundException("There is no such element in the heap");
        }

        int elementIndex = indexMap[element];

        Debug.Assert(elements[elementIndex].Equals(element));

        elements[elementIndex] = element;

        BubbleUp(elementIndex);
    }

    public void IncreaseKey(T element)
    {
        if (!Contains(element))
        {
            throw new KeyNotFoundException("There is no such element in the heap");
        }

        int elementIndex = indexMap[element];

        Debug.Assert(elements[elementIndex].Equals(element));

        elements[elementIndex] = element;

        BubbleDown(elementIndex);
    }

    public void Add(T element)
    {
        if (Contains(element))
        {
            throw new Exception($"{element} already exists in heap");
        }

        elements.Add(element);
        indexMap.Add(element, elements.Count - 1);
        BubbleUp(elements.Count - 1);
    }


    private void BubbleDown(int index = 0)
    {
        int childIndex;
        while (HasLeftChild(index))
        {
            childIndex = GetLeftChildIndex(index);
            if (HasRightChild(index) && GetRightChild(index).CompareTo(GetLeftChild(index)) < 0)
            {
                childIndex = GetRightChildIndex(index);
            }

            if (elements[childIndex].CompareTo(elements[index]) >= 0)
            {
                break;
            }

            Swap(childIndex, index);
            index = childIndex;
        }
    }

    private void BubbleUp(int index)
    {
        while (!IsRoot(index))
        {
            int parentIndex = GetParentIndex(index);
            if (elements[index].CompareTo(GetParent(index)) < 0) 
            {
                Swap(parentIndex, index);
            }
            index = parentIndex;
        }
    }

    public int Count()
    {
        return elements.Count;
    }
}
