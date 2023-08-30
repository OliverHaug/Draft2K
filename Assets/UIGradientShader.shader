Shader "Custom/UIGradientShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = _Color * tex2D(_MainTex, i.uv);

                float gradientPos = i.uv.y;

                if (gradientPos <= 0.1)
                {
                    col.a *= gradientPos * 2.0;
                }
                else if (gradientPos <= 0.45)
                {
                    col.a *= 0.2 + (gradientPos - 0.1) * 0.9;
                }
                else if (gradientPos <= 0.58)
                {
                    col.a *= 0.65 + (gradientPos - 0.45) * 0.22;
                }
                else if (gradientPos <= 0.77)
                {
                    col.a *= 0.76 + (gradientPos - 0.58) * 0.07;
                }
                else
                {
                    col.a *= 0.8 + (1.0 - gradientPos) * 0.7;
                }

                return col;
            }
            ENDCG
        }
    }
}
