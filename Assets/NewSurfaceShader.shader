Shader "Custom/RayTracer"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 pos : TEXCOORD0;
            };

            struct v2f
            {
                float2 pos : TEXCOORD0;
				float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.pos = v.pos;
                return o;
            }

            float3 CamDim;
            float4x4 CamLocalToWorldMatrix;

            struct Ray {
                float3 origin;
                float3 dir;
            };

            float4 frag(v2f i) : SV_Target
            {
                // Calculate position of a pixel with respect to the camera's projection plane.
                float3 viewPointLocal = float3(i.pos - 0.5, 1) * CamDim; // center the pixel and scale it by CamDim.
                float3 viewPoint = mul(CamLocalToWorldMatrix, float4(viewPointLocal, 1)); // Convert point to world coordinates.
                
                Ray ray;
                ray.origin = _WorldSpaceCameraPos;
                ray.dir = normalize(viewPoint - ray.origin); // Ray stars from camera and ends in viewPoint.
                return float4(ray.dir, 0); 
            }
            ENDCG
        }
    }
}
