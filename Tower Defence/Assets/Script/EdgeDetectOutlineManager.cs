using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class EdgeDetectOutlineManager : MonoBehaviour
{
    public Vector3 drawPositionOffset;
    public Material mat;
    private Camera _camera;
    private EdgeDetectOutlineInfo[] _edgeDetectOutlineInfos;

    void Awake()
    {
        _camera = GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.DepthNormals;
        _edgeDetectOutlineInfos = FindObjectsOfType<EdgeDetectOutlineInfo>();
    }
    void Update()
    {
        if (!Application.isPlaying)
        {
            _edgeDetectOutlineInfos = FindObjectsOfType<EdgeDetectOutlineInfo>();
        }

        foreach(var edgeDetectOutlineInfo in _edgeDetectOutlineInfos)
        {
            if(edgeDetectOutlineInfo != null && edgeDetectOutlineInfo.Enable)
            {
                DrawObject(edgeDetectOutlineInfo.meshFilter, edgeDetectOutlineInfo.block);
            }
        }
    }

    void DrawObject(MeshFilter meshFilter, MaterialPropertyBlock block)
    {
        Matrix4x4 drawMatrix = meshFilter.transform.localToWorldMatrix;

        drawMatrix.m03 += drawPositionOffset.x;
        drawMatrix.m13 += drawPositionOffset.y;
        drawMatrix.m23 += drawPositionOffset.z;
        
        Graphics.DrawMesh(meshFilter.sharedMesh,drawMatrix,mat,0,null,0,block);
    }
}
