using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageEffectAllowedInSceneView, ExecuteAlways]
public class Manager : MonoBehaviour
{
    Material rayTracerMaterial;
    Shader rayTracerShader;

    [SerializeField] bool useShaderInSceneView;

    void Start()
    {

    }

    void Update()
    {
        
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

    //  Post-processing effects: read in pixels from source image,
    //  apply custom shader, then render result to destination.
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Camera.current.name != "Main Camera" || useShaderInSceneView) {
            // Initialize shader and material.
            rayTracerShader = Shader.Find("Custom/RayTracer");
            InitMaterial(rayTracerShader, ref rayTracerMaterial);


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
}
