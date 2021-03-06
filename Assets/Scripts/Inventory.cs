using UnityEngine.UI;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Starter Tools")]
    public ToolClass pickaxe;
    public ToolClass axe;
    //public ToolClass shovel;
    public ToolClass hammer;

    [Header("Blocks")]
    public TileClass woodPlanks;
    public TileClass stone;
    public TileClass woodBackground;
    public TileClass stoneBackground;

    [Header("Inventory Positioning")]
    public Vector2 inventoryOffset;
    public Vector2 hotbarOffset;
    public Vector2 multiplier;

    [Header("Inventory Customisation ")]
    public GameObject inventoryUI;
    public GameObject hotbarUI;
    public GameObject inventorySlotPrefab;
    public int inventoryWidth;
    public int inventoryHeight;
    public int stackLimit = 64;

    public InventorySlot[,] inventorySlots;
    public InventorySlot[] hotbarSlots;

    public GameObject[,] uiSlots;
    public GameObject[] hotbarUISlots;

    private void Start()
    {
        inventorySlots = new InventorySlot[inventoryWidth, inventoryHeight];
        uiSlots = new GameObject[inventoryWidth, inventoryHeight];
        hotbarSlots = new InventorySlot[inventoryWidth];
        hotbarUISlots = new GameObject[inventoryWidth];

        SetupUI();
        UpdateInventoryUI();
        Add(new ItemClass(pickaxe));
        Add(new ItemClass(axe));
        //Add(new ItemClass(shovel));
        Add(new ItemClass(hammer));

        for (int i = 0; i < 999; i++)
        {
            Add(new ItemClass(woodPlanks)); //give player 75 wood planks on start up
            Add(new ItemClass(stone));
            Add(new ItemClass(woodBackground));
            Add(new ItemClass(stoneBackground));

        }
    }

    void SetupUI()
    {
        //setup inventory
        for (int x = 0; x < inventoryWidth; x++)
        {
            for (int y = 0; y < inventoryHeight; y++)
            {
                GameObject inventorySlot = Instantiate(inventorySlotPrefab, inventoryUI.transform.GetChild(0).transform);
                inventorySlot.GetComponent<RectTransform>().localPosition = new Vector3((x * multiplier.x) + inventoryOffset.x, (y * multiplier.y) + inventoryOffset.y);
                uiSlots[x, y] = inventorySlot;
                inventorySlots[x, y] = null;
            }
        }

        //setup hotbar afterwards
        for (int x = 0; x < inventoryWidth; x++)
        {
            GameObject hotbarSlot = Instantiate(inventorySlotPrefab, hotbarUI.transform.GetChild(0).transform);
            hotbarSlot.GetComponent<RectTransform>().localPosition = new Vector3((x * multiplier.x) + hotbarOffset.x, hotbarOffset.y);
            hotbarUISlots[x] = hotbarSlot;
            hotbarSlots[x] = null;
        }
    }

    void UpdateInventoryUI()
    {
        //update inventory
        for (int x = 0; x < inventoryWidth; x++)
        {
            for (int y = 0; y < inventoryHeight; y++)
            {
                if (inventorySlots[x, y] == null)
                {
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().sprite = null;
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().enabled = false;

                    uiSlots[x, y].transform.GetChild(1).GetComponent<Text>().text = "0";
                    uiSlots[x, y].transform.GetChild(1).GetComponent<Text>().enabled = false;
                }

                else
                {
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().enabled = true;
                    uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().sprite = inventorySlots[x, y].item.sprite;

                    if (inventorySlots[x, y].item.itemType == ItemClass.ItemType.block)
                    {
                        if (inventorySlots[x, y].item.tile.inBackdrop)
                            uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().color = new Color(.5f, .5f, .5f);

                        else
                            uiSlots[x, y].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    }

                    uiSlots[x, y].transform.GetChild(1).GetComponent<Text>().text = inventorySlots[x, y].quantity.ToString();
                    uiSlots[x, y].transform.GetChild(1).GetComponent<Text>().enabled = true;
                }
            }
        }

        //update hotbar afterwards
        for (int x = 0; x < inventoryWidth; x++)
        {
            if (inventorySlots[x, inventoryHeight - 1] == null)
            {
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().sprite = null;
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().enabled = false;

                hotbarUISlots[x].transform.GetChild(1).GetComponent<Text>().text = "0";
                hotbarUISlots[x].transform.GetChild(1).GetComponent<Text>().enabled = false;
            }
            else
            {
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().enabled = true;
                hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().sprite = inventorySlots[x, inventoryHeight - 1].item.sprite;

                if (inventorySlots[x, inventoryHeight - 1].item.itemType == ItemClass.ItemType.block)
                {
                    if (inventorySlots[x, inventoryHeight - 1].item.tile.inBackdrop)
                        hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().color = new Color(.5f, .5f, .5f);

                    else
                        hotbarUISlots[x].transform.GetChild(0).GetComponent<Image>().color = Color.white;
                }

                hotbarUISlots[x].transform.GetChild(1).GetComponent<Text>().text = inventorySlots[x, inventoryHeight - 1].quantity.ToString();
                hotbarUISlots[x].transform.GetChild(1).GetComponent<Text>().enabled = true;
            }
        }
    }

    public bool Add(ItemClass item)
    {
        Vector2Int itemPos = Contains(item);
        bool added = false;

        if (itemPos != Vector2Int.one * -1)
        {
            inventorySlots[itemPos.x, itemPos.y].quantity += 1;
            added = true;
        }

        if (!added)
        {
            for (int y = inventoryHeight - 1; y >= 0; y--)
            {
                if (added)
                    break;

                for (int x = 0; x < inventoryWidth; x++)
                {
                    if (inventorySlots[x, y] == null)
                    {
                        //this slot is empty
                        inventorySlots[x, y] = new InventorySlot { item = item, position = new Vector2Int(x, y), quantity = 1 };
                        added = true;
                        break;
                    }
                }
            }
        }

        UpdateInventoryUI();
        return added;
    }

    public Vector2Int Contains(ItemClass item)
    {
        for (int y = inventoryHeight - 1; y >= 0; y--)
        {

            for (int x = 0; x < inventoryWidth; x++)
            {
                if (inventorySlots[x, y] != null)
                {
                    if (inventorySlots[x, y].item.itemName == item.itemName)
                    {
                        if (item.isStackable && inventorySlots[x, y].quantity < stackLimit)
                            return new Vector2Int(x, y);
                    }
                }
            }
        }

        return Vector2Int.one * -1;
    }

    public bool Remove(ItemClass item)
    {
        //subtract item from inventory
        for (int y = inventoryHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < inventoryWidth; x++)
            {
                if (inventorySlots[x, y] != null)
                {
                    if (inventorySlots[x, y].item.itemName == item.itemName)
                    {
                        //this slot is empty
                        inventorySlots[x, y].quantity -= 1;

                        if (inventorySlots[x, y].quantity == 0)
                            inventorySlots[x, y] = null;

                        UpdateInventoryUI();
                        return true;
                    }
                }
            }
        }
        return false;
    }
}