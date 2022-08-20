using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathFinder : MonoBehaviour
{
    Node m_startNode;
    Node m_goalNode;
    Graph m_graph;
    GraphView m_graphView;

    PriorityQueue<Node> m_frontierNodes;
    List<Node> m_exploredNodes;
    List<Node> m_pathNodes;

    public Color startColor = Color.green;
    public Color goalColor = Color.red;
    public Color frontierColor = Color.magenta;
    public Color exploredColor = Color.gray;
    public Color pathColor = Color.cyan;
    public Color arrowColor = new Color(0.85f, 0.85f, 0.85f, 1f); // Getting the colors using Rgba values 
    public Color highlightColor = new Color(1f, 1f, 0.5f, 1f);

    public bool showIterations = true;
    public bool showColors = true;
    public bool showArrows = true;
    public bool exitOnGoal = true;

    public bool isComplete = false;
   public int m_iterations = 0; // This is for how many iterations it took to reach the Goal

    public enum Mode
    {
        BreadthSearch = 0,
        Dijkstra = 1,
        GreedyBestFirst = 2,
        AStar = 3
    }

    public Mode mode = Mode.BreadthSearch;

    public void Init(Graph graph, GraphView graphView, Node start, Node goal)
    {
        if (start == null || goal == null || graph == null || graphView == null)
        {
            Debug.LogWarning( "Error btw missing some components ");
            return;
        }

        if (start.nodeType == NodeType.Closed || goal.nodeType == NodeType.Closed)
        {
            Debug.LogWarning(" start and goal nodes must be unblocked!");
            return;
        }

        m_graph = graph;
        m_graphView = graphView;
        m_startNode = start;
        m_goalNode = goal;

        ShowColors(graphView, start, goal);

        m_frontierNodes = new PriorityQueue<Node>();
        m_frontierNodes.Enqueue(start);
        m_exploredNodes = new List<Node>();
        m_pathNodes = new List<Node>();

        for (int x = 0; x < m_graph.Width; x++)
        {
            for (int y = 0; y < m_graph.Height; y++)
            {
                m_graph.nodes[x, y].Reset();
            }
        }

        isComplete = false;
        m_iterations = 0;
        m_startNode.distanceTraveled = 0;
    }
    void ShowColors()
    {
        ShowColors(m_graphView, m_startNode, m_goalNode);
    }

    void ShowColors(GraphView graphView, Node start, Node goal)
    {
        if (graphView == null || start == null || goal == null)
        {
            return;
        }

        if (m_frontierNodes != null)
        {
            graphView.ColorNodes(m_frontierNodes.ToList(), frontierColor); // i HAVE added the To list is because m_frontier is a queue and not a list and here im converting it 
        }

        if (m_exploredNodes != null)
        {
            graphView.ColorNodes(m_exploredNodes, exploredColor);
        }

        if(m_pathNodes != null)
        {
            graphView.ColorNodes(m_pathNodes, pathColor);
        }
        NodeView startNodeView = graphView.nodeViews[start.xIndex, start.yIndex];

        if (startNodeView != null)
        {
            startNodeView.ColorNode(startColor);
        }

        NodeView goalNodeView = graphView.nodeViews[goal.xIndex, goal.yIndex];

        if (goalNodeView != null)
        {
            goalNodeView.ColorNode(goalColor);
        }
    }

    public IEnumerator SearchRoutine(float timeStep = 0.1f)
    {
        float timeStart = Time.realtimeSinceStartup;
        yield return null;

        while (!isComplete)
        {
            if (m_frontierNodes.Count > 0)
            {
                Node currentNode = m_frontierNodes.Dequeue();
                m_iterations++;

                if (!m_exploredNodes.Contains(currentNode))
                {
                    m_exploredNodes.Add(currentNode);
                }
                if (mode == Mode.BreadthSearch)
                {

                ExpandFrontierBreadthSearch(currentNode);
                }
                else if (mode == Mode.Dijkstra)
                {
                    ExpandFrontierDijkstra(currentNode);     

                }
                else if (mode == Mode.GreedyBestFirst)
                {
                    ExpandFrontierGreedyBestFirst(currentNode);
                }
                else if( mode == Mode.AStar)
                {
                    ExpandFrontierAStar(currentNode);
                }

                if (m_frontierNodes.Contains(m_goalNode))
                {
                    m_pathNodes = GetPathNodes(m_goalNode); 
                    if (exitOnGoal)
                    {
                        isComplete = true;
                    }
                }
                if (showIterations)
                {
                    ShowDiagnostics();

                    yield return new WaitForSeconds(timeStep);
                }
            }
            else
            {
                isComplete = true;
            }

        }

        ShowDiagnostics();

        Debug.Log(" Elapsed Time = " + (Time.realtimeSinceStartup - timeStart).ToString() + " Seconds");
    }

    private void ShowDiagnostics()
    {
        if (showColors)
        {
            ShowColors();

        }
        if (m_graphView != null && showArrows)
        {
            m_graphView.ShowNodeArrows(m_frontierNodes.ToList(), arrowColor);
            if (m_frontierNodes.Contains(m_goalNode))
            {
                m_graphView.ShowNodeArrows(m_pathNodes, highlightColor);
            }
        }
    }

    void ExpandFrontierBreadthSearch(Node node)  // whenver i pass a new node and any new neigbor that has not been encountered before get added to the frontier
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i])
                    && !m_frontierNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbour = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistanceTraveled = distanceToNeighbour + node.distanceTraveled;
                    node.neighbors[i].distanceTraveled = newDistanceTraveled;


                    node.neighbors[i].previous = node;
                    node.neighbors[i].priority = m_exploredNodes.Count;
                    m_frontierNodes.Enqueue(node.neighbors[i]);
                }
            }
        }
    }

    void ExpandFrontierDijkstra(Node node)  
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbour = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistanceTraveled = distanceToNeighbour + node.distanceTraveled; // the distance to neighbour plus the current node distance 

                    if (float.IsPositiveInfinity(node.neighbors[i].distanceTraveled) || newDistanceTraveled < node.neighbors[i].distanceTraveled)
                    {
                      node.neighbors[i].previous = node;
                        node.neighbors[i].distanceTraveled = newDistanceTraveled;

                    }
                    if (!m_frontierNodes.Contains(node.neighbors[i]))
                    {

                        node.neighbors[i].priority = node.neighbors[i].distanceTraveled; 
                    m_frontierNodes.Enqueue(node.neighbors[i]);

                    }
                }
            }
        }
    }

    void ExpandFrontierGreedyBestFirst(Node node)  // whenver i pass a new node and any new neigbor that has not been encountered before get added to the frontier
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i])
                    && !m_frontierNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbour = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistanceTraveled = distanceToNeighbour + node.distanceTraveled;
                    node.neighbors[i].distanceTraveled = newDistanceTraveled;


                    node.neighbors[i].previous = node;

                    if(m_graph != null)
                    {

                    node.neighbors[i].priority = m_graph.GetNodeDistance(node.neighbors[i], m_goalNode);
                    }
                    m_frontierNodes.Enqueue(node.neighbors[i]);
                }
            }
        }
    }

    void ExpandFrontierAStar(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbour = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistanceTraveled = distanceToNeighbour + node.distanceTraveled; // the distance to neighbour plus the current node distance 

                    if (float.IsPositiveInfinity(node.neighbors[i].distanceTraveled) || newDistanceTraveled < node.neighbors[i].distanceTraveled)
                    {
                        node.neighbors[i].previous = node;
                        node.neighbors[i].distanceTraveled = newDistanceTraveled;

                    }
                    if (!m_frontierNodes.Contains(node.neighbors[i]) && m_graph != null)
                    {
                        float distanceToGoal = m_graph.GetNodeDistance(node.neighbors[i], m_goalNode);

                        node.neighbors[i].priority = node.neighbors[i].distanceTraveled 
                            + distanceToGoal;

                        m_frontierNodes.Enqueue(node.neighbors[i]);

                    }
                }
            }
        }
    }

    List<Node> GetPathNodes(Node endnode)
    {

        List<Node> path = new List<Node>();
        if(endnode == null)
        {
            return path;
        }
        path.Add(endnode);

        Node currentNode = endnode.previous;


        while (currentNode != null)
        {
            path.Insert(0, currentNode); // here im inserting the currentnode in the 0 index to always put it in the front 
            currentNode = currentNode.previous;
        }

        return path; // iam just returing the path pof nodes in the end 

    }

}
