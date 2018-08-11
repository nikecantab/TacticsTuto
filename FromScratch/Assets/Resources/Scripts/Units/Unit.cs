using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public Vector2Int pos;
    Vector2Int lastPos;

    public bool turn = false;
    public bool destroy = false;
    protected bool done = false;

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
        Debug.Log("started turn: " + gameObject.tag);
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
        //turn this into a fade
        Destroy(gameObject, 0.1f);
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

                if (damage == 0 || Strength == 0)
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
} 
