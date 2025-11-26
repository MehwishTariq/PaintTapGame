Shader "Custom/FresnelHighlightAlwaysVisible"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _HighlightColor ("Highlight Color", Color) = (0,1,1,1)
        _FresnelPower ("Fresnel Power", Range(0.1, 8)) = 3
        _Intensity ("Highlight Intensity", Range(0, 5)) = 1
        _MinGlow ("Minimum Glow", Range(0, 1)) = 0.2
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
            };

            float4 _BaseColor;
            float4 _HighlightColor;
            float _FresnelPower;
            float _Intensity;
            float _MinGlow;

            v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normalWS = UnityObjectToWorldNormal(v.normal);
                o.viewDirWS = normalize(_WorldSpaceCameraPos.xyz - worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 N = normalize(i.normalWS);
                float3 V = normalize(i.viewDirWS);

                // Standard Fresnel
                float fresnel = pow(1.0 - saturate(dot(N, V)), _FresnelPower);

                // Force minimum glow
                fresnel = lerp(_MinGlow, 1.0, fresnel);

                float3 finalColor = _BaseColor.rgb + _HighlightColor.rgb * fresnel * _Intensity;
                return float4(finalColor, 1);
            }
            ENDHLSL
        }
    }
}
