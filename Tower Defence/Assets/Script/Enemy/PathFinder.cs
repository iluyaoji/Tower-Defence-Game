using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public WayPoint start;
    public WayPoint end;

    private Vector2Int _start;
    private Vector2Int _end;

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

    public void Initial()
    {
        start.GetComponent<TileScript>().DisplayCoordinates();
        end.GetComponent<TileScript>().DisplayCoordinates();
        _start = start.node.coordiates;
        _end = end.node.coordiates;
        gridManager = this.gameObject.GetComponent<GridManager>();
        gridManager.Ground = LevelManager.Instance.Ground;
        gridManager.CreateGrid();
        if(gridManager != null)
        {
            grid = gridManager.grid;
            
            startNode = gridManager.GetNode(_start);

            endNode = gridManager.GetNode(_end);

        }
        BreadFirstSearch();
    }


    private void ExploreNeighbors()
    {
        List<Node> neighbors = new List<Node>();
        //暂存邻居
        foreach(Vector2Int direction in directions)
        {
            //找到当前检查的节点的邻居
            Vector2Int neighborCoords = CurrentSearchNode.coordiates + direction;
            if (grid.ContainsKey(neighborCoords))
            {
                neighbors.Add(grid[neighborCoords]);

            }
        }
        //遍历邻居 将可行的邻居提取
        foreach(Node neighbor in neighbors)
        {
            if (!reached.ContainsKey(neighbor.coordiates) && neighbor.isWalkable)
            {
                //Debug.Log(CurrentSearchNode.coordiates);
                //节点相链接
                neighbor.ConnectedTo = CurrentSearchNode;
                //存入已经搜寻到的路径中
                reached.Add(neighbor.coordiates, neighbor); 
                //将该邻居作为下次检查的节点之一
                frontier.Enqueue(neighbor);
            }
        }
    }
    //遍历分支
    void BreadFirstSearch()
    {
        bool isRunning = true;

        frontier.Enqueue(startNode);
        if(!reached.ContainsKey(_start))
            reached.Add(_start, startNode);

        while(frontier.Count > 0 && isRunning)
        {
            CurrentSearchNode = frontier.Dequeue();

            ExploreNeighbors();
            if (CurrentSearchNode.coordiates == _end)
            {
                
                isRunning = false;
            }
            if(isRunning && frontier.Count <= 0)
            {
                //说明路径断开 检查全图是否存在不在已寻路径中且ConnectedTo不为空的可行路径
                foreach(Node n in gridManager.grid.Values)
                {
                    if(!reached.ContainsKey(n.coordiates) && n.ConnectedTo != null)
                    {
                        frontier.Enqueue(n);
                    }
                }
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
        //currentNode.isPath = true;

        while(currentNode.ConnectedTo != null)
        {
            currentNode = currentNode.ConnectedTo;
            path.Add(currentNode);
            //currentNode.isPath = true;
        }

        path.Reverse();

        return path;
    }
}
