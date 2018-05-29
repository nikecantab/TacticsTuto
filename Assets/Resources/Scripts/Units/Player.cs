using System.Collections;
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
        CheckIfDead();
        if (!turn)
            return;

        if (state!=State.Moving)
            GridManager.UpdateUnitPosition(this, new Vector2(transform.localPosition.x, transform.localPosition.z));

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
            case State.Attacking:
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
                        }
                        else if (t.selectable)
                        {
                            MoveToTile(t);
                        }
                    }
                    else if (state == State.SelectingActionTarget)
                    {
                        if (t.enemyOccupied)
                        {
                            //Attack
                            combatTarget = t.CheckEnemyOccupied();
                            state = State.Attacking;
                        }
                        else if (t.current)
                            TurnManager.EndTurn();
                    }

                }
            }
        }
    }

}
