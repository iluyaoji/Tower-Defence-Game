using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public Vector2Int start;
    public Vector2Int end;

    Node startNode;
    Node endNode;
    Node currentNode;
    //使用队列实现广度遍历
    Queue<Node> frontier = new Queue<Node>();
    Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>();

    Node CurrentSearchNode;
    Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

    GridManager gridManager;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        gridManager.CreateGrid();
        if(gridManager != null)
        {
            grid = gridManager.grid;
            
            startNode = gridManager.GetNode(start);

            endNode = gridManager.GetNode(end);

        }
        BreadFirstSearch();
    }


    private void ExploreNeighbors()
    {
        List<Node> neighbors = new List<Node>();
        foreach(Vector2Int direction in directions)
        {
            
            Vector2Int neighborCoords = CurrentSearchNode.coordiates + direction;
            if (grid.ContainsKey(neighborCoords))
            {
                neighbors.Add(grid[neighborCoords]);

            }
        }

        foreach(Node neighbor in neighbors)
        {
            if (!reached.ContainsKey(neighbor.coordiates) && neighbor.isWalkable)
            {
                neighbor.ConnectedTo = CurrentSearchNode;
                reached.Add(neighbor.coordiates, neighbor); 
                frontier.Enqueue(neighbor);
            }
        }
    }

    void BreadFirstSearch()
    {
        bool isRunning = true;

        
        frontier.Enqueue(startNode);
        reached.Add(start, startNode);

        while(frontier.Count > 0 && isRunning)
        {
            CurrentSearchNode = frontier.Dequeue();
            ExploreNeighbors();
            if(CurrentSearchNode.coordiates == end)
            {
                isRunning = false;
            }
        }
    }

    private List<Node> path;
    public List<Node> BuildPath()
    {
        if(path != null)
        {
            return path;
        }
        path = new List<Node>();
        currentNode = endNode;
        path.Add(currentNode);
        currentNode.isPath = true;

        while(currentNode.ConnectedTo != null)
        {
            currentNode = currentNode.ConnectedTo;
            path.Add(currentNode);
            currentNode.isPath = true;
        }

        path.Reverse();

        return path;
    }
}
