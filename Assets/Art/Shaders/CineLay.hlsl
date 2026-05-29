Shader "UI/VignetteGrain"
{
    Properties
    {
        _GrainStrength  ("Grain Strength",  Range(0, 1)) = 0.08
        _GrainScale     ("Grain Scale",     Range(1, 10)) = 4.0
        _VigStrength    ("Vig Strength",    Range(0, 1)) = 0.6
        _VigSmoothness  ("Vig Smoothness",  Range(0.01, 1)) = 0.4
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _GrainStrength;
            float _GrainScale;
            float _VigStrength;
            float _VigSmoothness;
            CBUFFER_END

            // Simple hash noise — no texture needed
            float rand(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.texcoord;

                // Film grain — animated per frame via _Time
                float2 grainUV = uv * _GrainScale + _Time.y * 0.1;
                float  grain   = rand(grainUV) - 0.5;

                // Radial vignette
                float2 vigUV  = uv * 2.0 - 1.0;
                float  vDist  = length(vigUV);
                float  vignette = smoothstep(1.0 - _VigSmoothness, 1.0, vDist * _VigStrength);

                float alpha = saturate(vignette + grain * _GrainStrength);
                return half4(0, 0, 0, alpha);
            }
            ENDHLSL
        }
    }
}