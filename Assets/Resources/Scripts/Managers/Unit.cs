using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool turn = false;
    protected bool done = false;

    List<Tile> selectableTiles = new List<Tile>();
    GameObject[] tiles;

    Stack<Tile> path = new Stack<Tile>();
    Tile currentTile;
    
    public State state = State.SelectingMoveTarget;

    public Vector2 gridCoord;
    public float moveSpeed = 2;

    public int Energy = 5;
    public int Strength = 1;
    public int Range = 1;

    public int remainingMove;

    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();

    float halfHeight = 0;

    public Tile actualTargetTile;
    
    SpriteFaceCamera spriteFaceCamera;

    protected Unit combatTarget;

    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        halfHeight = GetComponent<Collider>().bounds.extents.y;

        TurnManager.AddUnit(this);
        GridManager.AddUnit(this, new Vector2(transform.localPosition.x, transform.localPosition.z));

        spriteFaceCamera = GetComponent<SpriteFaceCamera>();

        remainingMove = Energy;
    }

    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }

    //get coordinates of specific tile
    public Tile GetTargetTile(GameObject target)
    {
        Tile tile = null;
        GridManager.GetTileBeneathUnit(target.GetComponent<Unit>(), out tile);
        //GridManager.GetTileAtCoord(new Vector2(target.transform.localPosition.x, target.transform.parent.localPosition.z), out tile);
        return tile;
    }


    public void ComputeAdjacencyList(Tile target)
    {
        foreach (GameObject tile in tiles)
        {
            //have every tile check its neighbors and add them to its adjacency list
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(target);
        }
    }

    public void FindSelectableTiles()
    {
        ComputeAdjacencyList(null);
        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue(currentTile); //add first tile to queue
        currentTile.visited = true;
        //currentTile.parent = null;

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();

            selectableTiles.Add(t);
            if (t.distance <= remainingMove)
            {
                if (!t.occupied)
                    t.selectable = true;
            }
            else
            {
                t.inAttackRange = true;
            }

            if (t.occupied)
            {
                var enemy = t.CheckEnemyOccupied(t.gridCoord);
                if (enemy != null)
                    t.enemyOccupied = true;
            }

            if (t.distance < remainingMove + Range)
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
        GridManager.UpdateTilesGrid();
    }
    
    //change to a*
    public void MoveToTile(Tile tile)
    {
        path.Clear();
        tile.target = true;
        state = State.Moving;

        Tile next = tile;
        while (next != null)
        {
            path.Push(next);

            //todo: check if parent is occupied, if it is, figure out a way around
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
                GridManager.UpdateUnitPosition(this, new Vector2(t.transform.localPosition.x, t.transform.parent.localPosition.z));
                path.Pop();
                
                remainingMove--;
            }
        }
        else
        {
            RemoveSelectedTiles();
            //Debug.Log("resetting tiles");
            state = State.SelectingActionTarget;
            done = false;
        }
    }

    protected void RemoveSelectedTiles()
    {
        if (currentTile != null)
        {
            //currentTile.current = false;
            //currentTile.target = false;
            currentTile.Reset();
            currentTile = null;
        }

        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.Reset();
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
            if (t.f < lowest.f && !t.occupied)
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
        while (next != null && next.occupied == false)
        {
            tempPath.Push(next);
            next = next.parent;
        }

        if (tempPath.Count < Energy)
        {
            return t.parent;
        }

        Tile endTile = null;
        for(int i = 0; i < Energy; i++)
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
            //Debug.Log(string.Format("openlist tile coord: x{0} y{1}", t.gridCoord.x, t.gridCoord.y));
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
        remainingMove = Energy;
        GridManager.UpdateUnitPosition(this, new Vector2(transform.localPosition.x, transform.localPosition.z));
        state = State.SelectingMoveTarget;
    }

    public void EndTurn()
    {
        turn = false;
        done = false;
        GridManager.UpdateUnitPosition(this, new Vector2(transform.localPosition.x, transform.localPosition.z));
        //Debug.Log(string.Format("Ended turn pos: x{0}, y{1}", transform.localPosition.x, transform.localPosition.z));
        //Debug.Log(string.Format("Ended turn coord: x{0}, y{1}", gridCoord.x, gridCoord.y));
    }

    protected bool CheckIfDead()
    {
        if (Energy < 1)
        {
            TurnManager.RemoveUnit(this);
            GridManager.ResetOccupied(gridCoord);
            //EndTurn();
            Tile tile;
            GridManager.GetTileAtCoord(gridCoord, out tile);
            tile.Reset();

            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
