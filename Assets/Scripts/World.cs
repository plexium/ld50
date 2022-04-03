using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    public enum GameState
    {
        STARTING,
        PLAYING,
        GAMEOVER
    }

    private Plot[,] plots;
    public Plot selectedPlot;

    public event Action OnPlotSelect;
    public event Action OnBuildablesChange;

    private int _sizeX = 16;
    private int _sizeY = 16;
    private float _ageSpeed = 2f; //seconds
    private Coroutine _worldUpdate;

    public int months;
    public int starOutput;
    public int energyStored;
    public int energyChange;
    public int cumulativeEnergyUse = 0;
    public int cumulativeEnergyOutput = 0;

    public List<BuildingScriptable> builtBuildings = new List<BuildingScriptable>();
    public List<BuildingScriptable> buildables = new List<BuildingScriptable>();

    public BuildingScriptable selectedBuilding;

    public GameState gameState;

    const float energyReduction = 1.5f;
    const int startingStarOutput = 3478;  //kW per month

    public Tilemap groundTilemap;
    public Tilemap buildingTilemap;

    public GameObject cursor;

    private void Awake()
    {
        months = 0;
        starOutput = startingStarOutput; 
        energyStored = 0;
        gameState = GameState.PLAYING;


        plots = new Plot[_sizeX, _sizeY];

        Vector2Int center = new Vector2Int(_sizeX / 2, _sizeY / 2);
        int radius = (_sizeX / 2) - 1;

        for ( int i = 0; i < _sizeX; i++)
        {
            for (int j = 0; j < _sizeY; j++)
            {
                plots[i, j] = new Plot();
                plots[i, j].loc = new Vector3Int(i, j, 0);                

                if (Vector2Int.Distance(center, new Vector2Int(i, j)) > radius)
                {
                    plots[i, j].usable = false;
                }
                else
                {
                    plots[i, j].usable = true;
                    plots[i, j].plot = Factory.I.GetRandomPlot();
                }
            }
        }

        plots[radius+1, radius+1].building = Factory.I.GetBuilding("Starship");
        builtBuildings.Add(Factory.I.GetBuilding("Starship"));
        buildables.Add(Factory.I.GetBuilding("Solar"));
        buildables.Add(Factory.I.GetBuilding("Battery"));
    }


    void Start()
    {
        Refresh();
        _worldUpdate = StartCoroutine(AgeTheWorld());
    }

    
    void Update()
    {
        if ( gameState == GameState.PLAYING)
        {
            Plot p = GetPlotAt(Input.mousePosition);

            if (p == null) return;

            //cursor.transform.position = point;

            if (Input.GetMouseButtonDown(0))
            {
                selectedPlot = p;
                OnPlotSelect?.Invoke();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (selectedBuilding == null) return;

                selectedPlot = p;
                selectedPlot.building = selectedBuilding;
                builtBuildings.Add(selectedBuilding);
                Refresh();
                OnPlotSelect?.Invoke();
            }
        }
    }

    Plot GetPlotAt(Vector3 mousePosition)
    {
        Vector3 point = Camera.main.ScreenToWorldPoint(mousePosition);
        point.z = 0;

        Vector3Int gridPosition = groundTilemap.WorldToCell(point);

        if (gridPosition.x >= _sizeX || gridPosition.x < 0 || gridPosition.y >= _sizeY || gridPosition.y < 0) return null;

        Plot plot = plots[gridPosition.x, gridPosition.y];

        if (!plot.usable) return null;

        return plot;
    }

    void Refresh()
    {
        for (int i = 0; i < _sizeX; i++)
            for (int j = 0; j < _sizeY; j++)
            {
                if (plots[i, j].usable)
                {
                    groundTilemap.SetTile(plots[i, j].loc, plots[i, j].plot.tile);

                    if (plots[i, j].building == null) continue;

                    buildingTilemap.SetTile(plots[i, j].loc, plots[i, j].building.tile);
                }
            }
    }

    IEnumerator AgeTheWorld()
    {
        while (true)
        {
            months++;
            starOutput = (int)(starOutput / energyReduction);

            int production = 0;
            int consumption = 0;
            int capacity = 0;

            foreach (BuildingScriptable b in builtBuildings) 
            {
                production += b.wattProduction;
                consumption += b.wattConsumption;
                capacity += b.storage;
            }            

            if (production > starOutput ) production = starOutput;

            cumulativeEnergyUse += production;
            cumulativeEnergyOutput += starOutput;

            //calculate generation difference
            energyChange = production - consumption;

            //add or remove from storage
            energyStored += energyChange;

            if (energyStored > capacity)
                energyStored = capacity;

            if ( energyStored < 0 )
            {
                energyStored = 0;
                StopCoroutine(_worldUpdate);
                gameState = GameState.GAMEOVER;
            }

            yield return new WaitForSeconds(_ageSpeed);
        }
    }
}
