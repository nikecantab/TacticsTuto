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

    public float mouseSensitivity;
    Vector3 lastPosition;
    float hSpeed = 0;
    float vSpeed = 0;

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
            var cam = Camera.main;

            cam.orthographicSize += 0.25f * zoomDirection * (Mathf.Abs(zoomSizes[currentZoomIndex] - zoomDestination));
            if (Mathf.Abs(cam.orthographicSize - zoomDestination) < 0.05f)
            {
                cam.orthographicSize = zoomDestination;
                currentZoomIndex += zoomDirection;
                zooming = false;
            }

        }

        //Pan
        if (Input.GetMouseButtonDown(2))
        {
            lastPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            hSpeed = -((Mathf.Sqrt(2) / 2) * delta.x - (Mathf.Sqrt(2) / 2) * delta.y) * mouseSensitivity * zoomSizes[currentZoomIndex];
            vSpeed = -((Mathf.Sqrt(2) / 2) * delta.x + (Mathf.Sqrt(2) / 2) * delta.y) * mouseSensitivity * zoomSizes[currentZoomIndex];

            //transform.Translate(
            //    -((Mathf.Sqrt(2) / 2) * delta.x - (Mathf.Sqrt(2) / 2) * delta.y) * mouseSensitivity * zoomSizes[currentZoomIndex],
            //    0,
            //    -((Mathf.Sqrt(2) / 2) * delta.x + (Mathf.Sqrt(2) / 2) * delta.y) * mouseSensitivity * zoomSizes[currentZoomIndex]);
            //transform.Translate(-(Mathf.Sqrt(2) / 2) * (delta.x * mouseSensitivity * zoomSizes[currentZoomIndex]), 0, -(Mathf.Sqrt(2) / 2) * (delta.y * mouseSensitivity * zoomSizes[currentZoomIndex]));
            
            lastPosition = Input.mousePosition;
        }

        transform.Translate(hSpeed, 0, vSpeed);
        hSpeed = Mathf.Lerp(hSpeed, 0, 0.08f);
        vSpeed = Mathf.Lerp(vSpeed, 0, 0.08f);

        hSpeed = Mathf.Abs(hSpeed) < 0.08f ? 0 : hSpeed;
        vSpeed = Mathf.Abs(vSpeed) < 0.08f ? 0 : vSpeed;

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -9, 9), 0, Mathf.Clamp(transform.position.z, -9, 9));
    }
}
