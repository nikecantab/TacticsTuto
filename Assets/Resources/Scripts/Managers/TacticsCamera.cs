using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsCamera : MonoBehaviour {
    
    [SerializeField]
    int rotationAmount = 90;

    [SerializeField]
    float rotSpeed = 1;

    [SerializeField]
    int[] zoomSizes = { 3, 7, 12 };

    int currentZoomIndex = 1;

    bool rotating;
    int rotDirection;
    int rotDestination;
    int rotCurrent;
    float r;

    bool zooming;
    int zoomDirection;
    float zoomDestination;

    private void Awake()
    {
        rotating = false;
        rotDirection = 1;
        zooming = false;
        zoomDirection = 1;
    }

    public void RotateLeft()
    {
        if (!rotating)
        {
            rotating = true;
            rotDirection = 1;
            rotCurrent = (int)transform.rotation.eulerAngles.y;
            rotDestination = rotCurrent + rotationAmount * rotDirection;
            r = 0;
        }
    }

    public void RotateRight()
    {
        if (!rotating)
        {
            rotating = true;
            rotDirection = -1;
            rotCurrent = (int)transform.rotation.eulerAngles.y;
            rotDestination = rotCurrent + rotationAmount * rotDirection;
            r = 0;
        }
    }

    public void ZoomIn()
    {
        if (!zooming)
        {
            if (currentZoomIndex > 0)
            {
                zooming = true;
                zoomDirection = -1;
                zoomDestination = zoomSizes[currentZoomIndex + zoomDirection];
            }
        }
    }

    public void ZoomOut()
    {
        if (!zooming)
        {
            if (currentZoomIndex < zoomSizes.Length - 1)
            {
                zooming = true;
                zoomDirection = 1;
                zoomDestination = zoomSizes[currentZoomIndex + zoomDirection];
            }
        }
    }

    private void FixedUpdate()
    {
        if (rotating)
        {
            r += 0.05f * rotSpeed;
            if (r >= 1)
                r = 1;
            transform.eulerAngles = new Vector3(transform.rotation.x, Mathf.Lerp(rotCurrent, rotDestination, r), transform.rotation.z);
            //rotation = Quaternion.Euler(transform.rotation.x, Mathf.Lerp(rotCurrent, rotDestination, r), transform.rotation.z);

            if (r == 1)
            {
                //transform.rotation = new Quaternion(transform.rotation.x, rotDestination, transform.rotation.z, transform.rotation.w);
                rotating = false;
            }
        }
        if (zooming)
        {
            var cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            cam.orthographicSize += 0.25f * zoomDirection * (Mathf.Abs(zoomSizes[currentZoomIndex] - zoomDestination));
            if (Mathf.Abs(cam.orthographicSize - zoomDestination) < 0.05f)
            {
                cam.orthographicSize = zoomDestination;
                currentZoomIndex += zoomDirection;
                zooming = false;
            }

        }
    }
}
