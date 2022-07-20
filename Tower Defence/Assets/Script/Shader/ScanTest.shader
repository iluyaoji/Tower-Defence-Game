Shader "MyShader/ScanTest"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white"{}
        [HDR]_LineColor("Color", Color) = (1,1,1,1)
        _LineSize("LineSize",float) = 1
        _Speed("Speed",float) = 1
        _FresnelWidth("FresnelWidth",float) = 1
    }
        SubShader
        {

            Pass
            {
                Name "SCANTEST"
                Tags { "Queue" = "Transparent" }
                Blend One One
                ZTest Greater
                ZWrite Off

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"
                #include "Lighting.cginc"

                float _LineSize;
                fixed4 _LineColor;
                float _Speed;
                float _FresnelWidth;
                sampler2D _MainTex;
                float4 _MainTex_ST;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;

                    float3 normal : NORMAL;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
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

                    //worldPos是没有裁剪和投影的 值不是0到1 所以当LineSize为1时 线的大小刚好一个坐标单位
                    fixed line_y = frac(i.worldPos.y * _LineSize + _Time.y * _Speed);
                    return fresnel * _LineColor * line_y;
                }
                ENDCG
            }


        }
}
