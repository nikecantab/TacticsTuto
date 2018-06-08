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
        unit.gridCoord = coordinates;
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
        //Debug.Log(string.Format("got tile coords x:{0} y{1}", coordinates.x, coordinates.y));
        tile = tilesGrid[(int)coordinates.x, (int)coordinates.y];
    }

    public static void GetTileBeneathUnit(Unit unit, out Tile tile)
    {
        tile = null;
        GetTileAtCoord(unit.gridCoord, out tile);
    }

    public static Unit GetOccupant(Vector2 coordinates)
    {
        return unitsGrid[(int)coordinates.x, (int)coordinates.y];
    }

    public static void ResetOccupied(Vector2 coordinates)
    {
        unitsGrid[(int)coordinates.x, (int)coordinates.y] = null;
        tilesGrid[(int)coordinates.x, (int)coordinates.y].occupied = false;
    }

    public static void OccupyTile(Unit unit, Vector2 coordinates)
    {
        unitsGrid[(int)coordinates.x, (int)coordinates.y] = unit;
        unit.gridCoord = coordinates;
        tilesGrid[(int)coordinates.x, (int)coordinates.y].occupied = true;
        //if (tilesGrid[(int)coordinates.x, (int)coordinates.y].occupied == true)
            //Debug.Log("tile occupied");
    }

    public static void UpdateUnitPosition(Unit unit, Vector2 coordinates)
    {
        //Debug.Log(string.Format("updated gridcoordx:{0}, gridcoordy{1}, coordx{2}, coordy{3}", unit.gridCoord.x, unit.gridCoord.y, coordinates.x, coordinates.y));
        ResetOccupied(unit.gridCoord);
        OccupyTile(unit, coordinates);
        //Debug.Log(string.Format("new coords: x{0} y{1}", unit.gridCoord.x, unit.gridCoord.y));
        //Debug.Log(string.Format("updated grid: {0},{1}", coordinates.x, coordinates.y));
    }

    public static void UpdateTilesGrid()
    {
        //reset all tiles
        for (int x = 0; x < unitsGrid.GetLength(0); x++)
        {
            for (int y = 0; y < unitsGrid.GetLength(1); y++)
            {
                if (unitsGrid[x, y] != null)
                {
                    if (tilesGrid[x, y].blocked)
                        return;
                    tilesGrid[x, y].occupied = true;
                    tilesGrid[x, y].selectable = false;
                }
            }
        }
    }

    public static void UpdateAll()
    {
        Debug.Log("UpdateAll");
        //reset all tiles
        for (int x = 0; x < unitsGrid.GetLength(0); x++)
        {
            for (int y = 0; y < unitsGrid.GetLength(1); y++)
            {
                unitsGrid[x, y] = null;
                tilesGrid[x, y].Reset();
            }
        }

        var players = GameObject.FindGameObjectsWithTag("Player");
        var npcs = GameObject.FindGameObjectsWithTag("NPC");

        foreach (GameObject player in players)
        {
            var unit = player.GetComponent<Unit>();

            //assign unit to cell
            int unitX = (int)unit.gridCoord.x;
            int unitY = (int)unit.gridCoord.y;
            unitsGrid[unitX, unitY] = unit;

            tilesGrid[unitX, unitY].occupied = true;

            if (player.tag != TurnManager.turnKey.Peek())
            {
                tilesGrid[unitX, unitY].enemyOccupied = true;
            }
        }

        foreach (GameObject npc in npcs)
        {
            var unit = npc.GetComponent<Unit>();

            //assign unit to cell
            int unitX = (int)unit.gridCoord.x;
            int unitY = (int)unit.gridCoord.y;
            unitsGrid[unitX, unitY] = unit;

            tilesGrid[unitX, unitY].occupied = true;

            if (npc.tag != TurnManager.turnKey.Peek())
            {
                tilesGrid[unitX, unitY].enemyOccupied = true;
            }
        }
    }
}
