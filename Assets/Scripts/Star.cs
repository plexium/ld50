using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{

    private SpriteRenderer _renderer;

    public int currentOutput;
    
    const int startingStarOutput = 3478;  //kW per month

    public Color whiteDwarf;
    public Color redDwarf;
    public Color brownDwarf;
    public Color blackDwarf;

    public int rdcutoff;
    public int bdcutoff;    

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        rdcutoff = startingStarOutput / 2;
        bdcutoff = rdcutoff / 2;
    }

    // Update is called once per frame
    void Update()
    {
        _renderer.color = Color.Lerp(blackDwarf, whiteDwarf, (float)currentOutput / (float) startingStarOutput);
    }
}
