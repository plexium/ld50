using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildButton : MonoBehaviour
{
    public UIManager ui;
    private BuildingScriptable _building;
    public BuildingScriptable Building { 
        set { 
            _building = value; 
            textTitle.text = _building.title + " ( "+ _building.requiredMetals +" metals)";
            textDesc.text = _building.description;
            imageIcon.sprite = _building.sprite; 
        } 
        get { return _building; } 
    }

    public Text textTitle;
    public Text textDesc;
    public Image imageIcon;


    public void OnClick()
    {
        ui.StartPlacing(_building);
    }

    public void Update()
    {
        GetComponent<Button>().interactable = (ui.world.metals >= _building.requiredMetals);
    }
}
