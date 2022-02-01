using UnityEngine;

[CreateAssetMenu(fileName = "newtileatlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{
    [Header("Surface")]
    public TileClass grass;
    public TileClass dirt;
    public TileClass log;
    public TileClass leaves;
    public TileClass trunk;
    public TileClass tallGrass;

    [Header("Underground")]
    public TileClass stone;
    public TileClass coal;
    public TileClass iron;
    public TileClass gold;
    public TileClass diamond;
    public TileClass bedrock;

    /*[Header("Snow")]
    public TileClass snow;
    public TileClass snowCoal;
    public TileClass snowIron;
    public TileClass snowGold;
    public TileClass snowDiamond;*/
}
