using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
    [SerializeField]
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
            var x = (int)t.transform.parent.localPosition.x;
            var y = (int)t.transform.parent.localPosition.z;

            greenTiles[x, y] = t;
        }

        go = GameObject.FindGameObjectsWithTag("RedTile");
        foreach (var t in go)
        {
            redTiles[(int)t.transform.parent.localPosition.x, (int)t.transform.parent.localPosition.z] = t;
        }
        DeactivateAllTiles(greenTiles);
        DeactivateAllTiles(redTiles);
    }

    void ActivateAllTiles(GameObject[,] grid)
    {
        for (int row = 0; row < gridSize; row++)
        {
            string print = "";
            for (int col = 0; col < gridSize; col++)
            {
                ActivateTile(grid[row, col]);
                print += string.Format(grid[row, col].name[0] + "|");

            }
            //Debug.Log(print);
        }
    }

    public void DeactivateAllTiles(GameObject[,] grid)
    {
        for (int row = 0; row < gridSize; row++)
        {
            string print = "";
            for (int col = 0; col <gridSize; col++)
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

        float[,] g = new float[gridSize,gridSize];
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
                float currentTileG = open.GetPriorityMin();
                var currentTile = open.GetElementPriorityMin();
                open.Sort();
                open.Remove(open.GetElementPriorityMin());
                

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

    public void ActivateFrontierTiles(GameObject[,] referenceGrid, GameObject[,] attackGrid, int moveRange)//, int attackRange)
    {
        //if (attackRange < 1)
        //    return;

        List<GameObject> open = new List<GameObject>();
        List<GameObject> closed = new List<GameObject>();
        //Range 1
        //check activated tiles
        for (var x = 0; x < gridSize; x++)
        {
            for (var y = 0; y < gridSize; y++)
            {
                if (referenceGrid[x, y].activeSelf)
                {
                    closed.Add(referenceGrid[x, y]);
                    for (var i = 0; i < 4; i++)
                    {
                        var neighbor = GetNeighbor(referenceGrid, referenceGrid[x, y], i);
                        if (neighbor == null)
                            continue;
                        var neighborPos = GetTilePosition(neighbor);
                        //if (neighborPos == null)
                        //    continue;

                        if (!neighbor.activeSelf && !attackGrid[neighborPos.x,neighborPos.y].activeSelf)
                            open.Add(ActivateTile(attackGrid[neighborPos.x, neighborPos.y]));
                    }
                }
            }
        }
        #region trash code
        /*
        //Range > 1 --- Currently don't need that, doesn't work for range > 2 yet
        if (attackRange>1)
        {
            for (var r = 0; r < attackRange - 1; r++)
            {
                for (var x = 0; x < gridSize; x++)
                {
                    for (var y = 0; y < gridSize; y++)
                    {
                        if (closed.Contains(attackGrid[x, y]))
                            continue;
                        if (attackGrid[x, y].activeSelf && open.Contains(attackGrid[x, y]))
                        {
                            closed.Add(attackGrid[x, y]);
                            open.Remove(attackGrid[x, y]);
                            for (var i = 0; i < 4; i++)
                            {
                                var neighbor = GetNeighbor(attackGrid, attackGrid[x, y], i);
                                if (neighbor == null)
                                    continue;
                                var neighborPos = GetTilePosition(neighbor);
                                if (neighborPos == null)
                                    continue;
                                if (!neighbor.activeSelf && !referenceGrid[neighborPos.x, neighborPos.y].activeSelf && !closed.Contains(attackGrid[neighborPos.x, neighborPos.y]))
                                {
                                    ActivateTile(attackGrid[neighborPos.x, neighborPos.y]);
                                }
                            }
                        }
                    }
                }

                //foreach closed tile
                foreach (var t in closed)
                {
                    if (open.Contains(t))
                        continue;

                    for (var i = 0; i < 4; i++)
                    {
                        var neighbor = GetNeighbor(attackGrid, attackGrid[GetTilePosition(t).x, GetTilePosition(t).y], i);
                        if (neighbor == null)
                            continue;

                        var neighborPos = GetTilePosition(neighbor);
                        if (neighborPos == null)
                            continue;

                        //if neighbor is empty
                        if (!neighbor.activeSelf)
                        {
                            if (!referenceGrid[neighborPos.x, neighborPos.y].activeSelf)
                            {
                                //re add to open list
                                open.Add(t);
                                Debug.Log("added tile to open " + GetTilePosition(t).x + ", " + GetTilePosition(t).y);
                            }
                        }
                            
                    }
                }

                foreach (var t in open)
                {
                    if (closed.Contains(t))
                        closed.Remove(t);
                }
            }
        }*/
        #endregion

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

    GameObject ActivateTile(GameObject tile)
    {
        tile.SetActive(true);
        return tile;
    }

    GameObject DeactivateTile(GameObject tile)
    {
        tile.SetActive(false);
        return tile;
    }

    Vector2Int GetTilePosition(GameObject tile)
    {
        return new Vector2Int((int)tile.transform.parent.localPosition.x, (int)tile.transform.parent.localPosition.z);
    }

    
}
