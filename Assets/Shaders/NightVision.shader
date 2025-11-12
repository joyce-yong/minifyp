Shader "Custom/NightVision"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NightVisionColor ("Night Vision Color", Color) = (0, 1, 0, 1)
        _Brightness ("Brightness", Range(0.5, 3.0)) = 1.5
        _Contrast ("Contrast", Range(0.5, 3.0)) = 1.5
        _NoiseIntensity ("Noise Intensity", Range(0, 1)) = 0.3
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.1
        _VignetteIntensity ("Vignette Intensity", Range(0, 1)) = 0.5
        _NoiseScale ("Noise Scale", Range(1, 100)) = 50
        _ScanlineSpeed ("Scanline Speed", Range(0, 5)) = 1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100

        Pass
        {
            Name "NightVisionPass"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _NightVisionColor;
                float _Brightness;
                float _Contrast;
                float _NoiseIntensity;
                float _ScanlineIntensity;
                float _VignetteIntensity;
                float _NoiseScale;
                float _ScanlineSpeed;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output = (Varyings)0;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = vertexInput.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                return output;
            }

            float random(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                
                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));
                
                float2 u = f * f * (3.0 - 2.0 * f);
                
                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            float4 frag (Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                
                float luminance = dot(col.rgb, float3(0.299, 0.587, 0.114));
                
                luminance = saturate((luminance - 0.5) * _Contrast + 0.5) * _Brightness;
                
                float3 nightVisionCol = luminance * _NightVisionColor.rgb;
                
                float2 noiseUV = uv * _NoiseScale + _Time.y * 0.1;
                float noiseValue = noise(noiseUV);
                nightVisionCol += (noiseValue - 0.5) * _NoiseIntensity;
                
                float scanline = sin((uv.y + _Time.y * _ScanlineSpeed) * 800.0) * 0.04;
                nightVisionCol += scanline * _ScanlineIntensity;
                
                float2 vignetteUV = uv * 2.0 - 1.0;
                float vignette = 1.0 - dot(vignetteUV, vignetteUV);
                vignette = saturate(vignette);
                vignette = 1.0 - (1.0 - vignette) * _VignetteIntensity;
                nightVisionCol *= vignette;
                
                float flicker = 1.0 + sin(_Time.y * 50.0) * 0.01;
                nightVisionCol *= flicker;
                
                nightVisionCol = saturate(nightVisionCol);
                
                return float4(nightVisionCol, col.a);
            }
            ENDHLSL
        }
    }
    
    FallBack "Sprites/Default"
}