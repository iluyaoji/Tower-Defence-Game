Shader "Unlit/ScanShader"
{
    Properties
    {
        _ScanPer("ScanPer",Range(0,1)) = 0
        _MainTex("MainTex", 2D) = "white"{}
        _NoiseTex("NoiseTex", 2D) = "white"{}
        [HDR]_LineColor("Color", Color) = (1,1,1,1)
        _LineSize("LineSize",float) = 1
        _Speed("Speed",float) = 1
        _FresnelWidth("FresnelWidth",float) = 1
    }
    SubShader
    {
        //Tags { "Queue"="Transparent" }
        Blend One One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            float4 _Color;
            float _LineSize;
            fixed4 _LineColor;
            float _Speed;
            float _FresnelWidth;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            float _ScanPer;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uvNoise : TEXCOORD3;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float4 vertex : SV_POSITION;

                float3 objectPos : TEXCOORD4;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvNoise = TRANSFORM_TEX(v.uv, _NoiseTex);

                o.objectPos = v.vertex.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 V = normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 N = normalize(i.worldNormal);
                fixed VdotN = dot(V, N);

                fixed3 L = normalize(UnityWorldSpaceLightDir(i.worldPos));

                fixed fresnel = 0.2 + 2 * pow(1 - VdotN, _FresnelWidth);
                
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 noi = tex2D(_NoiseTex, i.uvNoise);

                fixed4 ambient = fixed4(UNITY_LIGHTMODEL_AMBIENT.xyz * col,1);
                col = fixed4(_LightColor0.rgb * col * max(0, dot(N, L)),1);

                //worldPos是没有裁剪和投影的 值不是0到1 所以当LineSize为1时 线的大小刚好一个坐标单位
                fixed line_dir = frac((i.worldPos.y +noi.r) * _LineSize+_Time.y*_Speed);
                float scan = step(saturate(i.objectPos.y + 0.5 + noi.r * 0.1), _ScanPer + 0.01);
                //return col+ ambient;
                return lerp(fresnel* _LineColor* line_dir, col + ambient, scan);
                
                //return col*fresnel * _LineColor * line_dir;
            }
            ENDCG
        }


    }
}
