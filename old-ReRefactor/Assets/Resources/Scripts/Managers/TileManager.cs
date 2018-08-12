using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {
    public bool highlighting = false;
    GameObject[] tiles;

    private void Awake()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");
    }

    public void ComputeAdjacencyList()
    {
        foreach (GameObject tile in tiles)
        {
            //have every tile check its neighbors and add them to its adjacency list
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighborsLite();
        }
    }
    public void HighlightTiles(Unit unit)
    {
        ComputeAdjacencyList();

        Queue<Tile> process = new Queue<Tile>();

        Tile currentTile;
        GridManager.GetTileBeneathUnit(unit, out currentTile);
        process.Enqueue(currentTile); //add first tile to queue
        currentTile.visited = true;
        //currentTile.parent = null;

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();

            //selectableTiles.Add(t);
            t.highlighted = true;

            if (t.distance <= unit.Energy + unit.Range)
            {
                foreach (Tile tile in t.adjacencyList)
                {
                    if (t.blocked)
                        return;

                    tile.highlightParent = t;
                    tile.visited = true;
                    tile.distance = 1 + t.distance;
                    process.Enqueue(tile);
                }
            }

        }
        //GridManager.UpdateTilesGrid();
    }
    
    public void ResetHighlight()
    {
        foreach(GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.highlightParent = null;
            t.highlighted = false;
        }
        highlighting = false;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
