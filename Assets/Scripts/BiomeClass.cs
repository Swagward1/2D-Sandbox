using System.Collections;
using UnityEngine;

[System.Serializable]
public class BiomeClass
{
    public string biomeName;
    public Color biomeCol; 

    public TileAtlas tileAtlas;

    [Header("Noise Settings")]
    public Texture2D caveNoiseTexture;

    [Header("World Generation")]
    public bool caveGen = true;
    public int dirtHeight = 5;
    public float surfaceVal = .25f;
    public float heightMultiplier = 4f;

    [Header("Additional")]
    public int tallGrassChance = 10;

    [Header("Tree Customization")]
    public int treeGenChance = 15; //in percentages
    public int smallTree = 2;
    public int largeTree = 3;

    [Header("Ores")]   
    public OreClass[] ores;
}