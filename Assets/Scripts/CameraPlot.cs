using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraPlot : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        CameraRay();
    }

    // Update is called once per frame
    void Update()
    {
        CameraRay();
        //Application.Quit();
    }

    void DrawSphere(Vector3 position, float radius, Color color)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        sphere.GetComponent<Renderer>().material.color = color;
    }




    void CameraRay()
    {
        Camera cam = Camera.main;
        Transform camT = cam.transform;
        float planeHeight = cam.nearClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2;
        float planeWidth = planeHeight * cam.aspect;
        Vector3 bottomLeft = new Vector3(-planeWidth / 2, -planeHeight / 2, cam.nearClipPlane);

        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                float tx = x / (16 - 1f); 
                float ty = y / (9 - 1f);

                Vector3 pointLocal = bottomLeft + new Vector3(planeWidth * tx, planeHeight * ty);
                Vector3 point = camT.position + camT.right * pointLocal.x + camT.up * pointLocal.y + camT.forward * pointLocal.z;

                // Calculate the direction from the camera to the point on the near clipping plane
                Vector3 dir = (point - camT.position);
                dir.Normalize();

                // Draw the arrow fully spanning from the camera to the clipping plane
                Debug.DrawRay(camT.position, dir * cam.nearClipPlane, Color.green);

            }
        }
    }

}