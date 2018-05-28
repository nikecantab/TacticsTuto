using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove {

	// Use this for initialization
	void Start ()
    {
        Init();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!turn)
            return;

        switch (state)
        {
            case State.SelectingMoveTarget:
                FindSelectableTiles();
                CheckMouse();
                break;
            case State.SelectingActionTarget:
                FindAttackableTiles();
                CheckMouse();
                break;
            case State.Moving:
                Move();
                break;


        }
	}

    void CheckMouse()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var ignoreEntities = 1 << 8;
            ignoreEntities = ~ignoreEntities;

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ignoreEntities))
            {
                if (hit.collider.tag == "Tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();

                    //state 
                    if (state == State.SelectingMoveTarget)
                    {
                        if (t.selectable)
                        {
                            MoveToTile(t);
                        }
                    }
                    else if (state == State.SelectingActionTarget)
                    {
                        if (t.enemyOccupied)
                        {
                            //Attack
                            var enemy = t.GetOccupant();
                            enemy.Energy -= Strength;

                            TurnManager.EndTurn();
                        }

                        else if (t.current)
                            TurnManager.EndTurn();
                    }

                }
            }
        }
    }

}
