using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageEffectAllowedInSceneView, ExecuteAlways]
public class Manager : MonoBehaviour
{
    Material rayTracerMaterial;
    Shader rayTracerShader;
    ComputeBuffer sphereBuffer;


    [SerializeField] bool useShaderInSceneView;

    void Start()
    {

    }

    void Update()
    {
        
    }
    	// Create a compute buffer containing the given data (Note: data must be blittable)
	public static void CreateStructuredBuffer<T>(ref ComputeBuffer buffer, T[] data) where T : struct
	{
		// Cannot create 0 length buffer (not sure why?)
		int length = Mathf.Max(1, data.Length);
		// The size (in bytes) of the given data type
		int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));

		// If buffer is null, wrong size, etc., then we'll need to create a new one
		if (buffer == null || !buffer.IsValid() || buffer.count != length || buffer.stride != stride)
		{
			if (buffer != null) { buffer.Release(); }
			buffer = new ComputeBuffer(length, stride, ComputeBufferType.Structured);
		}

		buffer.SetData(data);
	}

    // Initialize a material given a shader. If shader is null, assign "Unlit/Texture" to material.
    public static void InitMaterial(Shader shader, ref Material mat)
    {
        if (mat == null || (mat.shader != shader && shader != null))
        {
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Texture");
            }

            mat = new Material(shader);
        }
    }

    void CreateSpheres() {
        RayTracedSphere[] sphereObjects = FindObjectsOfType<RayTracedSphere>();
        Sphere[] spheres = new Sphere[sphereObjects.Length];

        for (int i = 0; i < sphereObjects.Length; i++) {
            spheres[i] = new Sphere()
			{
				position = sphereObjects[i].transform.position,
				radius = sphereObjects[i].transform.localScale.x * 0.5f,
				material = sphereObjects[i].material
			};
        }
        CreateStructuredBuffer(ref sphereBuffer, spheres);
		rayTracerMaterial.SetBuffer("Spheres", sphereBuffer);
		rayTracerMaterial.SetInt("NumSpheres", sphereObjects.Length);
    }

    //  Post-processing effects: read in pixels from source image,
    //  apply custom shader, then render result to destination.
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Camera.current.name == "Main Camera" || useShaderInSceneView) {
            // Initialize shader and material.
            rayTracerShader = Shader.Find("Custom/RayTracer");
            InitMaterial(rayTracerShader, ref rayTracerMaterial);
            CreateSpheres();
            UpdateCamera(Camera.current); 

            Graphics.Blit(null, destination, rayTracerMaterial);
        } else {
            Graphics.Blit(source, destination);
        }
    }

    // Send updated camera dimensions and position to shader.
    void UpdateCamera(Camera cam)
    {
        float planeHeight = cam.nearClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2;
        float planeWidth = planeHeight * cam.aspect;

        rayTracerMaterial.SetVector("CamDim", new Vector3(planeWidth, planeHeight, cam.nearClipPlane));
        rayTracerMaterial.SetMatrix("CamLocalToWorldMatrix", cam.transform.localToWorldMatrix);
    }

    void OnDisable()
	{
		if (sphereBuffer != null)
		{
			sphereBuffer.Release();
		}
		
	}

}
