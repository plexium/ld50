using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOFactory : MonoBehaviour
{
    #region singleton
    public static SOFactory I;

    void Awake()
    {
        if (I != null && I != this)
            Destroy(gameObject);
        else
            I = this;
    }
    #endregion

    public List<PlotScriptable> plots = new List<PlotScriptable>();
    public List<BuildingScriptable> buildings = new List<BuildingScriptable>();

    public PlotScriptable GetPlot(string title)
    {
        foreach ( PlotScriptable plot in plots)
            if (plot.name == title)
                return plot;

        return null;
    }

    public PlotScriptable GetRandomPlot()
    {
        return plots[Mathf.FloorToInt(plots.Count * Random.value)];
    }

    public BuildingScriptable GetBuilding(string title)
    {
        foreach (BuildingScriptable building in buildings)
            if (building.name == title)
                return building;

        return null;
    }

}
