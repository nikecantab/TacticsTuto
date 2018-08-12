using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour {
    public Vector2Int pos;
    public GridManager gridManager;
    public TileManager tileManager;
    public Unit hoverUnit;
    public Unit selectedUnit;
    public CursorState state = CursorState.SelectingUnit;
    public GameObject hoverStats;
    public GameObject cornerStats;

    public int testRange;

    Vector3 lastPos;
    bool canClick;

    Vector2Int savedPos;

    GameObject actionPanel;
    Object hoverActions;
    

    private void Start()
    {
        cornerStats.SetActive(false);

        hoverActions = Resources.Load("Prefabs/GUI/HoverActions", typeof(GameObject));
    }

    // Update is called once per frame
    void Update ()
    {
        //this is the only way I could fix the hover panel remaining persistent when it shouldn't
        HoverPanelOff();

        switch (state)
        {
            case CursorState.SelectingUnit:
                #region Selecting Unit
                MoveCursor();

                ///TILE VISIBILITY
                //Activate/Deactivate Tiles
                if (hoverUnit != null)
                //get the hover unit's range
                {
                    gridManager.DeactivateAllTiles(gridManager.greenTiles);
                    gridManager.DeactivateAllTiles(gridManager.redTiles);
                    gridManager.ActivateAroundTile(gridManager.greenTiles, hoverUnit.Energy, hoverUnit.pos);
                    gridManager.ActivateFrontierTiles(gridManager.greenTiles, gridManager.redTiles, hoverUnit.Energy);

                    ///HOVER STAT PANEL ON
                    HoverPanelOn();
                }
                else
                {
                    gridManager.DeactivateAllTiles(gridManager.greenTiles);
                    gridManager.DeactivateAllTiles(gridManager.redTiles);
                    
                }

                ///UNIT SELECTION
                if (Input.GetMouseButtonUp(0) && canClick)
                {
                    //selecting the hoverunit: the else if prevents from walking on occupied tiles
                    if (hoverUnit != null)
                    {
                        if (hoverUnit.turn)
                        {
                            selectedUnit = hoverUnit;
                            state = CursorState.SelectingDestination;
                            savedPos = selectedUnit.pos;

                            ///HOVER STAT PANEL OFF
                            HoverPanelOff();

                            ///CORNER PANEL ON
                            cornerStats.SetActive(true);
                            cornerStats.GetComponent<StatPanelManager>().SetUnit(selectedUnit);
                        }
                    }
                }

                break;
            #endregion
            case CursorState.SelectingDestination:
                #region Selecting Destination
                MoveCursor();


                ///TILE VISIBILITY
                //Activate/Deactivate Tiles
                if (hoverUnit != null)
                //get the hover unit's range
                {
                    gridManager.DeactivateAllTiles(gridManager.greenTiles);
                    gridManager.DeactivateAllTiles(gridManager.redTiles);
                    gridManager.ActivateAroundTile(gridManager.greenTiles, hoverUnit.Energy, hoverUnit.pos);
                    gridManager.ActivateFrontierTiles(gridManager.greenTiles, gridManager.redTiles, hoverUnit.Energy);

                    if (hoverUnit != selectedUnit)
                    {
                        ///HOVER STAT PANEL ON
                        HoverPanelOn();
                    }
                }
                else if (selectedUnit != null)
                //if a character is already selected, revert the range to them
                {
                    gridManager.DeactivateAllTiles(gridManager.greenTiles);
                    gridManager.DeactivateAllTiles(gridManager.redTiles);
                    gridManager.ActivateAroundTile(gridManager.greenTiles, selectedUnit.Energy, selectedUnit.pos);
                    gridManager.ActivateFrontierTiles(gridManager.greenTiles, gridManager.redTiles, selectedUnit.Energy);

                    ///HOVER STAT PANEL OFF
                    HoverPanelOff();
                }
                else
                {
                    //this shouldn't happen
                    Debug.Log("SelectingDestination: invalid condition");
                }


                ///UNIT MOVEMENT
                if (Input.GetMouseButtonUp(0) && canClick)
                {
                    //Move Unit
                    if (selectedUnit != null)
                    {
                        //Limit movement to range
                        if (gridManager.greenTiles[pos.x, pos.y].activeSelf && tileManager.GetUnit(pos) == null)
                        {
                            //Move
                            Stack<Vector2Int> path = FindPath(selectedUnit.pos, pos);
                            selectedUnit.path = path;
                            selectedUnit.state = UnitState.Moving;
                            selectedUnit.pos = pos;
                        }
                        else if (tileManager.GetUnit(pos) == selectedUnit)
                        {
                            //select own tile
                            selectedUnit.state = UnitState.AwaitingChoice;

                            ///HOVER STAT PANEL OFF
                            HoverPanelOff();
                        }
                    }
                }
                //Cancel selection
                else if (Input.GetMouseButtonUp(1))
                {
                    if (selectedUnit != null)
                    {
                        selectedUnit = null;
                        state = CursorState.SelectingUnit;
                    }
                }

                break;
            #endregion
            case CursorState.AwaitingChoice:
                #region Awaiting Choice

                //cancel
                if (Input.GetMouseButtonUp(1))
                {
                    if (selectedUnit.state == UnitState.AwaitingChoice)
                    {
                        selectedUnit.pos = savedPos;
                        selectedUnit.transform.localPosition = new Vector3(savedPos.x, selectedUnit.transform.localPosition.y, savedPos.y);
                        selectedUnit.state = UnitState.SelectingDestination;
                        state = CursorState.SelectingDestination;

                        var panel = GameObject.FindGameObjectWithTag("ActionPanel");

                        if (panel != null)
                            panel.GetComponent<ActionPanelManager>().ClosePanel();
                    }
                    else if (selectedUnit.state == UnitState.SelectingTarget)
                    {
                        state = CursorState.SelectingTarget; //probably unnecessary
                        var panel = GameObject.FindGameObjectWithTag("ActionPanel");

                        if (panel != null)
                            panel.GetComponent<ActionPanelManager>().ClosePanel();
                    }
                    return;
                }

                if (GameObject.FindGameObjectWithTag("ActionPanel") != null)
                    return;
                
                ///ACTION PANEL ON
                var actionPanel = Instantiate(hoverActions) as GameObject;
                actionPanel.transform.SetParent(gameObject.transform.parent);
                var actionManager = actionPanel.GetComponent<ActionPanelManager>();
                //Debug.Log("creating panel");
                //panel 1
                if (selectedUnit.state == UnitState.AwaitingChoice)
                {
                    //Debug.Log("created action panel");
                    actionManager.UpdatePosition(selectedUnit);
                    actionManager.AddActionButton(ActionButton.Attack);
                    actionManager.AddActionButton(ActionButton.Wait);
                }
                else //panel 2
                {
                    //Debug.Log("created attack panel");
                    actionManager.UpdatePosition(selectedUnit.combatTarget); //change to taget unit

                    //get possible attacks
                    foreach(AttackType attack in selectedUnit.attackTypes)
                    {
                        switch (attack)
                        {
                            case AttackType.Cripple:
                                actionManager.AddActionButton(ActionButton.Cripple);
                                break;
                            case AttackType.Impair:
                                actionManager.AddActionButton(ActionButton.Impair);
                                break;
                            case AttackType.Weaken:
                                actionManager.AddActionButton(ActionButton.Weaken);
                                break;
                        }
                    }
                }

                break;
            #endregion
            case CursorState.SelectingTarget:
                #region Selecting Target
                MoveCursor();

                ///TILE VISIBILITY
                if (selectedUnit != null)
                //if a character is already selected, revert the range to them
                {
                    gridManager.DeactivateAllTiles(gridManager.greenTiles);
                    gridManager.DeactivateAllTiles(gridManager.redTiles);
                    gridManager.ActivateAroundTile(gridManager.greenTiles, 0, selectedUnit.pos); //can this be removed?
                    gridManager.ActivateFrontierTiles(gridManager.greenTiles, gridManager.redTiles, selectedUnit.Energy);
                }

                if (hoverUnit != null && hoverUnit!=selectedUnit)
                {
                    ///HOVER STAT PANEL ON
                    HoverPanelOn();
                }
                else
                {
                    ///HOVER PANEL OFF
                    HoverPanelOff();
                }

                ///UNIT SELECTION
                if (Input.GetMouseButtonUp(0) && canClick)
                {
                    if (selectedUnit != null)
                    {
                        if (gridManager.redTiles[pos.x, pos.y].activeSelf && tileManager.GetUnit(pos) != null)
                        {
                            //attacking
                            selectedUnit.combatTarget = hoverUnit;
                            //selectedUnit.state = UnitState.Attacking;
                            state = CursorState.AwaitingChoice;
                            //Debug.Log("cursor: awaiting choice");

                            ///HOVER STAT PANEL OFF
                            HoverPanelOff();
                        }
                    }
                }
                //Cancel selection
                else if (Input.GetMouseButtonUp(1))
                {
                    if (selectedUnit != null)
                    {
                        //TODO: cancelling;
                        selectedUnit.state = UnitState.AwaitingChoice;
                        state = CursorState.AwaitingChoice;
                    }
                }

                break;
            #endregion
            case CursorState.Inactive:
                #region Inactive
                gridManager.DeactivateAllTiles(gridManager.greenTiles);
                gridManager.DeactivateAllTiles(gridManager.redTiles);

                if (GetComponent<SpriteRenderer>().enabled)
                    GetComponent<SpriteRenderer>().enabled = false;
                HoverPanelOff();
                break;
                #endregion
        }


    }

    void HoverPanelOn()
    {
        var hoverCanvas = hoverStats.GetComponent<Canvas>();
        var hoverManager = hoverStats.GetComponent<StatPanelManager>();

        hoverCanvas.enabled = true;
        hoverManager.SetUnit(hoverUnit);
        hoverManager.UpdatePosition(hoverUnit);

    }

    void HoverPanelOff()
    {
        var hoverCanvas = hoverStats.GetComponent<Canvas>();
        hoverCanvas.enabled = false;
    }

    public void ResetCursor()
    {
        if (selectedUnit != null)
            selectedUnit = null;
        if (hoverUnit != null)
            selectedUnit = null;
    }
    
    void MoveCursor()
    {
        if (!GetComponent<SpriteRenderer>().enabled)
            GetComponent<SpriteRenderer>().enabled = true;
        Vector3 mousePos = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var gridLayer = 1 << LayerMask.NameToLayer("Grid");

        ///CURSOR POSITION
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridLayer))
        //mouse is within grid
        {
            //transform.localPosition = new Vector3(hit.transform.localPosition.x, transform.localPosition.y, hit.transform.localPosition.z);
            //Debug.Log(string.Format("mousex: {0}, mousey: {1}", hit.point.x, hit.point.z));
            pos.x = Mathf.FloorToInt(hit.point.x) - (int)transform.parent.position.x;
            pos.y = Mathf.FloorToInt(hit.point.z) - (int)transform.parent.position.z;
            pos.x = pos.x < 0 ? 0 : pos.x;
            pos.y = pos.y < 0 ? 0 : pos.y;
            transform.localPosition = new Vector3(pos.x, transform.localPosition.y, pos.y);
            lastPos = transform.localPosition;
            canClick = true;
        }
        else
        //mouse outside of grid: get last position
        {
            transform.localPosition = lastPos;
            pos.x = (int)lastPos.x;
            pos.y = (int)lastPos.z;
            canClick = false;
        }


        //Get the HoverUnit
        if (tileManager.GetUnit(pos) != null)
        {
            hoverUnit = tileManager.GetUnit(pos);
        }
        else hoverUnit = null;

    }


    /// PATHFINDING
    public Stack<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        int gridSize = GameObject.Find("Managers").GetComponent<GridManager>().gridSize;
        var invalidPos = new Vector2Int(-1, -1);

        Stack<Vector2Int> path = new Stack<Vector2Int>();
        float[,] g = new float[gridSize, gridSize];
        float[,] f = new float[gridSize, gridSize];
        Vector2Int[,] parent = new Vector2Int[gridSize, gridSize];

        for (var x = 0; x < gridSize; x++)
        {
            for (var y = 0; y < gridSize; y++)
            {
                //alt: instance of tile class
                // new tile = tile(x,y);
                g[x, y] = Mathf.Infinity;
                f[x, y] = Mathf.Infinity;
                parent[x, y] = invalidPos;
            }
        }

        g[start.x, start.y] = 0;

        //a star get range
        List<Vector2Int> closed = new List<Vector2Int>();
        PriorityQueue<Vector2Int> open = new PriorityQueue<Vector2Int>();
        open.Clear();
        open.Add(start, 0);

        //Add blocked tiles to closed list
        
        while (open.Count > 0)
        {
            //var lowestScore = open.GetPriorityMin();
            var currentTile = open.GetElementPriorityMin();
            float currentTileG = g[currentTile.x, currentTile.y];
            open.Sort();
            //check if we reached target
            //Debug.Log("current: " + currentTile.x + "," + currentTile.y + " end: " + end.x + "," + end.y);
            if (currentTile==end)
            {
                //start path
                //Debug.Log("path end reached");
                //while parent[lowestx, lowesty] != null
                var lowest = currentTile;
                path.Push(lowest);

                while(parent[lowest.x, lowest.y] != invalidPos)
                {
                    var next = parent[lowest.x, lowest.y];
                    lowest = next;
                    //Debug.Log("lowest: " + lowest.x + "," + lowest.y);
                    path.Push(lowest);
                }
                //lowest = camefrom[lowestx, lowesty]
                //stack.add(lowest)

                return path;
            }

            open.Remove(open.GetElementPriorityMin());
            closed.Add(currentTile);

            //Debug.Log("count 2nd" + open.Count);

            //Handle neighbors
            for (var i = 0; i < 4; i++)
            {
                var neighbor = GetNeighbor(currentTile, i);
                //Debug.Log("current neighbor: " + neighbor.x + "," + neighbor.y);
                if (neighbor == invalidPos)
                    continue;

                //check if it's in closed
                if (closed.Contains(neighbor))
                    continue;

                float tentativeG = currentTileG + 1; //can add terrain costs here
                
                //if neighbor is not in open, add to open queue
                if (!open.Contains(neighbor))
                {
                    open.Add(neighbor, tentativeG);
                }
                else if (tentativeG >= currentTileG)
                    continue;


                parent[neighbor.x, neighbor.y] = currentTile;
                g[neighbor.x, neighbor.y] = tentativeG;
                f[neighbor.x, neighbor.y] = tentativeG + GetHeuristic(neighbor, end); //F = G + H
                //Debug.Log("neighbor :" + neighbor.x + "," + neighbor.y + " G: " + tentativeG + " F: " + Mathf.Round(f[neighbor.x, neighbor.y]));

                open.SetPriority(neighbor, f[neighbor.x, neighbor.y]);
            }
            //Debug.Log("count 3rd" + open.Count);
        }
        return path;
    }

    Vector2Int GetNeighbor(Vector2Int coords, int loop)
    {
        int gridSize = GameObject.Find("Managers").GetComponent<GridManager>().gridSize;
        var invalidPos = new Vector2Int(-1, -1);

        switch (loop)
        {
            case 0:
                if (coords.x + 1 < gridSize && coords.y < gridSize)
                    return new Vector2Int(coords.x + 1, coords.y);
                return invalidPos;
            case 1:
                if (coords.x < gridSize && coords.y - 1 >= 0)
                    return new Vector2Int(coords.x, coords.y - 1);
                return invalidPos;
            case 2:
                if (coords.x - 1 >= 0 && coords.y < gridSize)
                    return new Vector2Int(coords.x - 1, coords.y);
                return invalidPos;
            case 3:
                if (coords.x < gridSize && coords.y + 1 < gridSize)
                    return new Vector2Int(coords.x, coords.y + 1);
                return invalidPos;
            default:
                return invalidPos;
        }
    }

    float GetHeuristic(Vector2Int from, Vector2Int to)
    {
        var dx = Mathf.Abs(from.x - to.x);
        var dy = Mathf.Abs(from.y - to.y);
        return dx + dy;
    }
}

/*
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var ignoreEntities = 1 << 8;
            ignoreEntities = ~ignoreEntities;

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ignoreEntities))
                 if (hit.collider.tag == "Tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();

                    //state 
                    if (state == State.SelectingMoveTarget)
                    {
                        if (t.current)
                        {
                            state = State.SelectingActionTarget;
                            done = false;
                        }
                        else if (t.selectable)
                        {
                            MoveToTile(t);
                            done = false;
                        }
                    }
                    else if (state == State.SelectingActionTarget)
                    {
                        if (t.enemyOccupied)
                        {
                            //Attack
                            combatTarget = t.CheckEnemyOccupied(t.gridCoord);
                            state = State.Attacking;
                            done = false;
                        }
                        else if (t.current)
                            TurnManager.EndTurn();
                    }

                }*/
