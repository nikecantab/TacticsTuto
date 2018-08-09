using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitButton : MonoBehaviour {

    public ActionPanelManager actionPanelManager;

    public void Wait()
    {
        var cursor = GameObject.Find("GUICursor").GetComponent<Cursor>();
        cursor.selectedUnit.state = UnitState.EndingPhase;

        actionPanelManager.ClosePanel();
    }
}
