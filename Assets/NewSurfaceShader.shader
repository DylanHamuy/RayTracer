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

            struct Collision {
                bool didCollide;
                float dst; // root/solution to the equation.
                float3 collisionPoint;
                float3 normal; // Fix the typo here
            };

            // https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection.html
            Collision intersect(Ray ray, float3 sphereCenter, float sphereRadius) 
            {
                Collision collision = (Collision)0;
                float3 originCenterOffset = ray.origin - sphereCenter;
                float a = dot(ray.dir, ray.dir); // a = 1, bc dot product with normalized vector with itself is 1
                float b = 2 * dot(ray.dir, originCenterOffset);
                float c = dot(originCenterOffset, originCenterOffset) - sphereRadius * sphereRadius;

                float discriminant = b * b - 4 * a * c; 
                // discriminant < 0 when ray doesn't intersect.
                if (discriminant >= 0) {
                    // the negation of the sqrt(discriminant) is the closer intersection.
                    float dst = (-b - sqrt(discriminant)) / (2 * a); // Use sqrt from UnityCG.cginc
                                
                    // dst < 0 is an intersection on the back side of the sphere.
                    if (dst >= 0) {
                        collision.didCollide = true;
                        collision.collisionPoint = ray.origin + dst * ray.dir;
                        collision.dst = dst;
                        collision.normal = normalize(collision.collisionPoint - sphereCenter); // Fix the typo here
                    }
                }
                return collision;
            }


            float4 frag(v2f i) : SV_Target
            {
                // Calculate position of a pixel with respect to the camera's projection plane.
                float3 viewPointLocal = float3(i.pos - 0.5, 1) * CamDim; // center the pixel and scale it by CamDim.
                float3 viewPoint = mul(CamLocalToWorldMatrix, float4(viewPointLocal, 1)); // Convert point to world coordinates.
                
                Ray ray;
                ray.origin = _WorldSpaceCameraPos;
                ray.dir = normalize(viewPoint - ray.origin); // Ray stars from camera and ends in viewPoint.
                return intersect(ray, float3(0,0,3), 0.5).didCollide; 
            }
            ENDCG
        }
    }
}
