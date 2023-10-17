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
        
        //Application.Quit();
    }

    void CameraRay()
    {
        Camera cam = Camera.main;
        Transform camT = cam.transform;
        // Calculate the width and height of the projection plane using trig
        float planeHeight = cam.nearClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2;
        float planeWidth = (planeHeight / cam.pixelHeight) * cam.pixelWidth; // alternatively cam.aspect*Height
        Vector3 bottomLeft = new Vector3(0, 0, cam.nearClipPlane);
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = bottomLeft;
        sphere.transform.localScale = new Vector3(1, 1, 1); // Set the scale
        Material sphereMaterial = new Material(Shader.Find("Standard"));
        sphereMaterial.color = Color.red; // Change the color
        sphere.GetComponent<Renderer>().material = sphereMaterial;
    }
}