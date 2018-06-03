using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Unit {

    GameObject targetUnit;

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
                FindNearestTarget();
                CalculatePath();
                if (!done)
                {
                    FindSelectableTiles();
                    done = true;
                }
                actualTargetTile.target = true;
                break;
            case State.SelectingActionTarget:
                TurnManager.EndTurn();
                break;
            case State.Attacking:
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

        }
    }

    void CalculatePath()
    {
        Tile targetTile = GetTargetTile(targetUnit);//GetTargetTile(target);
        //Debug.Log(string.Format("targetTile coords x:{0} y{1}", targetTile.gridCoord.x, targetTile.gridCoord.y));
        //GridManager.GetTileBeneathUnit(targetUnit.GetComponent<Unit>(), out targetTile);
        //GridManager.GetTileAtCoord(new Vector2((int)targetUnit.transform.localPosition.x, (int)targetUnit.transform.localPosition.z), out targetTile);
        FindPath(targetTile);
    }

    void FindNearestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

        GameObject nearest = null;
        float distance = Mathf.Infinity;

        foreach(GameObject obj in targets)
        {
            float d = Vector3.Distance(transform.position, obj.transform.position);

            if (d < distance)
            {
                distance = d;
                nearest = obj;
            }
        }

        targetUnit = nearest;
        //Debug.Log(string.Format("target found: x{0} y{1}", targetUnit.GetComponent<Unit>().gridCoord.x, targetUnit.GetComponent<Unit>().gridCoord.y));
    }
}
