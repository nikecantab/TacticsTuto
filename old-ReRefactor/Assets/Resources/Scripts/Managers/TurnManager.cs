using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    static Dictionary<string, Queue<Unit>> units = new Dictionary<string, Queue<Unit>>();
    public static Queue<string> turnKey = new Queue<string>();
    public static State gameState;
    public State visibleGameState;

    // Use this for initialization
    void Start()
    {
        StartTurn();
    }

    // Update is called once per frame
    void Update()
    {
        gameState = units[turnKey.Peek()].Peek().state;
        visibleGameState = gameState;
    }
    static void StartTurn()
    {
        //check if team has been wiped
        if (units[turnKey.Peek()].Count < 1)
        {
            Debug.Log("Game Over!");
        }

        //Check if unit has been killed
        if (!units[turnKey.Peek()].Peek().gameObject.activeSelf)
        {
            Debug.Log("unit is null");
            units[turnKey.Peek()].Dequeue();
        }

        units[turnKey.Peek()].Peek().BeginTurn();
    }

    public void EndTurnFromButton()
    {
        EndTurn();
    }

    public static void EndTurn()
    {
        //cycle queue
        Unit unit = units[turnKey.Peek()].Dequeue();
        units[turnKey.Peek()].Enqueue(unit);
        unit.EndTurn();

        GridManager.UpdateAll();

        //cycle teams
        string team = turnKey.Dequeue();
        turnKey.Enqueue(team);
        StartTurn();
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
}
