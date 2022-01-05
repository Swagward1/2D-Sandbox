using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "newtileatlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    [Header("Surface")]
    public TileClass grass;
    public TileClass dirt;
    public TileClass stone;
    public TileClass log;
    public TileClass leaves;
    public TileClass trunk;
    public TileClass tallGrass;
    public TileClass bedrock;

    [Header("Ores")]
    public TileClass coal;
    public TileClass iron;
    public TileClass gold;
    public TileClass diamond;
}
