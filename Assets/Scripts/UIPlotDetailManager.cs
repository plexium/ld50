using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlotDetailManager : MonoBehaviour
{
    public Image buildingIcon;
    public Text textPlotDescription;
    public Text textBuildingDetails;

    private Plot _inspectingPlot;
    void Update()
    {
        if (_inspectingPlot == null) return;

        if ( _inspectingPlot.building != null)
        {
            buildingIcon.sprite = _inspectingPlot.building.sprite;
            textPlotDescription.text = _inspectingPlot.building.title;
            textBuildingDetails.text = $"Energy Produced: {_inspectingPlot.building.wattProduction}kW\nEnergy Consumption: {_inspectingPlot.building.wattConsumption}kW\nStorage: {_inspectingPlot.building.storage}kW";
        }
        else
        {
            buildingIcon.sprite = _inspectingPlot.plot.sprite;
            textPlotDescription.text = _inspectingPlot.plot.title;
            textBuildingDetails.text = "Just some dirt";
        }
    }

    public void InsepctPlot(Plot plot)
    {
        _inspectingPlot = plot;
        gameObject.SetActive(true);
    }

    public void UninspectPlot()
    {
        _inspectingPlot = null;
        gameObject.SetActive(false);
    }
}
