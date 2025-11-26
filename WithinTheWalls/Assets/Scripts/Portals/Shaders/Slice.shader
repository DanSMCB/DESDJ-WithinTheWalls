Shader "Custom/SliceURP_Lit"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        sliceNormal("normal", Vector) = (0,0,0,0)
        sliceCentre ("centre", Vector) = (0,0,0,0)
        sliceOffsetDst("offset", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "IgnoreProjector"="True" }
        LOD 200
        Cull Back

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            // vertex / fragment entry
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"

            // Properties (mapped)
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _Color;
            half _Glossiness;
            half _Metallic;

            float3 sliceNormal;
            float3 sliceCentre;
            float sliceOffsetDst;

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                float4 tangentOS  : TANGENT;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float2 uv          : TEXCOORD2;
                float3 viewDirWS   : TEXCOORD3;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // Positions / normals in world space
                float4 posWS = TransformObjectToWorld(float4(IN.positionOS, 1.0));
                OUT.positionWS = posWS.xyz;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv;

                // Compute view direction in world space
                OUT.viewDirWS = normalize(_WorldSpaceCameraPos - OUT.positionWS);

                // HClip
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                return OUT;
            }

            // Helper: Fresnel Schlick approximation
            float3 FresnelSchlick(float3 F0, float cosTheta)
            {
                return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
            }

            // Distribution: GGX / Trowbridge-Reitz
            float DistributionGGX(float3 N, float3 H, float roughness)
            {
                float a = roughness*roughness;
                float a2 = a*a;
                float NdotH = saturate(dot(N,H));
                float NdotH2 = NdotH*NdotH;

                float denom = (NdotH2 * (a2 - 1.0) + 1.0);
                denom = UNITY_MAX(PI * denom * denom, 1e-5);
                return a2 / denom;
            }

            // Geometry – Schlick-GGX approx
            float GeometrySchlickGGX(float NdotV, float roughness)
            {
                float r = (roughness + 1.0);
                float k = (r*r) / 8.0;
                return NdotV / (NdotV * (1.0 - k) + k);
            }

            float GeometrySmith(float3 N, float3 V, float3 L, float roughness)
            {
                float NdotV = saturate(dot(N, V));
                float NdotL = saturate(dot(N, L));
                float ggx2 = GeometrySchlickGGX(NdotV, roughness);
                float ggx1 = GeometrySchlickGGX(NdotL, roughness);
                return ggx1 * ggx2;
            }

            // Sample ambient probe (irradiance) helper
            float3 SampleAmbientProbe(float3 n)
            {
                // Use SHAmbient from Core.hlsl
                // EvaluateSH9 is available via Core.hlsl in URP versions; fallback to unity_ambient if not
                #ifdef UNITY_INLINE_EVALUATE_SH9
                    return ShadeSH9(float4(n,0)).rgb;
                #else
                    // fallback to simple ambient
                    return UNITY_LIGHTMODEL_AMBIENT.xyz;
                #endif
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // ----- CLIP (match original behaviour) -----
                float3 adjustedCentre = sliceCentre + sliceNormal * sliceOffsetDst;
                float3 offsetToSliceCentre = adjustedCentre - IN.positionWS;
                float clipVal = dot(offsetToSliceCentre, sliceNormal);
                clip(clipVal);

                // ----- Albedo & alpha -----
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half3 albedo = tex.rgb * _Color.rgb;
                float alpha = tex.a * _Color.a;

                // ----- PBR inputs -----
                // roughness mapping: invert glossiness (user provided _Glossiness -> smoothness)
                float smoothness = saturate(_Glossiness);
                // Map to roughness
                float roughness = max(1e-4, 1.0 - smoothness);
                float metallic = saturate(_Metallic);

                // Normal (use interpolated normalWS)
                float3 N = normalize(IN.normalWS);
                float3 V = normalize(IN.viewDirWS);

                // ----- Main directional light (URP helper) -----
                // Get main directional light data
                Light mainLight = GetMainLight(); // from Lighting.hlsl / Core.hlsl
                float3 L = normalize(-mainLight.direction); // light direction points from surface toward light
                float3 radiance = mainLight.color;

                // Cook-Torrance BRDF
                float3 H = normalize(V + L);
                float NDF = DistributionGGX(N, H, roughness);
                float G   = GeometrySmith(N, V, L, roughness);

                // Fresnel
                // F0: dielectric default = 0.04; for metals use albedo
                float3 F0 = lerp(float3(0.04,0.04,0.04), albedo, metallic);
                float3 F = FresnelSchlick(F0, saturate(dot(H, V)));

                float3 numerator = NDF * G * F;
                float denom = 4.0 * max(0.001, saturate(dot(N, V)) * saturate(dot(N, L)));
                float3 specular = numerator / denom;

                // kD = (1 - F) * (1 - metallic)
                float3 kD = (1.0 - F) * (1.0 - metallic);

                float NdotL = saturate(dot(N, L));
                float3 Lo = (kD * albedo / PI + specular) * radiance * NdotL;

                // ----- Ambient / IBL approximation -----
                float3 ambient = SampleAmbientProbe(N) * 0.5; // scale factor to match URP ambient feel
                // Add a simple ambient specular approximation
                float3 ambientSpec = F * 0.03; // low ambient specular

                float3 colorOut = Lo + ambient + ambientSpec;

                // Apply tone mapping/gamma correction is handled by pipeline; output linear color
                return half4(colorOut, alpha);
            }

            ENDHLSL
        } // End Pass
    } // End SubShader

    FallBack "Diffuse"
}