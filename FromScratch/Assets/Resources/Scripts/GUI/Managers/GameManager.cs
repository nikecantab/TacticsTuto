using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject exitPrefab;
    GameObject exitInstance;

    [SerializeField]
    GameObject gameCanvas;
    [SerializeField]
    GameObject tutorialCanvas;
    [SerializeField]
    GameObject winCanvas;
    [SerializeField]
    GameObject loseCanvas;

    [SerializeField]
    Cursor cursor;

    public bool win = false;
    public bool loss = false;

    int counter = 0;

	// Use this for initialization
	void Awake () {
        Application.targetFrameRate = 60;

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            gameCanvas.SetActive(false);
            tutorialCanvas.SetActive(true);
            winCanvas.SetActive(false);
            loseCanvas.SetActive(false);
        }
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

        if (win)
        {
            if (winCanvas.activeSelf == false)
            {
                winCanvas.SetActive(true);
                gameCanvas.SetActive(false);
            }
            cursor.state = CursorState.Inactive;
        }
        else if (loss)
        {
            if (loseCanvas.activeSelf == false)
            {
                loseCanvas.SetActive(true);
                gameCanvas.SetActive(false);
            }
            cursor.state = CursorState.Inactive;
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

    public void StartMatch()
    {
        tutorialCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        TempTurnManager.StartTurn();
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene(0);
    }

    public void CheckWinLose()
    {
        StartCoroutine(CheckLater());
    }

    IEnumerator CheckLater()
    {
        yield return new WaitForSeconds(1.2f);
        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerUnit");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyUnit");

        Debug.Log("players: " + players.Length + "  enemies: " + enemies.Length);

        if (players.Length == 0)
        {
            loss = true;

            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<Unit>().turn = false;
            }

            yield break;
        }
        else if (enemies.Length == 0)
        {
            win = true;
            yield break;
        }
    }
}
