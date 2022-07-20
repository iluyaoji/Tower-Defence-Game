using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [System.NonSerialized] public Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    [System.NonSerialized] public Transform Ground;

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
        //Debug.Log(grid.Count);
        if (grid.Count > 0) return;
        for (int i = 0; i < Ground.childCount; i++)
        {
            Ground.GetChild(i).GetComponent<TileScript>().DisplayCoordinates();
            Node node = new Node();
            node.coordiates =  Ground.GetChild(i).GetComponent<WayPoint>().node.coordiates;
            node.isWalkable = Ground.GetChild(i).GetComponent<WayPoint>().node.isWalkable;
            grid.Add(node.coordiates, node);
        }
        
    }

}
