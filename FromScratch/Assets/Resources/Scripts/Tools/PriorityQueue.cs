using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<T> elementList = new List<T>();
    private List<float> priorityList = new List<float>();

    public List<T> GetElements()
    {
        return elementList;
    }

    public List<float> GetPriorities()
    {
        return priorityList;
    }

    public void Add(T item, float priority)
    {
        elementList.Add(item);
        priorityList.Add(priority);
    }

    public void Sort()
    {
        Quicksort(0, elementList.Count - 1);
    }

    public void Remove(T item)
    {
        int index = elementList.IndexOf(item);
        RemoveAt(index);
    }

    public void RemoveAt(int index)
    {
        elementList.RemoveAt(index);
        priorityList.RemoveAt(index);
    }

    public float GetPriority(T item)
    {
        int index = elementList.IndexOf(item);
        return priorityList[index];
    }

    public float GetPriorityAt(int index)
    {
        return priorityList[index];
    }

    public float GetPriorityMin()
    {
        Sort();
        return GetPriorityAt(0);
    }

    public float GetPriorityMax()
    {
        Sort();
        return GetPriorityAt(elementList.Count - 1);
    }

    public T GetElementPriorityMin()
    {
        Sort();
        return elementList[0];
    }

    public T GetElementPriorityMax()
    {
        Sort();
        return elementList[elementList.Count - 1];
    }

    public void SetPriority(T item, float priority)
    {
        int index = elementList.IndexOf(item);
        priorityList[index] = priority;
    }

    public void SetPriorityAt(int index, float priority)
    {
        priorityList[index] = priority;
    }

    public int IndexOf(T item)
    {
        return elementList.IndexOf(item);
    }

    public bool Contains(T item)
    {
        return elementList.Contains(item);
    }

    public void Clear()
    {
        elementList.Clear();
        priorityList.Clear();
    }

    public T this[int index]
    {
        get
        {
            return elementList[index];
        }
        set
        {
            elementList[index] = value;
        }
    }

    public int Count
    {
        get
        {
            return elementList.Count;
        }
    }

    // Quicksort implementation by FERHAT from http://snipd.net/quicksort-in-c
    private void Quicksort(int left, int right)
    {
        //indices
        int i = left, j = right;
        //value
        float pivot = priorityList[(left + right) / 2];

        while (i <= j)
        {
            while (priorityList[i] < pivot)
                i++;

            while (priorityList[j] > pivot)
                j--;

            if (i <= j)
            { //swap the elements and priorities
                T tmp = elementList[i];
                elementList[i] = elementList[j];
                elementList[j] = tmp;

                float ptmp = priorityList[i];
                priorityList[i] = priorityList[j];
                priorityList[j] = ptmp;

                i++;
                j--;
            }
        }

        if (left < j)
            Quicksort(left, j);

        if (i < right)
            Quicksort(i, right);
    }
}