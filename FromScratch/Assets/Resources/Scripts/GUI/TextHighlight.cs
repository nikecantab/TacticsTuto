using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextHighlight : MonoBehaviour {
    public Text text;

    Color defaultColor;
    int counter;
    bool highlight = false;
    bool fade = false;

    private void Start()
    {
        defaultColor = text.color;
    }

    private void Update()
    {
        if (highlight)
        {
            float val = Mathf.Lerp(text.color.r, 1.0f, 0.5f);

            text.color = new Color(val, val, val);
        }
        else if (fade)
        {
            float val = Mathf.Lerp(text.color.r, defaultColor.r, 0.5f);
                
            text.color = new Color(val, val, val);

        }
    }

    public void Highlight()
    {
        fade = false;
        highlight = true;
    }

    public void Fade()
    {
        highlight = false;
        fade = true;
    }
}
