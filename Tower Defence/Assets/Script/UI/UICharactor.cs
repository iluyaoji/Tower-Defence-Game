using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharactor : MonoBehaviour
{
    public bool isHarm;
    public float lifeTime = 10;
    public float offset = 0.1f;
    //该UI的初始化在该物体的位置
    [System.NonSerialized] public Transform instanceParentPos;
    //UI池
    [System.NonSerialized] public Transform UIRoot;

    float _lifeTime = 0;
    public void ResetPos()
    {
        screenPos = FindObjectOfType<Camera>().WorldToScreenPoint(instanceParentPos.position);
    }
    void Start()
    {
        transform.SetParent(UIRoot.transform);
    }

    void Update()
    {

        _lifeTime += Time.deltaTime;
        //如果是伤害数字则运动
        if(isHarm)
            Action();
        ShowAt();
    }

    public void SetValue(float per)
    {
        GetComponent<Slider>().value = per;
    }
    private Vector3 screenPos;
    /// <summary>
    /// 显示在父物体世界坐标在屏幕坐标的位置
    /// </summary>
    public void ShowAt()
    {
        
        if (isHarm && Time.timeScale != 0)
        {
            if(_lifeTime < lifeTime)
                screenPos += Vector3.up*offset;
        }
        else screenPos = FindObjectOfType<Camera>().WorldToScreenPoint(instanceParentPos.position);
        this.transform.position = screenPos;
    }

    void Action()
    {
        if (_lifeTime >= lifeTime)
        {
            _lifeTime = 0;
            gameObject.SetActive(false);
        }
        if (_lifeTime < lifeTime)
            transform.localScale = Vector3.Lerp(Vector3.one * 3, Vector3.one, _lifeTime * 2 / lifeTime);
    }
}
