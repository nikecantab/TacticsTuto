﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrippleButton : MonoBehaviour {

    public ActionPanelManager actionPanelManager;

    public void Cripple()
    {
        var cursor = GameObject.Find("GUICursor").GetComponent<Cursor>();
        cursor.state = CursorState.SelectingTarget;
        cursor.selectedUnit.state = UnitState.Attacking;

        actionPanelManager.ClosePanel();
    }
}
