// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/EdgDetectNormalAndDepth"
{
    Properties
    {
        _EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
        _SampleDistance ("Sample Distance", Float) = 1.0
        _NormalSensitivity ("_NormalSensitivity",Range(0,2)) = 1
        _DepthSensitivity("_DepthSensitivity",Range(0,2)) = 1
    }
    SubShader
    {
        Tags{
            "Queue" = "Geometry+550"
            "RenderType"="Opaque"
        }
        CGINCLUDE

#include "UnityCG.cginc"

        fixed4 _EdgeColor;
        float _SampleDistance;
        float _NormalSensitivity;
        float _DepthSensitivity;
        sampler2D _CameraDepthNormalsTexture;

        struct v2f {
            float4 pos : SV_POSITION;
            float4 uv[5] : TEXCOORD0;
        };
        //uv要改成发float4是因为拿到挂载该材质的物体的顶点坐标，需要变换才能做纹理采样；之前书上是摄像机的视角矩形，纹理坐标可以直接拿
        v2f vert(appdata_img v) {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);

            float4 uv = ComputeScreenPos(o.pos);
            //ComputeScreenPos返回的值是齐次坐标系下的屏幕坐标值，其范围为[0, w]
            o.uv[0] = uv;

            float2 size = float2(1.0 / 1920.0, 1.0 / 1920.0);

            o.uv[1] = float4((uv.xy + size * fixed2(1, 1) * _SampleDistance), uv.zw);
            o.uv[2] = float4((uv.xy + size * fixed2(-1, -1) * _SampleDistance), uv.zw);
            o.uv[3] = float4((uv.xy + size * fixed2(-1, 1) * _SampleDistance), uv.zw);
            o.uv[4] = float4((uv.xy + size * fixed2(1, -1) * _SampleDistance), uv.zw);

            return o;
        }

        half CheckSame(half4 center, half4 sample) {
            half2 centerNormal = center.xy;
            float centerDepth = DecodeFloatRG(center.zw);
            half2 sampleNormal = sample.xy;
            float sampleDepth = DecodeFloatRG(sample.zw);

            half2 diffNormal = abs(centerNormal - sampleNormal) * _NormalSensitivity;
            int isSameNormal = (diffNormal.x + diffNormal.y) < 0.1;

            float diffDepth = abs(centerDepth - sampleDepth) * _DepthSensitivity;
            int isSameDepth = diffDepth < 0.1 * centerDepth;

            return isSameNormal* isSameDepth ? 1.0 : 0.0;
        }

        fixed4 fragRobertsCrossDepthAndNormal(v2f i) : SV_Target{
            //tex2Dproj 和 tex2D 的区别是在内部做齐次除法操作
            //这里uv由于范围为[0, w]所以要做齐次除法
            float4 sample1 = tex2Dproj(_CameraDepthNormalsTexture, i.uv[1]);
            float4 sample2 = tex2Dproj(_CameraDepthNormalsTexture, i.uv[2]);
            float4 sample3 = tex2Dproj(_CameraDepthNormalsTexture, i.uv[3]);
            float4 sample4 = tex2Dproj(_CameraDepthNormalsTexture, i.uv[4]);

            half edge = 1.0;
            edge *= CheckSame(sample1, sample2);
            edge *= CheckSame(sample3, sample4);

            if (edge < 0.5)
                return _EdgeColor;
            else
                discard;
            return 0;

        }

        ENDCG

        Pass{
            ZTest Always Zwrite Off
            CGPROGRAM
            
#pragma vertex vert
#pragma fragment fragRobertsCrossDepthAndNormal

            ENDCG
        }
       
    }
    Fallback off
}
