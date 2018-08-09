using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeCanvas : MonoBehaviour
{
    public TacticsCamera camera;
    public Canvas canvas;

    [SerializeField]
    float[] canvasSizes = { 0.0075f, 0.0125f, 0.02f, 0.0275f };

    [SerializeField]
    float[] canvasHeights = { 4, 6, 7.5f, 10 };

    [SerializeField]
    int currentSizeIndex = 1;

    float canvasSize;


    private void Awake()
    {
        camera = GameObject.Find("TacticsCamera").GetComponent<TacticsCamera>();
        currentSizeIndex = camera.currentZoomIndex;
        canvasSize = canvasSizes[currentSizeIndex];
        var rect = gameObject.GetComponent<RectTransform>();
        rect.localScale = new Vector3(canvasSize, canvasSize, canvasSize);
        rect.transform.localPosition = new Vector3(rect.transform.localPosition.x, canvasHeights[currentSizeIndex], rect.transform.localPosition.z);
    }
    
    private void FixedUpdate()
    {
        //check zooming 
        if (camera.zooming)
        {
            canvas.enabled = false;
        }
        else
        {
            if (!canvas.enabled)
            {
                canvas.enabled = true;
                currentSizeIndex = camera.currentZoomIndex;
            }
        }

        if (canvasSize != canvasSizes[currentSizeIndex])
        {
            canvasSize = canvasSizes[currentSizeIndex];
            var rect = gameObject.GetComponent<RectTransform>();
            rect.localScale = new Vector3(canvasSize, canvasSize, canvasSize);
            rect.transform.localPosition = new Vector3(rect.transform.localPosition.x, canvasHeights[currentSizeIndex], rect.transform.localPosition.z);
        }
    }
}
