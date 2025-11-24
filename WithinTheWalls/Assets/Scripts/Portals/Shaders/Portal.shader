Shader "Custom/PortalURP"
{
    Properties
    {
        _InactiveColour ("Inactive Colour", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Includes URP core libraries
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _InactiveColour;
            int _DisplayMask ;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 screenPos   : TEXCOORD0;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;

                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.screenPos = ComputeScreenPos(o.positionHCS);

                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;

                half4 portalCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                return portalCol * _DisplayMask  + _InactiveColour * (1 - _DisplayMask );
            }

            ENDHLSL
        }
    }

    Fallback Off
}