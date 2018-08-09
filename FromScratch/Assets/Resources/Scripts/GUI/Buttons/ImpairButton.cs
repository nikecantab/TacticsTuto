using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpairButton : MonoBehaviour {

    public ActionPanelManager actionPanelManager;

    public void Impair()
    {
        var cursor = GameObject.Find("GUICursor").GetComponent<Cursor>();
        cursor.state = CursorState.SelectingTarget;
        cursor.selectedUnit.state = UnitState.Attacking;
        cursor.selectedUnit.currentAttack = AttackType.Impair;

        actionPanelManager.ClosePanel();
    }
}
