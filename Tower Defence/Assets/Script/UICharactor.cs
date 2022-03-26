using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharactor : MonoBehaviour
{
    public Canvas canvas;
    public int UIRootIndex;


    private void Start()
    {
        canvas = FindObjectOfType<Canvas>();
        Transform UIRoot = canvas.transform.GetChild(UIRootIndex);
        transform.SetParent(UIRoot.transform);
    }
    public void SetValue(float per)
    {
        GetComponent<Slider>().value = per;
    }
    public void SetUnick(string unick)
    {

    }

    public void ShowAt(Vector3 pos)
    {
        this.transform.position = pos;
    }


}
