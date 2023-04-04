Shader "Custom/TerrainShader"
{
    Properties
    {

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        const static float smallValue = 0.0001;

        float PerlinNoiseScale;
        float3 keyColours[4];
        float colourHeights[4];
        float colourBlends[4];

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        float inverseLerp(float smallestValue, float biggestValue, float value){
            return saturate((value-smallestValue)/(biggestValue-smallestValue));
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float heightPercent = inverseLerp(0, PerlinNoiseScale, IN.worldPos.y);
            for(int i = 0; i < 4; i++){
                float colourStrength = inverseLerp(-colourBlends[i]/2, colourBlends[i]/2, heightPercent - colourHeights[i]);
                o.Albedo = o.Albedo * (1 - colourStrength) + keyColours[i] * colourStrength;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
