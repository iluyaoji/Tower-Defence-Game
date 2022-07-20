Shader "MyShader/DecalShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Geometry+1" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 viewPos : TEXCOORD1;
                float4 screenUV : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _CameraDepthTexture;
            fixed4 _Color;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenUV = ComputeScreenPos(o.vertex);
                o.viewPos = UnityObjectToViewPos(v.vertex).xyz * float3(-1,-1,1);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.viewPos = i.viewPos * (_ProjectionParams.z / i.viewPos.z);
                float2 uv = i.screenUV.xy / i.screenUV.w;

                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);

                // 要转换成线性的深度值 //
                depth = Linear01Depth(depth);

                float4 vpos = float4(i.viewPos * depth,1);
                float3 wpos = mul(unity_CameraToWorld, vpos).xyz;
                float3 opos = mul(unity_WorldToObject, float4(wpos,1)).xyz;
                
                clip(float3(0.5,0.5,0.5) - abs(opos.xyz));

                // 转换到 [0,1] 区间 //
                float2 texUV = opos.xz + 0.5;

                float4 col = tex2D(_MainTex, texUV);
                return col * _Color;
            }
            ENDCG
        }
    }
        Fallback Off
}
