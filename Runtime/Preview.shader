Shader "Fidget/Preview"
{
    Properties { _Color ("Tint", Color) = (0.25, 0.7, 0.95, 1) }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Cull Back
            ZWrite On
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct Input { float4 vertex : POSITION; };
            struct Interpolator { float4 vertex : SV_POSITION; float3 position : TEXCOORD0; };
            float4 _Color;
            Interpolator vert(Input v)
            {
                Interpolator o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.position = v.vertex.xyz;
                return o;
            }
            float4 frag(Interpolator i) : SV_Target
            {
                // Preview lighting from screen derivatives: no CPU normal array or normal upload.
                float3 n = normalize(cross(ddy(i.position), ddx(i.position)));
                float light = 0.3 + 0.7 * abs(dot(n, normalize(float3(1, 2, -3))));
                return float4(_Color.rgb * light, _Color.a);
            }
            ENDCG
        }
    }
}
