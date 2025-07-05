Shader "Custom/GridMask"
{
    Properties {
        _MaskTex ("Grid Mask", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MaskTex;
            fixed4 _Color;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 maskCol = tex2D(_MaskTex, i.uv);
                // Умножаем цвет текстуры на цвет из материала, включая альфу
                fixed4 col = maskCol * _Color;
                // Если нужно, можно сделать альфа-тест: discard если прозрачный
                clip(col.a - 0.01);
                return col;
            }
            ENDCG
        }
    }
}
