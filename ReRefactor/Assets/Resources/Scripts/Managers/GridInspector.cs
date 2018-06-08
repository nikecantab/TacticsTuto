using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInspector : MonoBehaviour {
    

    // Use this for initialization
    void Start ()
    {
        //unitsGrid = GridManager.unitsGrid;
        //tilesGrid = GridManager.tilesGrid;
    }
	
	// Update is called once per frame
	void Update () {
        //unitsGrid = GridManager.unitsGrid;
        //tilesGrid = GridManager.tilesGrid;
	}

    public void UpdateAllTiles()
    {
        GridManager.UpdateAll();
    }

    public void UpdateUnit()
    {
        //GridManager.UpdateUnitPosition(TurnManager.turnTeam.Peek(), TurnManager.turnTeam.Peek().gridCoord);
    }

}
