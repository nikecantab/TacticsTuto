using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtilities;

public class EnemyNPC : Unit {

    Cursor cursor;
    Vector2Int target;
    Vector2Int actualTarget;
    //Animator animator;

	// Use this for initialization
	void Start () {
        Init();
        cursor = GameObject.Find("GUICursor").GetComponent<Cursor>();
        //animator = GetComponent<Animator>();
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
                //animator.speed = 1;
                if (!done)
                {
                    StartCoroutine(SelectingDestinationCoroutine());
                }

                break;
            case UnitState.Moving:
                //animator.speed = 2;
                Move();

                break;
            case UnitState.AwaitingChoice:
                //animator.speed = 1;
                if (!done)
                {
                    StartCoroutine(AwaitingChoiceCoroutine());
                }
                break;
            case UnitState.SelectingTarget:
                //animator.speed = 1;
                //combatTarget = GetCombatTarget();

                if (combatTarget != null)
                {
                    TurnToFace(combatTarget.pos);
                    currentAttack = Utilities.Choose<AttackType>(attackTypes);
                    state = UnitState.Attacking;
                }
                else
                {
                    state = UnitState.EndingPhase;
                }
                break;
            case UnitState.Attacking:

                if (combatTarget!=null)
                {
                    switch (currentAttack)
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
                    combatTarget = null;
                }

                if (!done)
                {
                    StartCoroutine(AttackingCoroutine());
                }

                break;
            case UnitState.EndingPhase:
                //animator.speed = 1;
                combatTarget = null;
                state = UnitState.SelectingDestination;
                UpdatePosition();
                done = false;
                TempTurnManager.EndPhase();
                break;
        }
	}

    Stack<Vector2Int> GetThePathThatRocks(Stack<Vector2Int> thePathThatSucks)
    {
        if (thePathThatSucks.Count <= Energy)
        {
            actualTarget = target;
            return thePathThatSucks;
        }

        //Debug.Log("Path length: " + thePathThatSucks.Count);
        Stack<Vector2Int> thePathThatRocks = Utilities.ReverseStack(thePathThatSucks);
        while (thePathThatRocks.Count > Energy)
        {
            thePathThatRocks.Pop();
        }

        actualTarget = thePathThatRocks.Peek();
        //pos = actualTarget;

        thePathThatRocks = Utilities.ReverseStack(thePathThatRocks);


        //Debug.Log("Path length: " + thePathThatRocks.Count);
        return thePathThatRocks;
    }

    IEnumerator SelectingDestinationCoroutine()
    {
        done = true;

        yield return new WaitForSeconds(1.0f);

        //check if there are any player units left
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag("PlayerUnit");
            if (targetObjects.Length == 0)
            {
                state = UnitState.EndingPhase;
                done = false; //flag recycling
                yield break;
            }

            //check if there are adjacent player units
            combatTarget = GetCombatTarget();
            if (combatTarget != null)
            {
                state = UnitState.SelectingTarget;
                done = false; //flag recycling
                yield break;
            }

            target = FindNearestValidTagetTile();
            //Debug.Log("target :" + target.x + "," + target.y);
            var tempPath = cursor.FindPath(pos, target);
            path = GetThePathThatRocks(tempPath);
            path = cursor.FindPath(pos, actualTarget);
            pos = actualTarget;
            //Debug.Log("actaul target :" + actualTarget.x + "," + actualTarget.y);

            state = UnitState.Moving;
            done = false; //flag recycling
            yield break;
    }

    IEnumerator AwaitingChoiceCoroutine()
    {
        done = true;

        combatTarget = GetCombatTarget();
        if (combatTarget != null)
            TurnToFace(combatTarget.pos);

        yield return new WaitForSeconds(1.0f);

        state = UnitState.SelectingTarget;
        done = false; //flag recycling
        yield break;
    }

    IEnumerator AttackingCoroutine()
    {
        done = true;
        yield return new WaitForSeconds(1.0f);
        state = UnitState.EndingPhase;
        done = false;
        yield break;
    }
}
