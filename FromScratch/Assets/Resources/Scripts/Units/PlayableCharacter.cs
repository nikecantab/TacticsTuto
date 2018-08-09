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
            case UnitState.SelectingDestination:
                animator.speed = 1;
                //cursor.state = CursorState.SelectingUnit;
                break;
            case UnitState.Moving:
                animator.speed = 2;
                cursor.state = CursorState.Inactive;
                Move();
                break;
            case UnitState.AwaitingChoice:
                animator.speed = 1;
                cursor.state = CursorState.AwaitingChoice;
                break;
            case UnitState.SelectingTarget:
                animator.speed = 1;
                break;
            case UnitState.Attacking:
                switch(currentAttack)
                {
                    case AttackType.Cripple:

                        combatTarget.TakeDamage(currentAttack, Strength);
                        break;
                    case AttackType.Impair:

                        combatTarget.TakeDamage(currentAttack, Energy);
                        break;
                    case AttackType.Weaken:

                        combatTarget.TakeDamage(currentAttack, Energy);
                        break;
                }
                //TODO: add delay
                state = UnitState.EndingPhase;

                break;
            case UnitState.EndingPhase:
                animator.speed = 1;
                cursor.ResetCursor();
                combatTarget = null;
                state = UnitState.SelectingDestination;
                UpdatePosition();
                done = false;
                TempTurnManager.EndPhase();
                break;
        }
	}
}
