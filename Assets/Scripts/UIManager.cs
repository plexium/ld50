using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Text textMonths;
    public Text textStarOutput;
    public Text textEnergyStored;
    public Text textEnergyCapture;
    public Text textMetals;

    public Text textGameOverSummary;

    public World world;

    public UIBuildButton uiBuildButtonPrefab;
    

    public GameObject gameOverPanel;
    public GameObject statsPanel;
    public GameObject buildPanel;

    public UIPlotDetailManager plotDetailPanel;
    public UICursor uiCursor;
    public UIDataMiningManager dataMiningPanel;

    public void StartPlacing(BuildingScriptable building)
    {
        world.selectedBuilding = building;
        uiCursor.StartPlacing(building);
    }

    public void EndPlacing()
    {
        world.selectedBuilding = null;
        uiCursor.EndPlacing();
    }

    void Start()
    {
        plotDetailPanel.UninspectPlot();
        RefreshBuildables();
        world.OnBuildablesChange += RefreshBuildables;
        dataMiningPanel.world = world;
        dataMiningPanel.ui = this;
    }

    public void ShowDataMiningPanel()
    {
        dataMiningPanel.gameObject.SetActive(true);
    }

    public void HideDataMiningPanel()
    {
        dataMiningPanel.gameObject.SetActive(false);
    }

    void RefreshBuildables()
    {
        for (int i = buildPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(buildPanel.transform.GetChild(i).gameObject);

        foreach ( BuildingScriptable buildable in world.unlockedBuildables)
        {            
            UIBuildButton button = Instantiate(uiBuildButtonPrefab, buildPanel.transform);
            button.Building = buildable;
            button.ui = this;
        }
    }

    public void Inspect(Plot plot)
    {
        plotDetailPanel.InsepctPlot(plot);
    }

    public void Uninspect()
    {
        plotDetailPanel.UninspectPlot();
    }
    
    void Update()
    {
        if ( world.gameState == World.GameState.PLAYING)
        {
            textMonths.text = world.years.ToString();
            textStarOutput.text = world.starOutput.ToString() + "kW";
            textEnergyStored.text = world.energyStored.ToString() + "kW";
            textEnergyCapture.text = world.energyChange.ToString() + "kW";
            textMetals.text = world.metals.ToString() + "t";
        }

        if (world.gameState == World.GameState.GAMEOVER)
        {
            textGameOverSummary.text = $"You lasted {world.years} months before running out of energy. You used { ((float) world.cumulativeEnergyUse / (float) world.cumulativeEnergyOutput).ToString("F2") }% of the last star.";
            gameOverPanel.SetActive(true);
            statsPanel.SetActive(false);
        }
    }
}
