using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
     Unit[,] units;
     TileObject[,] tiles; //change to basetile?

    void Awake()
    {
        int gridSize = GameObject.Find("Managers").GetComponent<GridManager>().gridSize;

        tiles = new TileObject[gridSize, gridSize];
        units = new Unit[gridSize, gridSize];
    }

    public void AddTile(TileObject tile, Vector2Int coordinates)
    {
        //Debug.Log(coordinates.x + ", " + coordinates.y);
        tiles[coordinates.x, coordinates.y] = tile;
    }

    public void AddUnit(Unit unit, Vector2Int coordinates)
    {
        units[coordinates.x, coordinates.y] = unit;
    }

    public void ChangePos(Unit unit, Vector2Int previousPos, Vector2Int newPos)
    {
        units[previousPos.x, previousPos.y] = null;
        units[newPos.x, newPos.y] = unit;
    }

    public void RemoveUnit(Vector2Int coordinates)
    {
        units[coordinates.x, coordinates.y] = null;
    }

    public TileObject GetTile (Vector2Int coordinates)
    {
        return tiles[coordinates.x, coordinates.y];
    }

    public Unit GetUnit(Vector2Int coordinates)
    {
        return units[coordinates.x, coordinates.y];
    }
}
