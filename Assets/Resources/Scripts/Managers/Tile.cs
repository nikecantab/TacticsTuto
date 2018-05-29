using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public bool walkable = true;
    public bool current = false;
    public bool target = false;
    public bool selectable = false;
    public bool occupied = false;
    public bool enemyOccupied = false;
    public bool inAttackRange = false;
    public Vector2 gridCoord;

    public List<Tile> adjacencyList = new List<Tile>();

    //needed bfs (breath first search)
    public bool visited = false;
    public Tile parent = null;
    public int distance = 0;

    //For A*
    public float f = 0;
    public float g = 0;
    public float h = 0;

    // Use this for initialization
    void Start ()
    {
        gridCoord = new Vector2(transform.localPosition.x, transform.parent.localPosition.z);
        GridManager.AddTile(this, gridCoord);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (inAttackRange)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }

        if (current)
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }

        if (enemyOccupied)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public void Reset()
    {
        adjacencyList.Clear();

        walkable = true;
        current = false;
        target = false;
        selectable = false;
        enemyOccupied = false;
        occupied = false;

        adjacencyList = new List<Tile>();

        //needed bfs (breath first search)
        visited = false;
        parent = null;
        distance = 0;

        f = g = h = 0;
    }

    public void FindNeighbors(Tile target)
    {
        Reset();

        CheckTile(Vector2.right, target);
        CheckTile(-Vector2.right, target);
        CheckTile(Vector2.up, target);
        CheckTile(-Vector2.up, target);

    }

    public void CheckTile(Vector2 direction, Tile target) 
    {
        Vector2 newCoord = gridCoord + direction;
        if (newCoord.x < 0 || newCoord.x >= GridManager.gridSize || newCoord.y < 0 || newCoord.y >= GridManager.gridSize)
            return;
        Tile tile = GridManager.tilesGrid[(int)newCoord.x, (int)newCoord.y];

        if (tile.walkable)
        {
            adjacencyList.Add(tile);

            if (GridManager.CheckIfOccupied(newCoord))
                tile.occupied = true;
        }
    }
    
    public Unit CheckEnemyOccupied()
    {
        Unit enemy = null;
        if (GridManager.CheckIfOccupied(gridCoord))
        {
            var occupant = GridManager.unitsGrid[(int)gridCoord.x, (int)gridCoord.y];
            if (occupant.gameObject.tag != TurnManager.turnKey.Peek())
            {
                enemyOccupied = true;
                enemy = occupant;
            }

        }
        return enemy;
    }
    

}
