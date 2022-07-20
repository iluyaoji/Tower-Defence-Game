using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    public int PosSize;
    public bool useArcing;
    public bool useSine;
    public bool useWiggle;
    public bool X, Y, Z;
    [Range(-1,1)]
    public float _ArcingPowParam1;
    [Range(0,5)]
    public float _SineScaleX;
    public float _SineScaleY;
    public float _CenterOffset;
    public float _Adjust;
    public float _RandomSize;
    public float _Speed;

    LineRenderer lineRenderer;
    List<Vector3> posList = new List<Vector3>();
    [System.NonSerialized] public Vector3 startPos;
    [System.NonSerialized] public Vector3 endPos;
    float fps;
    float sineRandom;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        startPos = lineRenderer.GetPosition(0);
        endPos = lineRenderer.GetPosition(1);
        LerpPos();
    }

    void Update()
    {
        fps += Time.deltaTime*_Speed;
        if (fps >= 1)
        {
            sineRandom = (float)Random.Range(0, posList.Count * 10) / 10;
            fps = 0;
        }

        List<Vector3> list = new List<Vector3>();
        for (int i = 0; i < posList.Count; i++)
        {
            Vector3 point = posList[i];
            if (useArcing)
            {
                Vector3 v = new Vector3();
                if (X) v.x = Arcing(i);
                if (Y) v.y = Arcing(i);
                if (Z) v.z = Arcing(i);
                point = Vector3.Lerp(posList[i], posList[i] + v, fps);
            }
            if (useSine && i != 0 && i != posList.Count-1)
            {
                Vector3 v = new Vector3();
                if (X) v.x = Sine(i + sineRandom);
                if (Y) v.y = Sine(i + sineRandom);
                if (Z) v.z = Sine(i + sineRandom);
                
                point += v;
            }
            if (useWiggle && i != 0 && i != posList.Count - 1)
                point += Wiggle(i);
            list.Add(point);
        }
        SetLinePosition(list);
    }

    void SetLinePosition(List<Vector3> listPoint)
    {
        lineRenderer.positionCount = listPoint.Count;
        for(int i = 0; i < listPoint.Count; i++)
        {
            lineRenderer.SetPosition(i, listPoint[i]);
        }
    }

    public void LerpPos()
    {
        int index = PosSize + 2;
        for(int i=0; i < index; i++)
        {
            if (posList.Count < index)
                posList.Add(Vector3.Lerp(startPos, endPos, (float)i / index));
            else posList[i] = Vector3.Lerp(startPos, endPos, (float)i / index);
        }
    }

    float Arcing(float param)
    {
        return _ArcingPowParam1 * Mathf.Pow((param - (float)posList.Count / 2 + _CenterOffset) * _ArcingPowParam1,2) + _Adjust;
    }


    float Sine(float param)
    {
        return Mathf.Sin((float)param / posList.Count * 2 * 3.14f * _SineScaleX) * _SineScaleY;
    }

    Vector3 Wiggle(int listIndex)
    {
        Vector3 v = new Vector3();
        if (X) v.x = Random.Range(0, 10) * _RandomSize;
        if (Y) v.y = Random.Range(0, 10) * _RandomSize;
        if (Z) v.z = Random.Range(0, 10) * _RandomSize;
        return v;
    }
}
