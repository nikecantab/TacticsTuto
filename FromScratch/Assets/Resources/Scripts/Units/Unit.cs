using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public Vector2Int pos;
    Vector2Int lastPos;

    public bool turn = false;
    public bool destroy = false;
    public bool done = false;

    public string Name = "Unit";

    public int Energy = 5;
    public int Strength = 1;
    public int Constitution = 1;
    public int Range = 1;

    public int MEnergy;
    public int MStrength;
    public int MConstitution;

    public UnitClass unitClass;
    public List<AttackType> attackTypes;
    public AttackType currentAttack;

    public float moveSpeed = 2;

    public UnitState state = UnitState.SelectingDestination;
    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();

    public Stack<Vector2Int> path = new Stack<Vector2Int>();

    SpriteFaceCamera spriteFaceCamera;
    TileManager tileManager;

    public Unit combatTarget;
    private GameObject floatingText;

    //GameObject[] tiles;
    //Tile currentTile;
    //public Vector2 gridCoord;

    protected void Init()
    {
        pos = new Vector2Int((int)transform.localPosition.x, (int)transform.localPosition.z);
        lastPos = pos;
        TempTurnManager.AddUnit(this);
        tileManager = GameObject.Find("Managers").GetComponent<TileManager>();
        tileManager.AddUnit(this, pos);
        
        spriteFaceCamera = GetComponent<SpriteFaceCamera>();
        // remainingMove = Energy;
        floatingText = Resources.Load("Prefabs/GUI/FloatingText") as GameObject;

        MEnergy = Energy;
        MStrength = Strength;
        MConstitution = Constitution;
    }
    
    /// MOVEMENT
    void CalculateHeading(Vector3 target)
    {
        heading = target - transform.localPosition;
        heading.Normalize();
        //Debug.Log("heading: " + heading.x + "," + heading.y + "," + heading.z);


        //figure out new direction 
        if (heading.z > 0)
        {
            spriteFaceCamera.FaceDirection(Facing.UpLeft);
        }
        else if (heading.x > 0)
        {
            spriteFaceCamera.FaceDirection(Facing.UpRight);
        }
        else if (heading.z < 0)
        {
            spriteFaceCamera.FaceDirection(Facing.DownRight);
        }
        else
        {
            spriteFaceCamera.FaceDirection(Facing.DownLeft);
        }
    }

    void SetHorizontalVelocity()
    {
        velocity = heading * moveSpeed;
    }

    protected void TurnToFace(Vector2Int target)
    {
        var direction = target - pos;
        heading.Normalize();
        //Debug.Log("heading: " + heading.x + "," + heading.y + "," + heading.z);


        //figure out new direction 
        if (direction.y > 0)
        {
            spriteFaceCamera.FaceDirection(Facing.UpLeft);
        }
        else if (direction.x > 0)
        {
            spriteFaceCamera.FaceDirection(Facing.UpRight);
        }
        else if (direction.y < 0)
        {
            spriteFaceCamera.FaceDirection(Facing.DownRight);
        }
        else
        {
            spriteFaceCamera.FaceDirection(Facing.DownLeft);
        }

    }

    public void Move()
    {
        if (path.Count > 0)
        {
            Vector2Int t = path.Peek();
            //Debug.Log("t: " + t.x + "," + t.y);
            Vector3 target = new Vector3(t.x, transform.localPosition.y, t.y);

            //calculate the unit's position on top of the target tile
            ///MOVE
            if (Vector3.Distance(transform.localPosition, target) >= 0.15f)
            {
                CalculateHeading(target);
                SetHorizontalVelocity();

                //Locomotion
                transform.forward = heading;
                transform.localPosition += velocity * Time.deltaTime;
            }
            else ///TILE DONE
            {
                //tile center reached
                transform.localPosition = target;
                //GridManager.UpdateUnitPosition(this, new Vector2(t.transform.localPosition.x, t.transform.parent.localPosition.z));
                path.Pop();

                //remainingMove--;
            }
        }
        else ///DONE MOVING
        {
            //Debug.Log("done moving");
            //state = UnitState.SelectingActionTarget;
            state = UnitState.AwaitingChoice;
        }
    }

    public void UpdatePosition()
    {
        tileManager.ChangePos(this, lastPos, pos);
        lastPos = pos;
    }

    /// TURN MANAGEMENT
    public void BeginTurn()
    {
        turn = true;
        //remainingMove = Energy;
        //GridManager.UpdateUnitPosition(this, new Vector2(transform.localPosition.x, transform.localPosition.z));
        //state = State.SelectingMoveTarget;
        //Debug.Log("started turn: " + gameObject.tag);
    }

    public void EndTurn()
    {
        turn = false;
        done = false;
        //GridManager.UpdateUnitPosition(this, new Vector2(transform.localPosition.x, transform.localPosition.z));
        //Debug.Log(string.Format("Ended turn pos: x{0}, y{1}", transform.localPosition.x, transform.localPosition.z));
        //Debug.Log(string.Format("Ended turn coord: x{0}, y{1}", gridCoord.x, gridCoord.y));
    }

    public bool CheckIfDead() //should enemy check this?
    {
        if (Energy < 1)
        {
            //GridManager.ResetOccupied(gridCoord);
            //EndTurn();
            //Tile tile;
            //GridManager.GetTileAtCoord(gridCoord, out tile);
            //tile.Reset();

            //gameObject.SetActive(false);
            tileManager.RemoveUnit(pos);
            TempTurnManager.RemoveUnit(this);

            return true;
        }
        return false;
    }

    public void RemoveUnit() 
    {
        tileManager.RemoveUnit(pos);
        TempTurnManager.RemoveUnit(this);
    }

    public void Die()
    {
        //TODO: turn this into a fade
        Destroy(gameObject, 0.1f);
        GameObject.Find("Managers").GetComponent<GameManager>().CheckWinLose();
    }
    
    public void TakeDamage(AttackType type, int damagingStat)
    {
        int damage; 
        switch(type)
        {
            ///ENERGY DAMAGE
            case AttackType.Cripple:
                damage = damagingStat - Constitution; //=strength
                if (damage < 0)
                    damage = 0;
                
                if (damage == 0)
                {
                    ShowFloatingText("NO DAMAGE", Color.green);
                }
                else
                {
                    Energy -= damage;
                    ShowFloatingText("-" + damage.ToString(), Color.green);
                }

                CheckIfDead();

                break;
            ///STRENGTH DAMAGE
            case AttackType.Impair:
                damage = damagingStat - Constitution; //=energy
                //Debug.Log("Impair: " + Strength + "STR - DMG(" + Constitution + "CON - " + damagingStat)
                if (damage < 0)
                    damage = 0;

                if (damage == 0 || Strength == 1)
                {
                    ShowFloatingText("NO DAMAGE", Color.red);
                }
                else
                {
                    Strength -= damage;
                    if (Strength < 0)
                        Strength = 0;
                    ShowFloatingText("-" + damage.ToString(), Color.red);
                }

                break;

            ///CONSTITUTION DAMAGE
            case AttackType.Weaken:
                damage = damagingStat - Strength; //=energy
                if (damage < 0)
                    damage = 0;

                if (damage == 0 || Constitution == 0)
                {
                    ShowFloatingText("NO DAMAGE", Color.blue);
                }
                else
                {
                    Constitution -= damage;
                    if (Constitution < 0)
                        Constitution = 0;
                    ShowFloatingText("-" + damage.ToString(), Color.blue);
                }

                break;
        }
    }

    public void ShowFloatingText(string text, Color col)
    {
        var go = Instantiate(floatingText, transform.position, Quaternion.identity, transform);
        var textMesh = go.GetComponent<TextMesh>();
        textMesh.text = text;
        textMesh.color = col;
    }


    protected Vector2Int FindNearestValidTagetTile()
    {
        var invalidPos = new Vector2Int(-1, -1);

        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag("PlayerUnit");
        if (targetObjects.Length == 0)
        {
            state = UnitState.EndingPhase;
        }
        List<Unit> playerUnits = new List<Unit>();
        foreach (GameObject target in targetObjects)
        {
            playerUnits.Add(target.GetComponent<Unit>());
        }

        List<Vector2Int> open = new List<Vector2Int>();
        List<Vector2Int> closed = new List<Vector2Int>();
        List<Vector2Int> unoccupiedTiles = new List<Vector2Int>();


        ///GET ALL ADJACENT TILES
        foreach (Unit unit in playerUnits)
        {
            for (var i = 0; i < 4; i++)
            {
                var neighbor = GetNeighbor(unit.pos, i);
                if (neighbor == invalidPos)
                    continue;
                
                if (!open.Contains(neighbor))
                    open.Add(neighbor);
            }
        }

        ///GET UNOCCUPIED TILES
        if (open.Count != 0)
        {
            foreach (Vector2Int tile in open)
            {
                if (tileManager.GetUnit(tile) == null)
                {
                    unoccupiedTiles.Add(tile);
                }
            }

            ///GET NEAREST
            if (unoccupiedTiles.Count != 0)
            {
                return GetNearest(unoccupiedTiles);
            }
            else
            ///NO VALID TILES?
            {
                while (open.Count > 0)
                {
                    List<Vector2Int> tempOpen = open;

                    ///GET ALL ADJACENT TILES AGAIN
                    foreach (Vector2Int tile in tempOpen)
                    {
                        open.Remove(tile);
                        for (var i = 0; i < 4; i++)
                        {
                            var neighbor = GetNeighbor(tile, i);

                            if (tileManager.GetUnit(tile) == null)
                            {
                                unoccupiedTiles.Add(tile);
                                continue;
                            }

                            if (neighbor == invalidPos)
                                continue;

                            if (!open.Contains(neighbor) && !closed.Contains(neighbor))
                                open.Add(neighbor);
                        }
                    }

                    ///GET NEAREST
                    if (unoccupiedTiles.Count != 0)
                    {
                        return GetNearest(unoccupiedTiles);
                    }
                }
            }

        }

        //if all else fails
        return pos;
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

    Vector2Int GetNearest(List<Vector2Int> tiles)
    {
        var nearest = pos;
        float distance = Mathf.Infinity;

        foreach (Vector2Int tile in tiles)
        {
            float d = Vector2Int.Distance(pos, tile);

            if (d < distance)
            {
                distance = d;
                nearest = tile;
            }
        }

        return nearest;
    }

    protected Unit GetCombatTarget()
    {
        var invalidPos = new Vector2Int(-1, -1);

        for (var i = 0; i < 4; i++)
        {
            var neighbor = GetNeighbor(pos, i);
            if (neighbor == invalidPos)
                continue;
            var unit = tileManager.GetUnit(neighbor);
            if (unit != null)
            {
                if (unit.tag == "PlayerUnit")
                    return unit;
            }
        }

        return null;
    }
} 
