using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public bool isConstructable;

    [SerializeField] public Node node;

    private void Start()
    {
        if (node.isWalkable)
            gameObject.isStatic = true;
    }

    void Update()
    {
        if (isConstructable)
            transform.GetChild(0).transform.localPosition = Vector3.zero;
        else
        {
            if(!node.isWalkable)
                transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
            transform.GetChild(0).transform.localPosition = -Vector3.up;
        }
    }

}
