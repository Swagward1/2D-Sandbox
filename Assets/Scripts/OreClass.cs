using System.Collections;
using UnityEngine;

[System.Serializable]
public class OreClass
{
    public string name;
    [Range(0, 1)]
    public float frequency;
    [Range(0, 1)]
    public float size;
    public int maxSpawnHeight;
    public Texture2D distributionTexture;
}
