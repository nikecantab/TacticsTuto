using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGridOpacity : MonoBehaviour {
    
    public void UpdateOpacity(float opacity)
    {
        var parent = GameObject.Find("Grid");
        if (parent != null)
        {
            for (var c = 0; c < parent.transform.childCount; c++)
            {
                parent.transform.GetChild(c).gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,opacity);
            }
        }
    }
}
