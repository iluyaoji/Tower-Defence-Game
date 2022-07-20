using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Vector2Int coordiates;
    public bool isWalkable;
    //public bool isExploerd;
    //public bool isPath;

    public Node ConnectedTo;

    public Node(Vector2Int coordinates, bool isWalkable)
    {
        this.coordiates = coordinates;
        this.isWalkable = isWalkable;
    }
    public Node()
    {
    }
}
