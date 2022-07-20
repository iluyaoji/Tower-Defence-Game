Shader "MyShader/ScanShader"
{
    Properties
    {
        _ScanPos("ScanPos",float) = 0
        _MainTex("MainTex", 2D) = "white"{}
        _NoiseTex("NoiseTex", 2D) = "white"{}
        [HDR]_LineColor("Color", Color) = (1,1,1,1)
        _LineSize("LineSize",float) = 1
        _Speed("Speed",float) = 1
        _FresnelWidth("FresnelWidth",float) = 1
    }
    SubShader
    {
        

        Pass
        {
            Tags { "Queue" = "Transparent" }
            Blend One One

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

            float _ScanPos;

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
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvNoise = TRANSFORM_TEX(v.uv, _NoiseTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 V = normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 N = normalize(i.worldNormal);
                fixed VdotN = dot(V, N);

                fixed3 L = normalize(UnityWorldSpaceLightDir(i.worldPos));

                fixed fresnel = 0.2 + 2 * pow(1 - VdotN, _FresnelWidth);
                
                fixed4 noi = tex2D(_NoiseTex, i.uvNoise);


                fixed line_dir = frac((i.worldPos.y +noi.r) * _LineSize+_Time.y*_Speed);
                
                return fresnel* _LineColor* line_dir;
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            float4 _Color;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            float _ScanPos;

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
            };


            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvNoise = TRANSFORM_TEX(v.uv, _NoiseTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed3 N = normalize(i.worldNormal);
                fixed3 L = normalize(UnityWorldSpaceLightDir(i.worldPos));


                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 noi = tex2D(_NoiseTex, i.uvNoise);

                fixed4 ambient = fixed4(UNITY_LIGHTMODEL_AMBIENT.xyz * col,1);
                col = fixed4(_LightColor0.rgb * col * max(0, dot(N, L)),1);

                clip(step(i.worldPos.y + noi.r , _ScanPos)-0.1);

                return col + ambient;
            }
            ENDCG
        }
    }
}
