using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    static Dictionary<string, Queue<Unit>> units = new Dictionary<string, Queue<Unit>>();
    public static Queue<string> turnKey = new Queue<string>();
    //static Queue<Unit> turnTeam = new Queue<Unit>();
    public static State gameState;

    // Use this for initialization
    void Start ()
    {
        StartTurn();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //if (turnTeam.Count == 0)
        //      {
        //          InitTeamQueue();
        //      }
        gameState = units[turnKey.Peek()].Peek().state;//turnTeam.Peek().state;
        //Debug.Log("turnTeam: " + turnTeam.Count);
	}

    //static void InitTeamQueue()
    //{
    //    List<Unit> teamList = units[turnKey.Peek()];

    //    foreach (Unit unit in teamList)
    //    {
    //        turnTeam.Enqueue(unit);
    //    }

    //    StartTurn();
    //}

    static void StartTurn()
    {
        if (units[turnKey.Peek()].Peek() == null)
            units[turnKey.Peek()].Dequeue();
        units[turnKey.Peek()].Peek().BeginTurn();
        //if (turnTeam.Count > 0)
        //{
        //    turnTeam.Peek().BeginTurn();
        //}
    }

    public static void EndTurn()
    {
        Unit unit = units[turnKey.Peek()].Dequeue();
        units[turnKey.Peek()].Enqueue(unit);
        unit.EndTurn();

        GridManager.UpdateAll();

        string team = turnKey.Dequeue();
        turnKey.Enqueue(team);
        StartTurn();
        //first unit of next team starts turn
        //if (turnTeam.Count > 0)
        //{
        //    StartTurn();
        //}
        //else
        //{
        //    //Loop the team queue
        //    string team = turnKey.Dequeue();
        //    turnKey.Enqueue(team);
        //    InitTeamQueue();
        //}
    }

    //Subscriber Pattern
    public static void AddUnit(Unit unit)
    {
        Queue<Unit> queue;
        //error prevention: has unit been added to dictionary?
        if (!units.ContainsKey(unit.tag))
        {
            //Add to dictionary
            queue = new Queue<Unit>();
            units[unit.tag] = queue;

            if (!turnKey.Contains(unit.tag))
            {
                turnKey.Enqueue(unit.tag);
            }
        }
        else
        {
            //get the list from the dictionary
            queue = units[unit.tag];
        }
        //add the unit to the list in the dictionary
        queue.Enqueue(unit); 
    }

    public static void RemoveUnit(Unit unit)
    {
        
        //Queue<Unit> queue;

        //queue = units[unit.tag];
        
        //units[unit.tag] = queue;
        //queue.(unit);

        //if (queue.Count < 1)
        //{
        //    Debug.Log("Game Over!");
        //}


    }
}
