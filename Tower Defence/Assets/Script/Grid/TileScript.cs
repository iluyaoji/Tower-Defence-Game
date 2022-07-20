using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class TileScript : MonoBehaviour
{
    //label组件
    public TextMeshPro label;
    public Color labelColor = Color.white;
    //
    private Vector2Int coordinates = new Vector2Int();
    /*
    void Awake()
    {
        //显示坐标到label
        DisplayCoordinates();
    }
    */
    void Update()
    {
        //开始游戏后更新坐标
        if (Application.isPlaying)
        {
            //DisplayCoordinates();
            UpudateObjectName();
        }
        
    }
    public void DisplayCoordinates()
    {
        //物体位置除以网格大小得到整数坐标
        coordinates.x = Mathf.RoundToInt(transform.position.x / 10);
        coordinates.y = Mathf.RoundToInt(transform.position.z / 10);
        label.text = coordinates.x + "," + coordinates.y;
        label.color = labelColor;
        GetComponent<WayPoint>().node.coordiates = coordinates;
    }

    void UpudateObjectName()
    {
        this.gameObject.name = coordinates.ToString();
        
    }
}
