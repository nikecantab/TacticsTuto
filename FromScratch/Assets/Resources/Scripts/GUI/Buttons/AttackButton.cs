using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour {

    public ActionPanelManager actionPanelManager;

    public void Attack()
    {
        var cursor = GameObject.Find("GUICursor").GetComponent<Cursor>();
        cursor.state = CursorState.SelectingTarget;
        cursor.selectedUnit.state = UnitState.SelectingTarget;

        actionPanelManager.ClosePanel();
    }
}
