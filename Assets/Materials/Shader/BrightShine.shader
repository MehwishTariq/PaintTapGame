Shader "Custom/BrightShine"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _Brightness ("Brightness", Range(0, 2)) = 1.2
        _Saturation ("Saturation", Range(0, 2)) = 1.3
        _ShineColor ("Shine Color", Color) = (1,1,1,1)
        _ShineWidth ("Shine Width", Range(0,1)) = 0.2
        _ShineOffset ("Shine Offset", Range(-1,1)) = -1
        _ShineIntensity ("Shine Intensity", Range(0,2)) = 1
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

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Brightness;
            float _Saturation;
            float _ShineWidth;
            float _ShineOffset;
            float4 _ShineColor;
            float _ShineIntensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Saturation
                float gray = dot(col.rgb, float3(0.3, 0.59, 0.11));
                col.rgb = lerp(gray.xxx, col.rgb, _Saturation);

                // Brightness
                col.rgb *= _Brightness;

                // Shine sweep
                float shine = smoothstep(_ShineOffset, _ShineOffset + _ShineWidth, i.uv.x);
                col.rgb += _ShineColor.rgb * shine * _ShineIntensity * col.a;

                return col;
            }
            ENDCG
        }
    }
}
