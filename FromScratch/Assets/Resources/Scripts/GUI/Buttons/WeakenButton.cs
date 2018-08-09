using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakenButton : MonoBehaviour {

    public ActionPanelManager actionPanelManager;

    public void Weaken()
    {
        var cursor = GameObject.Find("GUICursor").GetComponent<Cursor>();
        //cursor.state = CursorState.SelectingTarget;
        cursor.selectedUnit.state = UnitState.EndingPhase;

        actionPanelManager.ClosePanel();
    }
}
