using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCameraDepth : PostEffectsBase
{ 
    private void OnEnable()
    {
        FindObjectOfType<Camera>().GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
}
