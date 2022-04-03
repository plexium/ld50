using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName ="Plot")]
public class PlotScriptable : ScriptableObject
{
    public Sprite sprite;
    public string title;
    public Tile tile;
}
