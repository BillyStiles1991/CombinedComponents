using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxSpeed = 0.05f;
    public float imageWidth = 20f;

    private Transform[] backgrounds;
    private Vector3 lastCameraPos;

    //Script sourced from: https://github.com/LooFromLemonSchool/UnityScripts/blob/main/Parallax.cs

    void Start()
    {
        //We use three backgrounds for smoother looping

        backgrounds = new Transform[3];
        backgrounds[0] = transform.GetChild(0);
        backgrounds[1] = transform.GetChild(1);
        backgrounds[2] = transform.GetChild(2);


        lastCameraPos = cameraTransform.position;

    }

    private void LateUpdate()
    {
        float camDelta = cameraTransform.position.x - lastCameraPos.x;
        transform.position += Vector3.right * (camDelta * parallaxSpeed);
        lastCameraPos = cameraTransform.position;


        foreach (Transform bg in backgrounds)
        {
            float camToBg = cameraTransform.position.x - bg.position.x;

            if (camToBg > imageWidth * 1.5f)
            {
                //Move this background 3 widths ahead
                bg.position += Vector3.right * imageWidth * 3f;

            }

            else if (camToBg < -imageWidth * 1.5f)
            {
                bg.position -= Vector3.right * imageWidth * 3f;
            }
        }

    }
}