using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum NodeType 
{
    Open = 0,
    Closed = 1,
}
public class Node : IComparable<Node> // its not inherieting from icomaparable , it needs a method to compare
{
    public NodeType nodeType = NodeType.Open;

    public int xIndex = -1;
    public int yIndex = -1;

    public Vector3 position;

    public float distanceTraveled = Mathf.Infinity; // it doesnt have a useful value yet 
    public List<Node> neighbors = new List<Node>();

    public float priority;
    public Node previous = null;



    public Node( int xIndex, int yIndex, NodeType nodeType)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        this.nodeType = nodeType;
       
    }

    public int CompareTo(Node other)
    {
        if(this.priority < other.priority)
        {
            return -1;
        } 
        else if(this.priority > other.priority)
        {
            return 1;
        }
        else
        {
            return 0; // it will return zero if its equal 
           
        }

    }

    public void Reset()
    {
        previous = null;
    }
}
