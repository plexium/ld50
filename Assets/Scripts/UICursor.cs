using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICursor : MonoBehaviour
{
    public SpriteRenderer renderer;
    private Color ghost = new Color(1f, 1f, 1f, 0.5f);
    private Color ghostInvalid = new Color(1f, 0f, 0f, 0.5f);

    public World world;
    public Vector3 worldPoint;
    public Vector3Int plotPoint;
    public BuildingScriptable placingBuilding;
    public UIManager uiManager;
    public Plot overPlot;


    void Update()
    {
        if (world.gameState != World.GameState.PLAYING) return;

        if (EventSystem.current.IsPointerOverGameObject()) return;

        worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPoint.z = 0;
        plotPoint = world.GetCellFromWorld(worldPoint);
        overPlot = world.IsValidPlot(plotPoint) ? world.GetPlotAt(plotPoint) : null;        

        if (overPlot != null)        
            uiManager.Inspect(overPlot);
        else
            uiManager.Uninspect();

        if (placingBuilding == null) return;

        Vector3 snapTo = new Vector3(worldPoint.x, worldPoint.y, 0f);
        snapTo.x = Mathf.Round(snapTo.x * 2f) / 2f;
        snapTo.y = Mathf.Round(snapTo.y * 4f) / 4f;

        gameObject.transform.position = snapTo;
        renderer.color = (overPlot != null && overPlot.CanBuild()) ? ghost : ghostInvalid;

        if (Input.GetMouseButtonDown(0) && overPlot != null && overPlot.CanBuild())
        {
            world.BuildBuildingOn(placingBuilding, overPlot);
            uiManager.SetMessage($"Placed {placingBuilding.title} (-{placingBuilding.requiredMetals} metals)");
            EndPlacing();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            CancelPlacing();
        }
    }

    public void StartPlacing(BuildingScriptable building)
    {
        placingBuilding = building;
        world.metals -= placingBuilding.requiredMetals;
        renderer.sprite = placingBuilding.sprite;
    }

    public void CancelPlacing()
    {
        world.metals += placingBuilding.requiredMetals;
        EndPlacing();
    }

    public void EndPlacing()
    {
        placingBuilding = null;
        renderer.sprite = null;
    }
}
