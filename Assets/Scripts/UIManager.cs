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
    public Text textShortMessage;

    public Text textGameOverSummary;

    public World world;

    public UIBuildButton uiBuildButtonPrefab;

    public GameObject gameStartPanel;
    public GameObject gameOverPanel;
    public GameObject statsPanel;
    public GameObject buildPanel;

    public UIPlotDetailManager plotDetailPanel;
    public UICursor uiCursor;
    public UIDialogBox uiDialogPanel;
    public UIMineButton uiMineButton;

    public void StartGame()
    {
        world.gameState = World.GameState.PLAYING;
        gameStartPanel.SetActive(false);
        statsPanel.SetActive(true);
        buildPanel.SetActive(true);
        uiMineButton.gameObject.SetActive(true);
    }

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

    public void ShowDialogBox(string title, string message, Sprite icon = null)
    {
        world.gameState = World.GameState.PAUSED;
        uiDialogPanel.title = title;
        uiDialogPanel.message = message;
        uiDialogPanel.icon = icon;
        uiDialogPanel.gameObject.SetActive(true);
    }

    public void HideDialogBox()
    {
        world.gameState = World.GameState.PLAYING;
        uiDialogPanel.gameObject.SetActive(false);
    }

    public void Inspect(Plot plot)
    {
        plotDetailPanel.InsepctPlot(plot);
    }

    public void Uninspect()
    {
        plotDetailPanel.UninspectPlot();
    }
    
    public void SetMessage(string message)
    {
        textShortMessage.text = message;
        Invoke("ClearMessage", 3f);
    }

    public void ClearMessage()
    {
        textShortMessage.text = "";
    }

    void Update()
    {
        if ( world.gameState == World.GameState.PLAYING)
        {
            textMonths.text = "year: " + (world.years / 12).ToString();
            textStarOutput.text = "star output: " + world.starOutput.ToString() + "kW";
            textEnergyStored.text = $"energy [{world.energyStored}/{world.GetStorageCapacity()}]kW "+ ((world.energyChange<0)?"dis":"")  + $"charging at {world.energyChange}kW";
            textMetals.text = "metals: " + world.metals.ToString();
        }

        if (world.gameState == World.GameState.GAMEOVER)
        {
            //ShowDialogBox("Game Over", $"You lasted {world.years} months before running out of energy. You used { ((float)world.cumulativeEnergyUse / (float)world.cumulativeEnergyOutput).ToString("F2") }% of the last star.");

            textGameOverSummary.text = $"You lasted {world.years/12} years before running out of energy. You used { ((float) world.cumulativeEnergyUse / (float) world.cumulativeEnergyOutput).ToString("F2") }% of the last star.";
            gameOverPanel.SetActive(true);
            statsPanel.SetActive(false);
        }
    }
}
