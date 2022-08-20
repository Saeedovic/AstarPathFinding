using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Graph))]// This is just an extra step to have the Graph component as well
public class GraphView : MonoBehaviour // This graphview class coverts the textdata to onscreen nodeview
{
    public GameObject nodeViewPrefab;
    public NodeView[,] nodeViews;
    

    public Color baseColor = Color.white; // for the open nodes
    public Color wallColor = Color.black; // for the closed nodes

    public void Init(Graph graph)
    {
        if(graph == null)
        {
            Debug.Log("No Graph to Init");
            return;
        }

        nodeViews = new NodeView[graph.Width, graph.Height];
        foreach (Node n in graph.nodes)
        {
            GameObject instance = Instantiate(nodeViewPrefab, Vector3.zero, Quaternion.identity);
            NodeView nodeView = instance.GetComponent<NodeView>();

            if(nodeView != null)
            {
                nodeView.Init(n);
                nodeViews[n.xIndex, n.yIndex] = nodeView;


            }

            if (n.nodeType == NodeType.Closed)
            {
                nodeView.ColorNode(wallColor);
            }
            else
            {
                nodeView.ColorNode(baseColor);
            }
        }
    }

    public void ColorNodes(List<Node> nodes, Color color)
    {

        foreach (Node n in nodes)
        {
            NodeView nodeView = nodeViews[n.xIndex, n.yIndex];

            if(nodeView != null)
            {
                nodeView.ColorNode(color);
            }
        }
    }

    public void ShowNodeArrows(Node node, Color color)
    {
        if(node != null)
        {
            NodeView nodeView = nodeViews[node.xIndex, node.yIndex];
            if(nodeView != null)
            {
                nodeView.ShowArrow(color);
            }
        }

    }

    public void ShowNodeArrows(List<Node> nodes, Color color)
    {
        foreach(Node n in nodes)
        {
            ShowNodeArrows(n, color);
        }
    }
}
