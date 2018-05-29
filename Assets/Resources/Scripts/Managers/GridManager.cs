using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridManager
{
    public static int gridSize = 12;
    public static Unit[,] unitsGrid = new Unit[gridSize, gridSize];
    public static Tile[,] tilesGrid = new Tile[gridSize, gridSize];
    
    public static void AddUnit(Unit unit, Vector2 coordinates)
    {
        unitsGrid[(int)coordinates.x, (int)coordinates.y] = unit;
    }

    public static void AddTile(Tile tile, Vector2 coordinates)
    {
        tilesGrid[(int)coordinates.x, (int)coordinates.y] = tile;
    }

    public static bool CheckIfOccupied(Vector2 coordinates)//, string maskTag = "", bool setFlag = false)
    {
        if (unitsGrid[(int)coordinates.x, (int)coordinates.y] != null)
        {
            return true;
        }
        return false;
    }

    public static void GetTileAtCoord(Vector2 coordinates, out Tile tile)
    {
        tile = tilesGrid[(int)coordinates.x, (int)coordinates.y];
    }

    public static void ResetOccupied(Vector2 coordinates)
    {
        unitsGrid[(int)coordinates.x, (int)coordinates.y] = null;
        tilesGrid[(int)coordinates.x, (int)coordinates.y].occupied = false;
    }

    public static void OccupyTile(Unit unit, Vector2 coordinates)
    {
        unitsGrid[(int)coordinates.x, (int)coordinates.y] = unit;
        tilesGrid[(int)coordinates.x, (int)coordinates.y].occupied = true;
    }

    public static void UpdateUnitPosition(Unit unit, Vector2 coordinates)
    {
        ResetOccupied(unit.gridCoord);
        unit.gridCoord = coordinates;
        OccupyTile(unit, coordinates);
    }
}
