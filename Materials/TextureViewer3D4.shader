Shader "Unlit/TextureViewer3D4"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            Texture3D<float4> DisplayTexture;
            SamplerState samplerDisplayTexture;
            float sliceDepth;
            float surfaceLevel;
            float4 layersToShow;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 uv3 = float3(i.uv.xy, sliceDepth);
                float4 val = DisplayTexture.SampleLevel(samplerDisplayTexture, uv3, 0);
                float4 v = val * layersToShow;
                return float4(v.x * 1, v.yzw);
            }
            ENDCG
        }
    }
}
