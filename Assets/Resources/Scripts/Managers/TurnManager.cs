using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    static Dictionary<string, List<TacticsMove>> units = new Dictionary<string, List<TacticsMove>>();
    static Queue<string> turnKey = new Queue<string>();
    static Queue<TacticsMove> turnTeam = new Queue<TacticsMove>();

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
        List<TacticsMove> teamList = units[turnKey.Peek()];

        foreach (TacticsMove unit in teamList)
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
        TacticsMove unit = turnTeam.Dequeue();
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
    public static void AddUnit(TacticsMove unit)
    {
        List<TacticsMove> list;

        //error prevention: has unit been added to dictionary?
        if (!units.ContainsKey(unit.tag))
        {
            //Add to dictionary
            list = new List<TacticsMove>();
            units[unit.tag] = list;

            if (!turnKey.Contains(unit.tag))
            {
                turnKey.Enqueue(unit.tag);
            }
        }
        else
        {
            list = units[unit.tag];
        }

        list.Add(unit); 
    }

    //void RemoveUnit()
}
