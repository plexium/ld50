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

    public Text textGameOverSummary;

    public World world;

    public UIBuildButton uiBuildButtonPrefab;

    public GameObject gameOverPanel;
    public GameObject statsPanel;
    public GameObject buildPanel;

    public UIPlotDetailManager plotDetailPanel;

    public void StartPlacing(BuildingScriptable building)
    {
        world.selectedBuilding = building;
    }

    void Start()
    {
        plotDetailPanel.UninspectPlot();
        RefreshBuildables();

        world.OnPlotSelect += ShowInspector;
        world.OnBuildablesChange += RefreshBuildables;
    }


    void RefreshBuildables()
    {
        for (int i = buildPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(buildPanel.transform.GetChild(i).gameObject);

        foreach ( BuildingScriptable buildable in world.buildables)
        {            
            UIBuildButton button = Instantiate(uiBuildButtonPrefab, buildPanel.transform);
            button.Building = buildable;
            button.ui = this;
        }
    }

    void ShowInspector()
    {
        plotDetailPanel.InsepctPlot(world.selectedPlot);
    }
    
    void Update()
    {
        if ( world.gameState == World.GameState.PLAYING)
        {
            textMonths.text = world.months.ToString();
            textStarOutput.text = world.starOutput.ToString();
            textEnergyStored.text = world.energyStored.ToString();
            textEnergyCapture.text = world.energyChange.ToString();


        }

        if (world.gameState == World.GameState.GAMEOVER)
        {
            textGameOverSummary.text = $"You lasted {world.months} months before running out of energy. You used { ((float) world.cumulativeEnergyUse / (float) world.cumulativeEnergyOutput).ToString("F2") }% of the last star.";
            gameOverPanel.SetActive(true);
            statsPanel.SetActive(false);
        }
    }
}
