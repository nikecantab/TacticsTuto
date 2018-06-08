using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

    // public void FindSelectableTiles()
    //{
    //    ComputeAdjacencyList(null);
    //    GetCurrentTile();

    //    Queue<Tile> process = new Queue<Tile>();

    //    process.Enqueue(currentTile); //add first tile to queue
    //    currentTile.visited = true;
    //    //currentTile.parent = null;

    //    while (process.Count > 0)
    //    {
    //        Tile t = process.Dequeue();


    //        var enemy = t.CheckEnemyOccupied(t.gridCoord);
    //        if (enemy != null)
    //            t.enemyOccupied = true;
    //        //selectableTiles.Add(t);

    //        if (t.distance <= remainingMove)
    //        {
    //            if (t.blocked)
    //                return;

    //            if (!t.occupied)
    //                t.selectable = true;
    //        }
    //        else
    //        {
    //            if (t.blocked)
    //                return;
    //            t.inAttackRange = true;
    //        }

    //        if (t.distance < remainingMove + Range)
    //        {
    //            foreach (Tile tile in t.adjacencyList)
    //            {
    //                if (t.blocked)
    //                    return;
    //                if (!tile.visited)
    //                {
    //                    tile.parent = t;
    //                    tile.visited = true;
    //                    tile.distance = 1 + t.distance;
    //                    process.Enqueue(tile);
    //                }
    //            }
    //        }

    //        //double check 
    //        if (t.occupied)
    //        {
    //            enemy = t.CheckEnemyOccupied(t.gridCoord);
    //            if (enemy != null)
    //                t.enemyOccupied = true;
    //        }

    //    }
    //    GridManager.UpdateTilesGrid();
    //}
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
