Shader "Unlit/SolidLine"
{
    Properties
    {
        _Antialias("Antialias", Range(0.001, 0.01)) = 0.001
        _Thickness("Thickness", float) = 0.5
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

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
                fixed4 color : COLOR0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR0;
            };

            float _Antialias;
            float _Thickness;

            float4 Line(float2 pos, float2 p1, float2 p2, float width, float4 color) {
                float k = (p1.y - p2.y) / (p1.x - p2.x);
                float b = p1.y - k * p1.x;
                float dst = abs(k * pos.x - pos.y + b) / sqrt(k * k + 1);
                float t = 1 - smoothstep(width - _Antialias, width + _Antialias, dst);
                return float4(color.rgb, t);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 a = float2(0, 0.5);
                float2 b = float2(1, 0.5);
                float4 l = Line(i.uv, a, b, _Thickness * 5, i.color);
                return l;
            }
            ENDCG
        }
    }
}
