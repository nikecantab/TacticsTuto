﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

	// Use this for initialization
	void Start ()
    {
        Init();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (CheckIfDead())
            return;
        if (!turn)
            return;

        switch (state)
        {
            case State.SelectingMoveTarget:
                if (!done)
                {
                    //FindAttackTiles();
                    FindSelectableTiles();
                    done = true;
                }
                CheckMouse();
                break;
            case State.SelectingActionTarget:
                remainingMove = 0;
                if (!done)
                {
                    //FindAttackTiles();
                    FindSelectableTiles();
                    done = true;
                }
                CheckMouse();
                break;
            case State.Moving:
                if (!done)
                {
                    GridManager.UpdateAll();
                    done = true;
                    remainingMove++; //cheat to correct the move countdown
                }
                Move();
                break;
            case State.Attacking:
                //if (!done)
                //{
                //    //start attacking
                //}
                combatTarget.Energy -= Strength;
                TurnManager.EndTurn();
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

                }
            }
        }
    }

}
