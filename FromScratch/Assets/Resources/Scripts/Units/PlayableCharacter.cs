using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacter : Unit {

    Cursor cursor;
    Animator animator;

	// Use this for initialization
	void Start () {
        Init();
        cursor = GameObject.Find("GUICursor").GetComponent<Cursor>();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (destroy)
        {
            RemoveUnit();
            return;
        }
        

        if (!turn)
            return;

        switch (state)
        {
            case UnitState.SelectingMoveTarget:
                animator.speed = 1;
                //cursor.state = CursorState.SelectingUnit;
                break;
            case UnitState.Moving:
                animator.speed = 2;
                cursor.state = CursorState.Inactive;
                Move();
                break;
            case UnitState.SelectingActionTarget:
                animator.speed = 1;
                cursor.state = CursorState.SelectingTarget;
                break;
            case UnitState.Attacking:
                //TODO: attacking
                state = UnitState.EndingPhase;

                break;
            case UnitState.EndingPhase:
                animator.speed = 1;
                cursor.ResetCursor();
                state = UnitState.SelectingMoveTarget;
                UpdatePosition();
                done = false;
                TempTurnManager.EndPhase();
                break;
        }
	}
}
