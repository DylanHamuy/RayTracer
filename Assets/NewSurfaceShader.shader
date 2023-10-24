Shader "Custom/Shader"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy; // Pass vertex coordinates as UV
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return half4(i.uv.x, i.uv.x, i.uv.x, 1); // Gradient from black to white
            }
            ENDCG
        }
    }
}
