Shader "Custom/FresnelAlwaysOn"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _HighlightColor ("Highlight Color", Color) = (1,0,0,1)
        _FresnelPower ("Fresnel Power", Range(0.1, 8)) = 3
        _Intensity ("Highlight Intensity", Range(0, 5)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        Cull Off   // render both sides so no culling problems

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 viewDirWS : TEXCOORD0;
                float3 worldPos  : TEXCOORD1;
            };

            float4 _BaseColor;
            float4 _HighlightColor;
            float _FresnelPower;
            float _Intensity;

            v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.worldPos = worldPos;
                o.viewDirWS = normalize(_WorldSpaceCameraPos.xyz - worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Instead of using normals, just base fresnel on view distance from object center
                float viewFactor = 1.0 - saturate(dot(normalize(i.viewDirWS), float3(0,0,1)));
                float fresnel = pow(viewFactor, _FresnelPower);

                float3 finalColor = _BaseColor.rgb + _HighlightColor.rgb * fresnel * _Intensity;
                return float4(finalColor, 1);
            }
            ENDHLSL
        }
    }
}
