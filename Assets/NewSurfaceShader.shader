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

            struct RayTracerMaterial {
                float4 color;
                float4 emissionColor;
	            float4 emissionStrength;
            };
            
            struct Collision {
                bool didCollide;
                float dst; // root/solution to the equation.
                float3 collisionPoint;
                float3 normal; 
                RayTracerMaterial material;
            };

            struct Sphere {
                float3 position;
                float radius;
                RayTracerMaterial material;
            };

            StructuredBuffer<Sphere> Spheres;
            int NumSpheres;
  


            float RandomValue(inout uint state)
			{
                state = state * 747796405 + 2891336453;
				uint result = ((state >> ((state >> 28) + 4)) ^ state) * 277803737;
				result = (result >> 22) ^ result;
				return result / 4294967295.0; // 2^32 - 1
			}

			// Random value in normal distribution (with mean=0 and sd=1)
			float RandomValueNormalDistribution(inout uint state)
			{
				// Thanks to https://stackoverflow.com/a/6178290
				float theta = 2 * 3.1415926 * RandomValue(state);
				float rho = sqrt(-2 * log(RandomValue(state)));
				return rho * cos(theta);
			}

			// Calculate a random direction
			float3 RandomDirection(inout uint state)
			{
				// Thanks to https://math.stackexchange.com/a/1585996
				float x = RandomValueNormalDistribution(state);
				float y = RandomValueNormalDistribution(state);
				float z = RandomValueNormalDistribution(state);
				return normalize(float3(x, y, z));
			}

            float3 RandomHemisphereDirection(float3 normal, inout uint rngState) 
            {
                float3 dir = RandomDirection(rngState);
                return dir * sign(dot(normal, dir));
            }

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

            Collision closestCollision(Ray ray) {
                Collision closest = (Collision)0;
                closest.dst = 1.#INF;
                
                for (int i = 0; i < NumSpheres; i++) {
                    Sphere nextSphere = Spheres[i];
                    Collision nextCollision = intersect(ray, nextSphere.position, nextSphere.radius);
                    if (nextCollision.didCollide && nextCollision.dst < closest.dst) {
                        closest = nextCollision;
                        closest.material = nextSphere.material;
                    }
                }
                return closest;
            }

            float3 Trace(Ray ray, inout uint rngState) {
                float3 incomingLight = 0;
                float3 rayColor = 1;
                for (int i = 0; i < 1; i++) {
                    Collision collision = closestCollision(ray);
                    if (collision.didCollide) {
                        ray.origin = collision.collisionPoint;
                        ray.dir = RandomHemisphereDirection(collision.normal, rngState);
                        RayTracerMaterial material = collision.material;
                        float3 emittedLight = material.emissionColor * material.emissionStrength;
                        incomingLight += emittedLight * rayColor;
                        rayColor *= material.color;
                    } else {
                        break;
                    }
                }
                return incomingLight;
            }

            float4 frag(v2f i) : SV_Target
            {
                uint2 numPixels = _ScreenParams.xy;
                uint2 pixelCoord = i.pos * numPixels;
                uint pixelIndex = pixelCoord.y * numPixels.x + pixelCoord.x;
                uint rngState = pixelIndex;
                // Calculate position of a pixel with respect to the camera's projection plane.
                float3 viewPointLocal = float3(i.pos - 0.5, 1) * CamDim; // center the pixel and scale it by CamDim.
                float3 viewPoint = mul(CamLocalToWorldMatrix, float4(viewPointLocal, 1)); // Convert point to world coordinates.
                
                Ray ray;
                ray.origin = _WorldSpaceCameraPos;
                ray.dir = normalize(viewPoint - ray.origin); // Ray stars from camera and ends in viewPoint.
                
                float3 pixelCol = Trace(ray, rngState);
                return float4(pixelCol, 1); 
            }
            ENDCG
        }
    }
}
