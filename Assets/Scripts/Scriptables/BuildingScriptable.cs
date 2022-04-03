using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Building")]
public class BuildingScriptable : ScriptableObject
{
    public Sprite sprite;
    public string title;
    public Tile tile;
    public int wattProduction;
    public int wattConsumption;
    public int storage;
    public int requiredMetals;
    public int producedMetals;
    public int researchPoints;
}
