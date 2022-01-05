using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "newtileclass", menuName = "Tile Class")]
public class TileClass : ScriptableObject
{
    public string tileName;
    public TileClass wallVariant;
    //public Sprite tileSprite;
    public Sprite[] tileSprites;
    public bool inBackdrop = false;
    public TileClass tileDrop;
    public ItemClass.ToolType toolToBreak;
    public bool naturallyPlaced = true;
    public bool isStackable;

    public static TileClass CreateInstance(TileClass tile, bool isNaturallyPlaced)
    {
        var thisTile = ScriptableObject.CreateInstance<TileClass>();
        thisTile.Init(tile, isNaturallyPlaced);

        return thisTile;
    }

    public void Init (TileClass tile, bool isNaturallyPlaced)
    {
        tileName = tile.tileName;
        wallVariant = tile.wallVariant;
        tileSprites = tile.tileSprites;
        inBackdrop = tile.inBackdrop;
        tileDrop = tile.tileDrop;
        naturallyPlaced = isNaturallyPlaced;
        toolToBreak = tile.toolToBreak;
    }
}
