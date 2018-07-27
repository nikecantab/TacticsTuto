using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static Dictionary<string, List<Unit>> units;

    //public?
    List<string> currentTurnOrder;
    List<string> nextTurnOrder;

    void CreateTurnList(string nameA, string nameB, out List<string> turnOrderList)
    {
        turnOrderList = new List<string>();
        List<Unit> teamA = units[nameA];
        List<Unit> teamB = units[nameB];

        int countA = teamA.Count; int countB = teamB.Count;

        int highestCount = countA > countB ? countA : countB;

        //Shuffle
        for (int i = 0; i < highestCount*2; i++)
        {
            if (teamA.Count > 0 && teamB.Count > 0)
            {
                if (Random.Range(0f, 1f)>0.5f)
                {
                    turnOrderList.Add(nameA);
                    teamA.RemoveAt(0);
                }
                else
                {
                    turnOrderList.Add(nameB);
                    teamB.RemoveAt(0);
                }
            }
            else if (teamA.Count > 0)
            {
                turnOrderList.Add(nameA);
                teamA.RemoveAt(0);
            }
            else if (teamB.Count > 0)
            {
                turnOrderList.Add(nameB);
                teamB.RemoveAt(0);
            }
        }
    }

    public static void RemoveUnit(Unit unit)
    {
        if (units[unit.tag].Contains(unit))
        {
            units[unit.tag].Remove(unit);
        }
    }

    //Subscriber Pattern
    public static void AddUnit(Unit unit)
    {
        List<Unit> unitList;
        //error prevention: has team been added to dictionary?
        if (!units.ContainsKey(unit.tag))
        {
            #region //Add to dictionary
            /*good to know: 
            adding a list to a dictionary and modifying it later 
            will update the queue in the dictionary*/
            #endregion
            unitList = new List<Unit>();
            units[unit.tag] = unitList;
        }
        else
        {
            //get the list from the dictionary
            unitList = units[unit.tag];
        }
        //add the unit to the list in the dictionary
        unitList.Add(unit);
    }

}
/*
    static Dictionary<string, Queue<Unit>> units = new Dictionary<string, Queue<Unit>>();
    public static Queue<string> turnKey = new Queue<string>();
    //public static State gameState;
    //public State visibleGameState;

    // Use this for initialization
    void Start()
    {
        StartTurn();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    gameState = units[turnKey.Peek()].Peek().state;
    //    visibleGameState = gameState;
    //}
    static void StartTurn()
    {
        //check if team has been wiped
        if (units[turnKey.Peek()].Count < 1)
        {
            Debug.Log("Game Over!");
        }

        //Check if unit has been killed - glitched
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
        //error prevention: has team been added to dictionary?
        if (!units.ContainsKey(unit.tag))
        {
            //Add to dictionary
            /*good to know: 
            adding a queue to a dictionary and modifying it later 
            will update the queue in the dictionary
queue = new Queue<Unit>();
            units[unit.tag] = queue;

            if (!turnKey.Contains(unit.tag))
            {
                turnKey.Enqueue(unit.tag);
            }
        }
        else
        {
            //get the queue from the dictionary
            queue = units[unit.tag];
        }
        //add the unit to the queue in the dictionary
        queue.Enqueue(unit);
    }
    */
      