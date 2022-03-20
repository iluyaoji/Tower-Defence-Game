// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/ToonShading"
{
    Properties
    {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Ramp ("Ramp Texture", 2D) = "white" {}
        _Outline ("Outline", Range(0, 1)) = 0.1
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _Specular ("Specular", Color) = (1, 1, 1, 1)
        _SpecularScale ("Specular Scale", Range(0, 0.1)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            NAME "OUTLINE"
            Cull Front

            CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

            float _Outline;
            float4 _OutlineColor;

            struct v2f {
                float4 pos : POSITION;
            };

            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            v2f vert(a2v v) {
                v2f o;

                float4 pos = mul(UNITY_MATRIX_MV, v.vertex);
                float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                pos = pos + float4 (normalize(normal), 0) * _Outline;
                o.pos = mul(UNITY_MATRIX_P, pos);

                return o;
            }

            float4 frag(v2f i) : SV_Target {
                return float4(_OutlineColor.rgb, 1);
            }

            ENDCG
        }

        Pass{
            Tags {"LightMode" = "ForwardBase"}

            Cull Back

            CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

            float4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Ramp;
            float4 _Ramp_ST;
            
            float4 _Specular;
            float _SpecularScale;

#pragma multi_compile_fwdbase

            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                SHADOW_COORDS(3)
            };

            v2f vert(a2v v) {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject) ;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                TRANSFER_SHADOW(o);

                return o;
            }

            float4 frag(v2f i) : SV_Target{
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 worldHalfDir = normalize(worldLightDir + worldViewDir);

                fixed3 c = tex2D(_MainTex, i.uv);
                fixed3 albedo = c.rgb * _Color.rgb;

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                fixed diff = dot(worldNormal, worldLightDir);
                diff = (diff * 0.5 + 0.5) * atten;

                fixed3 diffuse = _LightColor0.rgb * albedo * tex2D(_Ramp, float2(diff, diff)).rgb;
                fixed spec = dot(worldNormal, worldHalfDir);
                fixed w = fwidth(spec) * 2.0;
                fixed3 specular = 
                    _Specular.rgb * lerp(0, 1, smoothstep(-w, w, spec + _SpecularScale - 1)) * step(0.0001, _SpecularScale);
                
                return fixed4(ambient + diffuse + specular, 1.0);

            }
            ENDCG
        }
    }
    //Fallback "Diffuse"
}
