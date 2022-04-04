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
        PAUSED,
        PLAYING,
        GAMEOVER
    }

    private Plot[,] plots;
    public Plot selectedPlot;

    public event Action OnBuildablesChange;

    public UIManager ui;

    private int _sizeX = 16;
    private int _sizeY = 16;
    private float _ageSpeed = 1f; //seconds
    private float _mineSpeed = 2f; //seconds
    private float _dataMineSpeed = 2f; //seconds
    private Coroutine _worldUpdate;
    private Coroutine _minerUpdate;
    private Coroutine _dataMineUpdate;

    public int bufferYears = 10;
    public int years;
    public int starOutput;
    public int energyStored;
    public int energyChange;
    public int cumulativeEnergyUse = 0;
    public int cumulativeEnergyOutput = 0;

    public bool dataMiningOn;
    public int researchPoints;

    public int metals;

    public Star star;

    public List<BuildingScriptable> builtBuildings = new List<BuildingScriptable>();
    public List<BuildingScriptable> unlockedBuildables = new List<BuildingScriptable>();
    public List<BuildingScriptable> researchQueue = new List<BuildingScriptable>();
    public List<string> lore = new List<string>();

    public BuildingScriptable selectedBuilding;

    public GameState gameState;

    const float energyReduction = 1.01f;
    const int startingStarOutput = 3478;  //kW per month

    private int metalsAvailable = 1000;

    public Tilemap groundTilemap;
    public Tilemap buildingTilemap;

    public GameObject cursor;

    private void Awake()
    {

        years = 0;
        starOutput = startingStarOutput; 
        energyStored = 0;
        metals = 10;
        gameState = GameState.STARTING;
        //gameState = GameState.PLAYING;
        researchPoints = 0;
        dataMiningOn = false;
    }


    void Start()
    {
        BuildLand();
        Refresh();
        OnBuildablesChange?.Invoke();

        _worldUpdate = StartCoroutine(AgeTheWorld());        
        _minerUpdate = StartCoroutine(MineMetals());
        _dataMineUpdate = StartCoroutine(MineData());
    }


    void Update()
    {
    }

    void BuildLand()
    {
        plots = new Plot[_sizeX, _sizeY];

        Vector2Int center = new Vector2Int(_sizeX / 2, _sizeY / 2);
        int radius = (_sizeX / 2) - 1;

        for (int i = 0; i < _sizeX; i++)
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
                    plots[i, j].plot = SOFactory.I.GetRandomPlot();
                }
            }
        }

        BuildBuildingOn(SOFactory.I.GetBuilding("Starship"), plots[radius + 1, radius + 1]);

        unlockedBuildables.Add(SOFactory.I.GetBuilding("Wind"));
        unlockedBuildables.Add(SOFactory.I.GetBuilding("Miner"));

        researchQueue.Add(SOFactory.I.GetBuilding("Solar"));
        researchQueue.Add(SOFactory.I.GetBuilding("Battery"));


        lore.Add("\"The Entropy's computers had to be reprogrammed 1,338 times to continue tracking the year (25.83^38 CE).\"");
        lore.Add("\"A multi-generational starship sent coasting into the void. Forgotten. 1.4 million souls slowly turned to stone.\"");

    }

    public void BuildBuildingOn(BuildingScriptable building, Plot plot)
    {
        plot.building = building;
        builtBuildings.Add(building);
        Refresh();
    }

    public Plot GetPlotAt(Vector3Int cell)
    {
        Plot plot = plots[cell.x, cell.y];
        return (plot.usable) ? plot : null;
    }

    public Vector3Int GetCellFromWorld(Vector3 worldPoint)
    {
        return groundTilemap.WorldToCell(worldPoint);
    }

    public bool IsValidPlot(Vector3Int cell)
    {
        return !(cell.x >= _sizeX || cell.x < 0 || cell.y >= _sizeY || cell.y < 0);
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

    public void ToggleDataMining()
    {
        dataMiningOn = !dataMiningOn;
    }

    IEnumerator MineData()
    {
        while (true)
        {
            if ( dataMiningOn && gameState == World.GameState.PLAYING )
            {
                researchPoints++;

                if ( researchPoints >= researchQueue[0].researchPoints)
                {
                    ui.ShowDialogBox("Data Block Recovered : " + researchQueue[0].title, lore[0], researchQueue[0].sprite);
                    AddNewTech(researchQueue[0]);
                    researchQueue.RemoveAt(0);
                    lore.RemoveAt(0);
                    researchPoints = 0;                    

                    if (researchQueue.Count == 0)
                    {
                        dataMiningOn = false;
                        ui.uiMineButton.gameObject.SetActive(false);
                        StopCoroutine(_dataMineUpdate);
                    }
                }
            }         

            yield return new WaitForSeconds(_dataMineSpeed);
        }
    }

    public void AddNewTech(BuildingScriptable tech)
    {
        unlockedBuildables.Add(tech);
        OnBuildablesChange?.Invoke();
    }

    IEnumerator MineMetals()
    {
        while (true)
        {
            if (gameState == World.GameState.PLAYING)
            {
                int mining = 0;

                foreach (BuildingScriptable b in builtBuildings)
                    mining += b.producedMetals;

                metalsAvailable -= mining;

                if (metalsAvailable < 0)
                {
                    metals += mining + metalsAvailable;
                    StopCoroutine(_minerUpdate);
                }

                metals += mining;
            }

            yield return new WaitForSeconds(_mineSpeed);
        }
    }

    public int GetStorageCapacity()
    {
        int capacity = 0;

        foreach (BuildingScriptable b in builtBuildings)
        {
            capacity += b.storage;
        }

        return capacity;
    }

    IEnumerator AgeTheWorld()
    {
        while (true)
        {
            if (gameState == World.GameState.PLAYING)
            {
                years++;
                starOutput = (int)(starOutput / energyReduction);

                star.currentOutput = starOutput;

                int production = 0;
                int consumption = 0;
                int capacity = 0;

                foreach (BuildingScriptable b in builtBuildings)
                {
                    production += b.wattProduction;
                    consumption += b.wattConsumption;
                    capacity += b.storage;
                }

                if (dataMiningOn) consumption += 10;

                if (production > starOutput) production = starOutput;

                cumulativeEnergyUse += production;
                cumulativeEnergyOutput += starOutput;

                //calculate generation difference
                energyChange = production - consumption;

                //add or remove from storage
                energyStored += energyChange;

                if (energyStored > capacity)
                    energyStored = capacity;

                if (energyStored < 0)
                {
                    energyStored = 0;
                    dataMiningOn = false;
                    ui.SetMessage($"!! ALERT NO ENERGY !! RESERVES AT {bufferYears} !!");
                    bufferYears--;
                    if (bufferYears == 0)
                    {
                        energyStored = 0;
                        StopCoroutine(_worldUpdate);
                        gameState = GameState.GAMEOVER;
                    }
                }
                else
                {
                    bufferYears = 10;
                }
            }

            yield return new WaitForSeconds(_ageSpeed);
        }
    }
}
