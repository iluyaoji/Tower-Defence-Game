// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "MyShader/Dissolve"
{
    Properties
    {
        _Ambient("Ambient",Color) = (1,1,1,1)
        _BurnAmount("Burn Amount", Range(0.0, 1.0)) = 0.0
        _LineWidth("Burn Line Width",Range(0.0, 0.2)) = 0.1
        _BumpMap("Bormal Map",2D) = "bump"{}
        _MainTex ("Texture", 2D) = "white" {}
        _BurnFristColor("Burn First Color",Color) = (1,0,0,1)
        _BurnSecondColor("Burn Second Color",Color) = (1,0,0,1)
        _BurnMap("Burn Map",2D) = "white"{}
    }
    SubShader
    {

        Pass
        {
            Tags{"LightMode" = "ForwardBase"}
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uvMainTex : TEXCOORD0;
                float2 uvBumpMap : TEXCOORD1;
                float2 uvBurnMap : TEXCOORD2;
                float3 lightDir : TEXCOORD3;
                float3 worldPos : TEXCOORD4;
                SHADOW_COORDS(5)
            };

            fixed4 _Ambient;
            float _BurnAmount;
            float _LineWidth;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BurnFristColor;
            float4 _BurnSecondColor;
            sampler2D _BurnMap;
            float4 _BurnMap_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uvMainTex = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvBumpMap = TRANSFORM_TEX(v.uv, _BumpMap);
                o.uvBurnMap = TRANSFORM_TEX(v.uv, _BurnMap);
                //使用该宏时 a2v一定要先定义float3 normal : NORMAL 和 float4 tangent : TANGENT;
                TANGENT_SPACE_ROTATION;
                o.lightDir = mul(rotation,ObjSpaceLightDir(v.vertex)).xyz;

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //获取噪音图
                fixed3 burn = tex2D(_BurnMap, i.uvBurnMap).rgb;
                //只取高于_BurnAmount的值
                clip(burn.r - _BurnAmount);
                float3 trangentLightDir = normalize(i.lightDir);
                fixed3 trangentNormal = UnpackNormal(tex2D(_BumpMap, i.uvBumpMap));
                //漫反射颜色
                fixed3 albedo = tex2D(_MainTex, i.uvMainTex).rgb;
                //环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo * _Ambient.xyz;
                fixed3 diffuse = _Ambient.xyz * _LightColor0.rgb * albedo * max(0, dot(trangentNormal, trangentLightDir));
                
                fixed t = 1 - smoothstep(0.0, _LineWidth, burn.r - _BurnAmount);
                fixed3 burnColor = lerp(_BurnFristColor, _BurnSecondColor, t);
                burnColor = pow(burnColor, 5);

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                // t = 0.8 ~ 1
                //step(a,x) = if(x<a) return 0 else return 1
                fixed3 finalColor = lerp(ambient + diffuse * atten, burnColor, t * step(0.0001, _BurnAmount));

                return fixed4(finalColor,1);
            }
            ENDCG
        }

        Pass{
                Tags{"LightMode" = "ShadowCaster"}

                CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_shadowcaster
                #include "UnityCG.cginc"

                float _BurnAmount;
                sampler2D _BurnMap;
                float4 _BurnMap_ST;

                struct v2f {
                    V2F_SHADOW_CASTER;
                    float2 uvBurnMap : TEXCOORD1;
                };

                v2f vert(appdata_base v) {
                    v2f o;
                    TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                    o.uvBurnMap = TRANSFORM_TEX(v.texcoord, _BurnMap);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target{
                    fixed3 burn = tex2D(_BurnMap,i.uvBurnMap).rgb;
                    clip(burn.r - _BurnAmount);
                    SHADOW_CASTER_FRAGMENT(i)
                }
                ENDCG
        }
    }
}
