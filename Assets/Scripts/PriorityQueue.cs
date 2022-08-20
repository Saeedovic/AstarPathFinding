using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PriorityQueue<T > where T : IComparable<T> //TO check whatever the object type is it implements the icmaparable interface 
{
    List<T> data;
    public int Count { get { return data.Count; } } // instead of using a Count method im just using a count property to read only

    public PriorityQueue()
    {
        data = new List<T>();
    }


    public void Enqueue(T item)
    {
        data.Add(item);

        int childindex = data.Count - 1; // Thats for the first index which is zero


        while (childindex > 0)
        {
            int parentindex = (childindex - 1) / 2;

            if (data[childindex].CompareTo(data[parentindex]) >= 0)
            {
                break;

            }

            T tmp = data[childindex];
            data[childindex] = data[parentindex];
            data[parentindex] = tmp;

            childindex = parentindex;

        }

    }

    public T Dequeue()
    {
        int lastindex = data.Count - 1;
        T frontItem = data[0];

        data[0] = data[lastindex];

        data.RemoveAt(lastindex);

        lastindex--;

        int parentindex = 0;


        while (true)
        {
            int childindex = parentindex * 2 + 1;   // to get the child index on the left 

            if(childindex > lastindex)
            {
                break;
            }

            int rightchild = childindex + 1; // comparing the right child with the left child, to find the right child its leftchildindex plus one 

            if(rightchild <= lastindex && data[rightchild].CompareTo(data[parentindex]) < 0)
            {
                childindex = rightchild;
            }

            if (data[parentindex].CompareTo(data[childindex]) <= 0)
            {
                break;
            }

            T tmp = data[parentindex];
            data[parentindex] = data[childindex];
            data[childindex] = tmp;

            parentindex = childindex;
        }

        return frontItem;

    }

    public T Peek()
    {
        T frontItem = data[0];
        return frontItem;
    }

    public bool Contains(T item)
    {
        return data.Contains(item);
    }


    public List<T> ToList()
    {
        return data;
    }
}
