using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Plot 
{
    public Vector3 worldLoc;
    public Vector3Int loc;
    public BuildingScriptable building;
    public PlotScriptable plot;

    public bool usable = false;

    public Plot()
    {        
    }

}
