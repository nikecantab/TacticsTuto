using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    static Dictionary<string, List<Unit>> units = new Dictionary<string, List<Unit>>();
    public static Queue<string> turnKey = new Queue<string>();
    static Queue<Unit> turnTeam = new Queue<Unit>();

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (turnTeam.Count == 0)
        {
            InitTeamQueue();
        }
	}

    static void InitTeamQueue()
    {
        List<Unit> teamList = units[turnKey.Peek()];

        foreach (Unit unit in teamList)
        {
            turnTeam.Enqueue(unit);
        }

        StartTurn();
    }

    static void StartTurn()
    {
        if (turnTeam.Count > 0)
        {
            turnTeam.Peek().BeginTurn();
        }
    }

    public static void EndTurn()
    {
        Unit unit = turnTeam.Dequeue();
        unit.EndTurn();

        //first unit of next team starts turn
        if (turnTeam.Count > 0)
        {
            StartTurn();
        }
        else
        {
            //Loop the team queue
            string team = turnKey.Dequeue();
            turnKey.Enqueue(team);
            InitTeamQueue();
        }
    }

    //Subscriber Pattern
    public static void AddUnit(Unit unit)
    {
        List<Unit> list;
        //error prevention: has unit been added to dictionary?
        if (!units.ContainsKey(unit.tag))
        {
            //Add to dictionary
            list = new List<Unit>();
            units[unit.tag] = list;

            if (!turnKey.Contains(unit.tag))
            {
                turnKey.Enqueue(unit.tag);
            }
        }
        else
        {
            //get the list from the dictionary
            list = units[unit.tag];
        }
        //add the unit to the list in the dictionary
        list.Add(unit); 
    }

    public static void RemoveUnit(Unit unit)
    {
        List<Unit> list;

        list = units[unit.tag];

        list.Remove(unit);

        if (units[unit.tag].Count > 0)
            units[unit.tag] = list;
        else
        {
            //team wipe

        }
    }
}
