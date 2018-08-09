using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject exitPrefab;
    GameObject exitInstance;

    int counter = 0;

	// Use this for initialization
	void Awake () {
        Application.targetFrameRate = 60;
	}

    private void Update()
    {
        if (Input.GetButton("Exit"))
        {
            if (counter == 0)
            {
                var canvas = GameObject.Find("Canvas");
                exitInstance = Instantiate(exitPrefab,canvas.transform);
                exitInstance.GetComponent<Text>().color = new Color(1,1,1,0);
            }
            //Only works in build, not in editor
            counter++;

            var text = exitInstance.GetComponent<Text>();
            float a = (float)counter / 20;
            text.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Clamp(a, 0.0f, 1.0f));
            

            if (counter > 60)
            {
                Exit();
            }
        }
        else
        {
            if (exitInstance != null)
            {
                Destroy(exitInstance);
                exitInstance = null;
            }
            counter = 0;
        }
    }

    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void Exit()
    {
        Debug.Log("exit");
        Application.Quit();
    }
}
