using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDataMiningManager : MonoBehaviour
{
    public UIManager ui;
    public World world;
    
    public GameObject badDataPrefab;
    public List<Sprite> chunks;

    public Text textProgress;
    public Text textMessage;
    public Image imageUnlockedTech;
    public Button saveData;
    public Button toggleDataMining;
    public Text textDataMiningButton;
    public Transform badDataBlock;

    public BuildingScriptable currentlyResearching;

    

    void Start()
    {
        currentlyResearching = world.researchQueue[0];
    }

    void Update()
    {
        string blocks = $"[{world.researchPoints}/{currentlyResearching.researchPoints}]";
        if (world.dataMiningOn)
            textProgress.text = $"Data Mining {blocks} Blocks Recovered";
        else
            textProgress.text = $"{blocks} Blocks Recovered";


    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {    
    }
}
