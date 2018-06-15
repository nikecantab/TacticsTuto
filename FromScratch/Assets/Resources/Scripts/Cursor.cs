using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
    public Vector2Int pos;
    public GridManager gridManager;

    Vector3 lastPos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 mousePos = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var gridLayer = 1 << LayerMask.NameToLayer("Grid");


        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridLayer))
        {
            transform.localPosition= new Vector3(hit.transform.localPosition.x, transform.localPosition.y, hit.transform.localPosition.z);
            pos.x = (int)transform.localPosition.x;
            pos.y = (int)transform.localPosition.z;
            lastPos = transform.localPosition;
        }
        else
        {
            transform.localPosition = lastPos;
            pos.x = (int)lastPos.x;
            pos.y = (int)lastPos.z;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //test astar
            gridManager.DectivateAllTiles(gridManager.greenTiles);
            gridManager.ActivateAroundTile(gridManager.greenTiles, 4, pos);
        }
    }
}

/*
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var ignoreEntities = 1 << 8;
            ignoreEntities = ~ignoreEntities;

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ignoreEntities))
                 if (hit.collider.tag == "Tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();

                    //state 
                    if (state == State.SelectingMoveTarget)
                    {
                        if (t.current)
                        {
                            state = State.SelectingActionTarget;
                            done = false;
                        }
                        else if (t.selectable)
                        {
                            MoveToTile(t);
                            done = false;
                        }
                    }
                    else if (state == State.SelectingActionTarget)
                    {
                        if (t.enemyOccupied)
                        {
                            //Attack
                            combatTarget = t.CheckEnemyOccupied(t.gridCoord);
                            state = State.Attacking;
                            done = false;
                        }
                        else if (t.current)
                            TurnManager.EndTurn();
                    }

                }*/
