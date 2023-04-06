Shader "Custom/TerrainShader"
{
    Properties
    {
        defaultTexture("Texture", 2D) = "white"
        grassTexture("Texture", 2D) = "green"
        dirtTexture("Texture", 2D) = "brown"
        sandTexture("Texture", 2D) = "yellow"
        waterTexture("Texture", 2D) = "blue"
        defaultScale("Scale", float) = 1
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
        float textureBlends[4];
        float textureStartHeights[4];
        float textureScales[4];
        UNITY_DECLARE_TEX2DARRAY(terrainTextures);

        sampler2D defaultTexture;
        float defaultScale;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        float inverseLerp(float smallestValue, float biggestValue, float value){
            return saturate((value-smallestValue)/(biggestValue-smallestValue));
        }

        float3 projectionAxis(float3 worldPosistion, float scale, float3 blendAxes, int index){

            float3 scaledWorldPos = worldPosistion / scale;
            float3 xAxis = UNITY_SAMPLE_TEX2DARRAY(terrainTextures, float3(scaledWorldPos.y, scaledWorldPos.z, index)) * blendAxes.x;
            float3 yAxis = UNITY_SAMPLE_TEX2DARRAY(terrainTextures, float3(scaledWorldPos.x, scaledWorldPos.z, index)) * blendAxes.y;
            float3 zAxis = UNITY_SAMPLE_TEX2DARRAY(terrainTextures, float3(scaledWorldPos.x, scaledWorldPos.y, index)) * blendAxes.z;

            return xAxis + yAxis + zAxis;
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
            float3 blendAxes = abs(IN.worldNormal);
            blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

            for(int i = 0; i < 4; i++){
                
                float drawStrength = inverseLerp(-textureBlends[i]/2, textureBlends[i]/2, heightPercent - textureStartHeights[i]);
                float3 textureProjection = projectionAxis(IN.worldPos, textureScales[i], blendAxes, i);

                o.Albedo = o.Albedo * (1 - drawStrength) + textureProjection * drawStrength;
            }

        }
        ENDCG
    }
    FallBack "Diffuse"
}
