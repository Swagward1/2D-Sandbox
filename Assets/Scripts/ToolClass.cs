using UnityEngine;

[CreateAssetMenu(fileName = "ToolClass", menuName = "Tool Class")]
public class ToolClass : ScriptableObject
{
    /*public enum ToolTier
    {
        wood,
        stone,
        iron,
        gold,
        diamond
    };
    public ToolTier toolTier;*/

    public string toolName;
    public Sprite sprite;
    public ItemClass.ToolType toolType;
}