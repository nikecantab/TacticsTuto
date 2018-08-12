using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTurnManager : MonoBehaviour {

    static List<Unit> units = new List<Unit>();
    static Unit currentUnit;

    public GameObject getArrow;
    static GameObject arrow;

	// Use this for initialization
	void Start ()
    {
        arrow = getArrow;
        //StartTurn();
	}

    public static void StartTurn()
    {
        currentUnit = units[0];
        StartPhase();
    }

    static void NextPhase()
    {
        int i = units.IndexOf(currentUnit);
        if (units.Count > i + 1)
        {
            currentUnit = units[i + 1];
            StartPhase();
        }
        else
        {
            StartTurn();
        }
    }

    static void StartPhase()
    {
        currentUnit.BeginTurn();

        arrow.transform.localPosition = new Vector3(currentUnit.pos.x, arrow.transform.localPosition.y, currentUnit.pos.y);
        
        var cursor = GameObject.Find("GUICursor").GetComponent<Cursor>();
        cursor.state = currentUnit.tag == "PlayerUnit" ? CursorState.SelectingUnit : CursorState.Inactive;
    }

    public static void EndPhase()
    {
        currentUnit.EndTurn();
        NextPhase();
    }

    //Subscriber Pattern
    public static void AddUnit(Unit unit)
    {
        units.Add(unit);
    }

    public static void RemoveUnit(Unit unit)
    {
        if (units.Contains(unit))
        {
            if (unit != currentUnit)
                units.Remove(unit);
            else
            {
                Debug.Log("turn manager error: trying to remove an active unit");
                NextPhase();
            }
            unit.Die();
        }
    }
}
