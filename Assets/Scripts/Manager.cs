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

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Camera.current.name != "Main Camera" || useShaderInSceneView) {
            rayTracerShader = Shader.Find("Custom/RayTracer");
            InitMaterial(rayTracerShader, ref rayTracerMaterial);
            UpdateCamera(Camera.current); 
            Graphics.Blit(null, destination, rayTracerMaterial);
        } else {
            Graphics.Blit(source, destination);
        }
    }

    void UpdateCamera(Camera cam)
    {
        float planeHeight = cam.nearClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2;
        float planeWidth = planeHeight * cam.aspect;
        rayTracerMaterial.SetVector("CamDim", new Vector3(planeWidth, planeHeight, cam.nearClipPlane));
        rayTracerMaterial.SetMatrix("CamLocalToWorldMatrix", cam.transform.localToWorldMatrix);
    }
}
