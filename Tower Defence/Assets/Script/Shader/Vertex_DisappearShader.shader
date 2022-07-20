// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "MyShader/Vertex_DisappearShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _NoiseMap("Noise Map", 2D) = "white" {}
        [HDR]_Ambient("Ambient", Color) = (1,1,1,1)
        _Diffuse("Diffuse", Color) = (1,1,1,1)
        [HDR]_DisappearColor("Disappear Color", Color) = (1,1,1,1)
        _Pow("Offset Pow", Range(0,0.2)) = 0
        _LineSize("Line Size", float) = 0
        _stepY("StepY", float) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
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
                    float4 vertex : SV_POSITION;
                    float3 worldPos : TEXCOORD3;

                    float3 color : COLOR;

                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                sampler2D _NoiseMap;
                float4 _NoiseMap_ST;
                fixed4 _Diffuse;
                fixed4 _Ambient;
                float _Pow;
                float _stepY;
                float _LineSize;
                fixed4 _DisappearColor;

                v2f vert(appdata v)
                {
                    v2f o;
                    fixed3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                    v.vertex.xyz += v.normal * _Pow * smoothstep(_stepY - 0.1, _stepY + 0.1, worldPos.y);


                    o.vertex = UnityObjectToClipPos(v.vertex);

                    o.uv.xy = v.uv.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                    o.uv.zw = v.uv.xy * _NoiseMap_ST.xy + _NoiseMap_ST.zw;
                    o.uv.w += _Time.y * 0.1;

                    o.worldPos = worldPos;

                    fixed3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                    fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
                    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * _Ambient.rgb;
                    fixed3 diffuse =  _LightColor0.rgb * _Diffuse.rgb * saturate(dot(worldNormal, worldLight));
                    o.color = ambient + diffuse;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv.xy);
                    fixed4 noi = tex2D(_NoiseMap, i.uv.zw);

                    float v = smoothstep(_stepY + 0.1, _stepY - 0.3, i.worldPos.y);
                    float v1 = smoothstep(_stepY + 0.1, _stepY - 0.3, i.worldPos.y - _LineSize*noi.r);
                    clip(v1 - 0.01);
                    clip(noi.r - 0.5 + v);
                    col = col * fixed4(i.color,1);
                    col = lerp(_DisappearColor,col, saturate(v * 2));

                    return col;
                }
                ENDCG
            }
        }
}
