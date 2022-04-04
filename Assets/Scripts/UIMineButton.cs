using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMineButton : MonoBehaviour
{
    public Text textDisplay;
    public World world;
    private Button _button;

    public int energyRequired = 10;

    void Start()
    {
        _button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (world.researchQueue.Count == 0 || world.gameState != World.GameState.PLAYING) return;

        _button.interactable = (world.energyStored >= energyRequired || world.energyChange > 0);
        textDisplay.text = (world.dataMiningOn) ? $"{Random.Range(999999f,100000f).ToString("000000")} [{world.researchPoints}/{world.researchQueue[0].researchPoints}] Found" : "Not Data Mining (costs 10kW)";
    }
    
}
