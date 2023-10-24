using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    Material rayTracerMaterial;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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

        rayTracerMaterial = new Material(Shader.Find("Custom/Shader"));
        UpdateCamera(Camera.current);
        Graphics.Blit(source, destination, rayTracerMaterial);
    }


    void UpdateCamera(Camera cam)
    {
        float planeHeight = cam.nearClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2;
        float planeWidth = planeHeight * cam.aspect;
        rayTracerMaterial.SetVector("CamDim", new Vector3(planeWidth, planeHeight, cam.nearClipPlane));
        rayTracerMaterial.SetMatrix("CamLocalToWorldMatrix", cam.transform.localToWorldMatrix);

    }

}
