// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "MyShader/DisappearShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _NoiseMap("Noise Map", 2D) = "white" {}
        _Diffuse ("Diffuse", Color) = (1,1,1,1)
        _Specular("Specular" , Color) = (1,1,1,1)
        _Gloss("Gloss", Range(8.0,256))  = 20
        [HDR]_DisappearColor("Disappear Color", Color) = (1,1,1,1)
        _Pow("Pow", Range(0,0.2)) = 0
        _stepY("StepY", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            //Tags{"LightMode" = "ForwardBase"}
            Cull Off
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                float3 normal : NORMAL;
                //BumpMap
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float2 noiUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD3;
                
                float3 lightDir : TEXCOORD4;
                float3 viewDir : TEXCOORD2;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            sampler2D _NoiseMap;
            float4 _NoiseMap_ST;
            fixed4 _Diffuse;
            fixed4 _Specular;
            float _Gloss;
            float _Pow;
            float _stepY;
            fixed4 _DisappearColor;

            v2f vert (appdata v)
            {
                v2f o;
                fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                v.vertex.xyz += v.normal * _Pow * smoothstep(_stepY-0.1, _stepY + 0.1, worldPos.y);
                

                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv.xy = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.uv.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;
                o.noiUV = v.uv.xy * _NoiseMap_ST.xy + _NoiseMap_ST.zw;
                o.noiUV.y += _Time.y*0.1;

                o.worldPos = worldPos;

                TANGENT_SPACE_ROTATION;

                o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
                o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex)).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv.xy);
                fixed4 noi = tex2D(_NoiseMap, i.noiUV);

                float v = smoothstep(_stepY + 0.1, _stepY-0.3, i.worldPos.y);
                clip(v-0.01);
                clip(noi.r-0.5+v);
                col = lerp(_DisappearColor,col, saturate(v*2));

                fixed3 tangentLightDir = normalize(i.lightDir);
                fixed3 tangentViewDir = normalize(i.viewDir);
                fixed4 packedNormal = tex2D(_BumpMap, i.uv.zw);
                fixed3 tangentNormal = UnpackNormal(packedNormal);

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * col;

                fixed3 diffuse = col * _LightColor0.rgb * _Diffuse.rgb * saturate(dot(tangentNormal, tangentLightDir));
                
                fixed3 halfDir = normalize(tangentLightDir + tangentViewDir);
                fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(tangentNormal, halfDir)), _Gloss);
                return fixed4(ambient + diffuse + specular, 1.0);
            }
            ENDCG
        }
    }
}
