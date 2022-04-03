using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DataMiner
{
    private bool _on = false;
    private BuildingScriptable _current;
    private int _blocksRecovered;
    private List<BuildingScriptable> _researchQueue;
    private List<string> _lore;
    private World _world;

    public DataMiner(World world)
    {
        _world = world;

        _researchQueue.Add(SOFactory.I.GetBuilding("Solar"));
        _researchQueue.Add(SOFactory.I.GetBuilding("Battery"));

        _lore.Add("The Entropy's computers had to be reprogrammed 1,338 times to continue tracking the year (25.83^38 CE).");
        _lore.Add("A multi-generational starship sent coasting into the void. Forgotten. 1.4 million souls slowly turned to stone.");

        StartMining(_researchQueue[0]);
    }

    public void StartMining(BuildingScriptable b)
    {
        _blocksRecovered = 0;
        _current = b;
    }

    public void StopMining()
    {
        _on = false;
    }    

    public void Mine()
    {
        if (_current == null) return;

        _blocksRecovered++;

        if (_blocksRecovered >= _current.researchPoints)
        {
            _world.AddNewTech(_current);            
            _researchQueue.RemoveAt(0);

            if (_researchQueue.Count == 0)
            {
                _current = null;
            }
            else
            {
                StartMining(_researchQueue[0]);
            }
        }

    }

    public string GetProgress()
    {        
        return (_current == null) ? "No data" : $"[{_blocksRecovered}/{_current.researchPoints}]";
    }

    public bool IsDataMining() => _on; 
    public void ToggleMining() => _on = !_on;
    
}
