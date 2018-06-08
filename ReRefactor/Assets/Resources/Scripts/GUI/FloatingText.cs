using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour {
    float destroyTime = 1f;
    float count = 0f;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, destroyTime);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.rotation = Camera.main.transform.rotation;
        count++;

        if (count > 3.5 * destroyTime * 10f)
        {
            //start fade
            if (count % 2 == 0)
                GetComponent<MeshRenderer>().enabled = !GetComponent<MeshRenderer>().enabled;
        }
        else if (count > 2 * destroyTime * 10f)
        {
            //start fade
            if (count % 5 == 0)
                GetComponent<MeshRenderer>().enabled = !GetComponent<MeshRenderer>().enabled;
        }
	}
}
