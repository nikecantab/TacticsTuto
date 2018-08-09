using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatPanelManager : MonoBehaviour {
    [SerializeField]
    GameObject NameObject;
    [SerializeField]
    GameObject EnergyObject;
    [SerializeField]
    GameObject StrengthObject;
    [SerializeField]
    GameObject ConstitutionObject;
    [SerializeField]
    GameObject ESliderObject;
    [SerializeField]
    GameObject SSliderObject;
    [SerializeField]
    GameObject CSliderObject;

    public string Name = "Name";
    public int Energy = 4;
    public int Strength = 2;
    public int Constitution = 1;
    public int MEnergy = 4;
    public int MStrength = 2;
    public int MConstitution = 1;

    public Unit Unit;

    // Use this for initialization
    void Start () {
        UpdateStatPanel();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetUnit(Unit unit)
    {
        Unit = unit;

        Name = unit.Name;
        Energy = unit.Energy;
        Strength = unit.Strength;
        Constitution = unit.Constitution;
        MEnergy = unit.MEnergy;
        MStrength = unit.MStrength;
        MConstitution = unit.MConstitution;

        UpdateStatPanel();
    }

    public void UpdateStatPanel()
    {
        SetStatPanel(Name, Energy, MEnergy, Strength, MStrength, Constitution, MConstitution);
    }

    public void SetStatPanel(string name, int eng, int maxE, int str, int maxS, int con, int maxC) 
    {
        SetName(name);
        SetEnergy(eng, maxE);
        SetStrength(str, maxS);
        SetConstitution(con, maxC);
    }

    public void UpdatePosition(Unit unit)
    {
        transform.localPosition = new Vector3(unit.pos.x, transform.localPosition.y, unit.pos.y);
    }

    void UpdatePosition(Vector2Int coords)
    {
        transform.localPosition = new Vector3(coords.x, transform.localPosition.y, coords.y);
    }

    void SetName(string name)
    {
        var text = NameObject.GetComponent<Text>();

        text.text = name;
    }

    void SetEnergy(int energy, int maxEnergy)
    {
        var text = EnergyObject.GetComponent<Text>();
        var slider = ESliderObject.GetComponent<Slider>();

        text.text = energy + "/" + maxEnergy;
        slider.value = energy;
    }

    void SetStrength(int strength, int maxStrength)
    {
        var text = StrengthObject.GetComponent<Text>();
        var slider = SSliderObject.GetComponent<Slider>();

        text.text = strength + "/" + maxStrength;
        slider.value = strength;
    }

    void SetConstitution(int con, int maxCon)
    {
        var text = ConstitutionObject.GetComponent<Text>();
        var slider = CSliderObject.GetComponent<Slider>();

        text.text = con + "/" + maxCon;
        slider.value = con;
    }

}
