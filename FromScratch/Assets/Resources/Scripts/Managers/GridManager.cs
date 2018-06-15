using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

    public int gridSize;
    public GameObject[,] greenTiles;
    public GameObject[,] redTiles;

	// Use this for initialization
	void Awake () {

        //initialize tiles

        //get all green/red tiles
        greenTiles = new GameObject[gridSize, gridSize];
        redTiles = new GameObject[gridSize, gridSize];

        GameObject[] go = GameObject.FindGameObjectsWithTag("GreenTile");
        foreach (GameObject t in go)
        {
            greenTiles[(int)t.transform.parent.localPosition.x, (int)t.transform.parent.localPosition.z] = t;
        }

        go = GameObject.FindGameObjectsWithTag("RedTile");
        foreach (var t in go)
        {
            redTiles[(int)t.transform.parent.localPosition.x, (int)t.transform.parent.localPosition.z] = t;
        }
        

        DectivateAllTiles(greenTiles);
        DectivateAllTiles(redTiles);
    }

    void ActivateAllTiles(GameObject[,] grid)
    {
        for (int row = 0; row < Mathf.Sqrt(grid.Length); row++)
        {
            string print = "";
            for (int col = 0; col < Mathf.Sqrt(grid.Length); col++)
            {
                ActivateTile(grid[row, col]);
                print += string.Format(grid[row, col].name[0] + "|");

            }
            //Debug.Log(print);
        }
    }

    public void DectivateAllTiles(GameObject[,] grid)
    {
        for (int row = 0; row < Mathf.Sqrt(grid.Length); row++)
        {
            string print = "";
            for (int col = 0; col < Mathf.Sqrt(grid.Length); col++)
            {
                print += string.Format(grid[row, col].name[0] + "|");
                DeactivateTile(grid[row, col]);
                
            }
            //Debug.Log(print);
        }
    }

    //temp
    public void ActivateAroundTile(GameObject[,] grid, int range, Vector2Int start)
    {
        //get origin
        var origin = grid[start.x, start.y];

        float[,] g = new float[(int)Mathf.Sqrt(grid.Length), (int)Mathf.Sqrt(grid.Length)];
        for (var x = 0; x < gridSize; x++)
        {
            for (var y = 0; y < gridSize; y++)
            {

                g[x, y] = Mathf.Infinity;
            }
        }
        g[start.x, start.y] = 0;
        //a star get range
        List<GameObject> closed = new List<GameObject>();
        PriorityQueue<GameObject> open = new PriorityQueue<GameObject>();
        open.Clear();

        open.Add(origin, 0);


        //for (var t = 0; t < range; t++)
        {
            while (open.Count > 0)
            {
                //foreach(var t in open.GetElements())
                //{
                //    Debug.Log(open.GetPriorities()[open.IndexOf(t)] + "|" + GetTilePosition(t).x + "," + GetTilePosition(t).y);
                //}
                //Debug.Log("min sorted: " + open.GetPriorityMin());

                float currentTileG = open.GetPriorityMin();
                var currentTile = open.GetElementPriorityMin();
                open.Sort();
                //Debug.Log("current: " + currentTileG + "|" + GetTilePosition(currentTile).x + "," + GetTilePosition(currentTile).y);
                //Debug.Log("recheck: " + currentTileG + "|" + GetTilePosition(open.GetElementPriorityMin()).x + "," + GetTilePosition(open.GetElementPriorityMin()).y);
                open.Remove(open.GetElementPriorityMin());



                //foreach (var t in open.GetElements())
                //{
                //    Debug.Log("second check: " + open.GetPriorities()[open.IndexOf(t)] + "|" + GetTilePosition(t).x + "," + GetTilePosition(t).y);
                //}

                closed.Add(currentTile);

                //Handle neighbors
                for (var i = 0; i < 4; i++)
                {
                    var neighbor = GetNeighbor(grid, currentTile, i);
                    if (neighbor == null)
                        continue;

                    //check if it's in closed
                    if (closed.Contains(neighbor))
                        continue;

                    float tentativeG = currentTileG + 1; //can add terrain costs here


                    //Debug.Log("neighbor: " + tentativeG + "|" + GetTilePosition(neighbor).x + "," + GetTilePosition(neighbor).y);
                    //attack tiles could be handled here

                    if (tentativeG > range)
                        continue;

                    //if neighbor is not in open, add to open queue
                    if (!open.Contains(neighbor))
                    {
                        open.Add(neighbor, tentativeG);
                    }
                    else if (tentativeG >= currentTileG)
                        continue;

                    open.SetPriority(neighbor, tentativeG);

                    Vector2Int neighborPos = GetTilePosition(neighbor);
                    g[neighborPos.x, neighborPos.y] = tentativeG;
                }


            }
        }

        //activate 
        for (var x = 0; x < gridSize; x++)
        {
            for (var y = 0; y < gridSize; y++)
            {
                if (g[x, y] <= range)
                    ActivateTile(grid[x, y]);
            }
        }
    }

    GameObject GetNeighbor(GameObject[,] grid, GameObject tile, int loop)
    {
        Vector2Int pos = GetTilePosition(tile);

        switch(loop)
        {
            case 0:
                if (pos.x + 1 < gridSize && pos.y < gridSize)
                    return grid[pos.x + 1, pos.y];
                return null;
            case 1:
                if (pos.x < gridSize && pos.y - 1 >= 0)
                    return grid[pos.x, pos.y - 1];
                return null;
            case 2:
                if (pos.x - 1 >= 0 && pos.y < gridSize)
                    return grid[pos.x - 1, pos.y];
                return null;
            case 3:
                if (pos.x < gridSize && pos.y + 1 < gridSize)
                    return grid[pos.x, pos.y + 1];
                return null;
            default:
                return null;
        }
    }

    void ActivateTile(GameObject tile)
    {
        tile.SetActive(true);
    }

    void DeactivateTile(GameObject tile)
    {
        tile.SetActive(false);
    }

    Vector2Int GetTilePosition(GameObject tile)
    {
        return new Vector2Int((int)tile.transform.parent.localPosition.x, (int)tile.transform.parent.localPosition.z);
    }
    
}
