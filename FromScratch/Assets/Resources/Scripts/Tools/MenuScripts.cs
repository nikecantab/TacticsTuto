using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MenuScript
{
    public static void CreateGrid(int size)
    {
        GameObject grid = GameObject.Find("Grid");

        if (grid != null)
        {
            for (var c = 0; c < grid.transform.childCount; c++)
            {
                var child = grid.transform.GetChild(c);
                for (var d = 0; d < child.transform.childCount; d++)
                {
                    Object.DestroyImmediate(child.transform.GetChild(d).gameObject);
                }
                    Object.DestroyImmediate(grid.transform.GetChild(c).gameObject);
            }
            Object.DestroyImmediate(grid);
        }

        GameObject offset = GameObject.Find("Offset");

        grid = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/Tiles/Grid") as GameObject) as GameObject;
        offset.transform.position = (Vector3.left * (size/2) + Vector3.up * 0.1f + Vector3.back * (size/2));
        grid.transform.parent = offset.transform;
        grid.transform.localPosition = Vector3.zero;

        for (var x = 0; x < size; x++)
        {

            for (var y = 0; y < size; y++)
            {
                GameObject tile = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/Tiles/TileFrame") as GameObject) as GameObject;
                tile.transform.parent = grid.transform;
                tile.transform.localPosition = Vector3.zero;
                tile.transform.localPosition = new Vector3(tile.transform.localPosition.x + x, tile.transform.localPosition.y, tile.transform.localPosition.z + y);

                GameObject greenTile = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/Tiles/GreenTile") as GameObject) as GameObject;
                greenTile.transform.parent = tile.transform;
                greenTile.transform.localPosition = Vector3.zero;
                greenTile.transform.localRotation = Quaternion.identity;
                //greenTile.SetActive(false);

                GameObject redTile = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/Tiles/RedTile") as GameObject) as GameObject;
                redTile.transform.parent = tile.transform;
                redTile.transform.localPosition = Vector3.zero;
                redTile.transform.localRotation = Quaternion.identity;
                //redTile.SetActive(false);
            }
        }


        GameObject go = GameObject.Find("Managers");
        go.GetComponent<GridManager>().gridSize = size;
    }

    [MenuItem("Tools/Create Grid/16x16")]
    public static void CreateGrid16x16()
    {
        CreateGrid(16);
    }

    [MenuItem("Tools/Create Grid/24x24")]
    public static void CreateGrid24x24()
    {
        CreateGrid(24);
    }

    [MenuItem("Tools/Create Grid/32x32")]
    public static void CreateGrid32x32()
    {
        CreateGrid(32);
    }
    //[MenuItem("Tools/Assign Tile Script")]
    //public static void AssignTileScript()
    //{
    //    GameObject[]
    //    tiles = GameObject.FindGameObjectsWithTag("Tile");

    //    foreach (GameObject t in tiles)
    //    {
    //        t.AddComponent<Tile>();
    //    }
    //}
}