using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgDetectNormalsAndDepth : PostEffectsBase
{
    public Shader edgeDetechShader;
    private Material edgeDetechMaterial = null;
    public Material material
    {
        get
        {
            edgeDetechMaterial = CheckShaderAndCreateMaterial(edgeDetechShader, edgeDetechMaterial);
            return edgeDetechMaterial;
        }
    }

    [Range(0.0f, 1.0f)]
    public float edgesOnly = 0.0f;
    public Color edgeColor = Color.black;
    public Color backgraundColor = Color.white;
    public float sampleDistance = 1.0f;
    public float sensitivityDepth = 1.0f;
    public float sensitivityNormals = 1.0f;

    private void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
    }


    [ImageEffectOpaque]


    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(material != null)
        {
            material.SetFloat("_EdgeOnly", edgesOnly);
            material.SetColor("_EdgeColor", edgeColor);
            material.SetColor("_BackgroundColor", backgraundColor);
            material.SetVector("_Sensitivity", new Vector4(sensitivityNormals, sensitivityDepth, 0.0f, 0.0f));
            Graphics.Blit(src, dest, material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
