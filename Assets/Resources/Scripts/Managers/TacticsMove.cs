using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    public bool turn = false;

    List<Tile> selectableTiles = new List<Tile>();
    GameObject[] tiles;

    Stack<Tile> path = new Stack<Tile>();
    Tile currentTile;

    public State state = State.SelectingMoveTarget;
    public float moveSpeed = 2;

    public int Energy = 5;
    public int Strength = 1;
    public int Range = 1;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();

    float halfHeight = 0;

    public Tile actualTargetTile;
    
    SpriteFaceCamera spriteFaceCamera;

    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        halfHeight = GetComponent<Collider>().bounds.extents.y;

        TurnManager.AddUnit(this);

        spriteFaceCamera = GetComponent<SpriteFaceCamera>();
    }

    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }

    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        //check the tile the target it standing on top of
        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }

    public void ComputeAdjacencyList(Tile target)
    {
        foreach (GameObject tile in tiles)
        {
            //have every tile check its neighbors
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(target);
        }
    }

    public void FindSelectableTiles()
    {
        ComputeAdjacencyList(null);
        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue(currentTile);
        currentTile.visited = true;
        //currentTile.parent = null;

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();

            selectableTiles.Add(t);
            if (!t.occupied)
                t.selectable = true;

            if (t.distance < Energy)
            {
                foreach (Tile tile in t.adjacencyList)
                {
                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);
                    }
                }
            }
            
        }
    }

    public void FindAttackableTiles()
    {
        ComputeAdjacencyList(null);
        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue(currentTile);
        currentTile.visited = true;
        //currentTile.parent = null;

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();

            selectableTiles.Add(t);

            t.CheckEnemyOccupied();

            if (t.distance < Range)
            {
                foreach (Tile tile in t.adjacencyList)
                {
                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);
                    }
                }
            }

        }
    }

    public void MoveToTile(Tile tile)
    {
        path.Clear();
        tile.target = true;
        state = State.Moving;

        Tile next = tile;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }

    public void Move()
    {
        if (path.Count > 0)
        {
            Tile t = path.Peek();
            Vector3 target = t.transform.position;

            //calculate the unit's position on top of the target tile
            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            if (Vector3.Distance(transform.position, target) >= 0.15f)
            {
                CalculateHeading(target);
                SetHorizontalVelocity();

                //Locomotion
                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else //DONE MOVING
            {
                //tile center reached
                transform.position = target;
                path.Pop();
            }
        }
        else
        {
            //check if there are enemies


            RemoveSelectedTiles();
            state = State.SelectingActionTarget;

            //selection mode
            //put endturn in the selection end function
            //TurnManager.EndTurn();
        }
    }

    protected void RemoveSelectedTiles()
    {
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }

        foreach (Tile tile in selectableTiles)
        {
            tile.Reset();
        }

        selectableTiles.Clear();
    }

    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();


        //figure out new direction 
        if (heading.z > 0)
        {
            spriteFaceCamera.FaceDirection(Facing.UpRight);
        }
        else if (heading.x > 0)
        {
            spriteFaceCamera.FaceDirection(Facing.DownRight);
        }
        else if (heading.z < 0)
        {
            spriteFaceCamera.FaceDirection(Facing.DownLeft);
        }
        else
        {
            spriteFaceCamera.FaceDirection(Facing.UpLeft);
        }
    }

    void SetHorizontalVelocity()
    {
        velocity = heading * moveSpeed;
    }

    protected Tile FindLowestF(List<Tile> list)
    {
        Tile lowest = list[0];

        foreach (Tile t in list)
        {
            if (t.f < lowest.f)
            {
                lowest = t;
            }
        }
        list.Remove(lowest);

        return lowest;
    }

    protected Tile FindEndTile(Tile t)
    {
        Stack<Tile> tempPath = new Stack<Tile>();

        Tile next = t.parent;
        while (next != null)
        {
            tempPath.Push(next);
            next = next.parent;
        }

        if (tempPath.Count <= Energy)
        {
            return t.parent;
        }

        Tile endTile = null;
        for(int i = 0; i <= Energy; i++)
        {
            //trace back steps, then stop when you reach the maximum extent
            endTile = tempPath.Pop();
        }

        return endTile;
    }

    //For efficiency's sake, making a priority queue struct would be better
    protected void FindPath(Tile target)
    {
        ComputeAdjacencyList(target);
        GetCurrentTile();

        List<Tile> openList = new List<Tile>();
        List<Tile> closedList = new List<Tile>();

        openList.Add(currentTile);
        //currentTile.parent = ??
        currentTile.h = Vector3.Distance(currentTile.transform.position, target.transform.position); //use SqrMagnitude for more efficiency
        currentTile.f = currentTile.h;

        while (openList.Count > 0)
        {
            Tile t = FindLowestF(openList);

            closedList.Add(t);

            if (t == target)
            {
                actualTargetTile = FindEndTile(t);
                MoveToTile(actualTargetTile);
                return;
            }

            foreach (Tile tile in t.adjacencyList)
            {
                if (closedList.Contains(tile))
                {
                    //do nothing, its already processed
                }
                else if (openList.Contains(tile))
                {
                    float tempG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    
                    if (tempG < tile.g)
                    {
                        tile.parent = t;

                        tile.g = tempG;
                        tile.f = tile.g + tile.h;
                    }

                }
                else
                {
                    tile.parent = t;
                    tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position);
                    tile.h = Vector3.Distance(tile.transform.position, target.transform.position);//eucledian distance, manhattan would normally be better...
                    tile.f = tile.g + tile.h;

                    openList.Add(tile);
                }
            }
        }

        //what if there is no path to target tile? what if target tile is occupied? find closest open tile using bfs
    }

    public void BeginTurn()
    {
        turn = true;
        state = State.SelectingMoveTarget;
    }

    public void EndTurn()
    {
        turn = false;
    }
}
