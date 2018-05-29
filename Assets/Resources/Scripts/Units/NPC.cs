using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Unit {

    GameObject target;

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

        switch (state)
        {
            case State.SelectingMoveTarget:
                FindNearestTarget();
                CalculatePath();
                FindSelectableTiles();
                actualTargetTile.target = true;
                break;
            case State.SelectingActionTarget:
                TurnManager.EndTurn();
                break;
            case State.Moving:
                Move();
                break;

        }
    }

    void CalculatePath()
    {
        Tile targetTile = GetTargetTile(target);
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

        target = nearest;
    }
}
