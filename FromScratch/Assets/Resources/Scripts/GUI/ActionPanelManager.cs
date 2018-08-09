using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPanelManager : MonoBehaviour {
    
	

    public void AddActionButton(ActionButton button)
    {
        var panel = gameObject.transform.GetChild(0);

        switch (button)
        {
            case ActionButton.Attack:
                GameObject attack = Instantiate(Resources.Load("Prefabs/GUI/ActionButtons/AttackButton", typeof(GameObject))) as GameObject;
                attack.transform.SetParent(panel.transform);
                var rectA = attack.GetComponent<RectTransform>();
                rectA.localScale = new Vector3(1, 1, 1);
                attack.GetComponent<AttackButton>().actionPanelManager = this; 
                break;
            case ActionButton.Wait:
                GameObject wait = Instantiate(Resources.Load("Prefabs/GUI/ActionButtons/WaitButton", typeof(GameObject))) as GameObject;
                wait.transform.SetParent(panel.transform);
                var rectW = wait.GetComponent<RectTransform>();
                //rectW.position = new Vector3(0, 0, 0); //this doesn't work for the z axis for some reason....
                rectW.localScale = new Vector3(1, 1, 1);
                wait.GetComponent<WaitButton>().actionPanelManager = this;
                break;
            case ActionButton.Cripple:
                GameObject cripple = Instantiate(Resources.Load("Prefabs/GUI/ActionButtons/CrippleButton", typeof(GameObject))) as GameObject;
                cripple.transform.SetParent(panel.transform);
                var rectC = cripple.GetComponent<RectTransform>();
                rectC.position = new Vector3(0, 0, 0);
                rectC.localScale = new Vector3(1, 1, 1);
                cripple.GetComponent<CrippleButton>().actionPanelManager = this;
                break;
            case ActionButton.Impair:
                GameObject impair = Instantiate(Resources.Load("Prefabs/GUI/ActionButtons/ImpairButton", typeof(GameObject))) as GameObject;
                impair.transform.SetParent(panel.transform);
                var rectI = impair.GetComponent<RectTransform>();
                rectI.position = new Vector3(0, 0, 0);
                rectI.localScale = new Vector3(1, 1, 1);
                impair.GetComponent<ImpairButton>().actionPanelManager = this;
                break;
            case ActionButton.Weaken:
                GameObject weaken = Instantiate(Resources.Load("Prefabs/GUI/ActionButtons/WeakenButton", typeof(GameObject))) as GameObject;
                weaken.transform.SetParent(panel.transform);
                var rectWe = weaken.GetComponent<RectTransform>();
                rectWe.position = new Vector3(0, 0, 0);
                rectWe.localScale = new Vector3(1, 1, 1);
                weaken.GetComponent<WeakenButton>().actionPanelManager = this;
                break;

        }
    }

    public void UpdatePosition(Unit unit)
    {
        transform.localPosition = new Vector3(unit.pos.x, transform.localPosition.y, unit.pos.y);
    }

    public void ClosePanel()
    {
        Destroy(gameObject);
    }
}
