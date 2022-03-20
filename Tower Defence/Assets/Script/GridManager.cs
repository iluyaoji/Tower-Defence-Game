using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Transform Ground;
    public Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();


    public Node GetNode(Vector2Int croodinates)
    {
        if (grid.ContainsKey(croodinates))
        {
            return grid[croodinates];
        }
        return null;
    }

    public void CreateGrid()
    {
        for(int i = 0; i < Ground.childCount; i++)
        {
            Node node = Ground.GetChild(i).GetComponent<WayPoint>().node;
            grid.Add(node.coordiates, node);
        }
    }

}
