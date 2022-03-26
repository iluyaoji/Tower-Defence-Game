using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerScript : MonoBehaviour
{
    private float initialMousePosX;
    private float initialMousePosY;

    private float initialRotX;
    private float initialRotY;

    public Transform camT;

    public Transform thisT;

    public float minRotateAngle = 10;
    public float maxRotateAngle = 89;

    public static CamerScript instance;
    public static void Disable() { if (instance != null) instance.enabled = false; }
    public static void Enable() { if (instance != null) instance.enabled = true; }

    void Awake()
    {
        thisT = transform;

        instance = this;
        camT = Camera.main.transform;
    }

    void Start()
    {
        minRotateAngle = Mathf.Max(10, minRotateAngle);
        maxRotateAngle = Mathf.Min(89, maxRotateAngle);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            initialMousePosX = Input.mousePosition.x;
            initialMousePosY = Input.mousePosition.y;

            initialRotX = thisT.eulerAngles.y;
            initialRotY = thisT.eulerAngles.x;
        }

        if (Input.GetMouseButton(1))
        {
            //当前鼠标位置和右击时候的坐标差
            float deltaX = Input.mousePosition.x - initialMousePosX;
            float deltaY = Input.mousePosition.y - initialMousePosY;

            float deltaRotX = (.1f * (initialRotX / Screen.width));
            float deltaRotY = -(.1f * (initialRotY / Screen.height));

            float rotX = deltaX + deltaRotX;
            float rotY = -deltaY + deltaRotY;
            float y = rotY + initialRotY;

            if (y > maxRotateAngle)
            {
                initialRotY -= (rotY + initialRotY) - maxRotateAngle;
                y = maxRotateAngle;
            }
            else if (y < minRotateAngle)
            {
                initialRotY += minRotateAngle - (rotY + initialRotY);
                y = minRotateAngle;
            }

            thisT.rotation = Quaternion.Euler(y, rotX + initialRotX, 0);
        }
        
    }
}
