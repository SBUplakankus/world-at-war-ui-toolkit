Shader "Hidden/UIToolkit/VignetteGrain"
{
    Properties
    {
        _GrainStrength ("Grain Strength", Range(0,1)) = 0.08
        _GrainScale ("Grain Scale", Range(1,10)) = 4.0
        _VigStrength ("Vignette Strength", Range(0,1)) = 0.6
        _VigSmoothness ("Vignette Smoothness", Range(0.01,1)) = 0.4
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }

        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            Name "UIE"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            float _GrainStrength;
            float _GrainScale;
            float _VigStrength;
            float _VigSmoothness;

            Varyings vert(Attributes v)
            {
                Varyings o;

                o.positionCS = UnityObjectToClipPos(float4(v.positionOS, 1.0));
                o.uv = v.uv;
                o.color = v.color;

                return o;
            }

            float random(float2 uv)
            {
                return frac(
                    sin(dot(uv, float2(12.9898, 78.233)))
                    * 43758.5453123
                );
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 centered = i.uv * 2.0 - 1.0;

                float dist = length(centered);

                float vignette = smoothstep(
                    1.0 - _VigSmoothness,
                    1.0,
                    dist
                );

                vignette *= _VigStrength;

                float2 grainUV =
                    i.uv * _GrainScale +
                    frac(_Time.y);

                float grain =
                    (random(grainUV) - 0.5)
                    * _GrainStrength;

                float alpha = saturate(vignette + grain);

                return half4(0,0,0,alpha) * i.color;
            }

            ENDHLSL
        }
    }
}