using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public Vector2Int pos;
    public List<GameObject> greenActivatorParent;
    public List<GameObject> redActivatorParent;

    private void Start()
    {
        pos = new Vector2Int((int)transform.localPosition.x, (int)transform.localPosition.z);
        var go = GameObject.Find("Managers");
        go.GetComponent<TileManager>().AddTile(this, pos);
        //TileManager.AddTile(this, pos);
    }

    void CheckParents()
    {
        if (greenActivatorParent.Count < 1)
        {
            //tell grid manager to turn off green overlay
        }

        if (redActivatorParent.Count < 1)
        {
            //tell grid manager to turn off red overlay
        }
    }
}
