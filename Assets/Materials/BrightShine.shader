Shader "UI/ColorPop"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Brightness ("Brightness", Range(1, 3)) = 1.5
        _Saturation ("Saturation", Range(0, 3)) = 1.4
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _Brightness;
            float _Saturation;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 SaturateColor(float3 color, float sat)
            {
                float luminance = dot(color, float3(0.3, 0.59, 0.11));
                return lerp(float3(luminance, luminance, luminance), color, sat);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 tex = tex2D(_MainTex, i.uv);

                // Apply color only where image is white
                tex.rgb *= _Color.rgb;

                tex.rgb *= _Brightness;

                tex.rgb = SaturateColor(tex.rgb, _Saturation);

                return tex;
            }
            ENDCG
        }
    }
}
