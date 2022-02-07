using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public LayerMask layerMask;

    public int selectedSlotIndex = 0;
    public GameObject hotBarSelector;
    public GameObject heldItem;

    public bool inventoryShowing = false;
    public bool pauseMenuShowing = false;
    public GameObject pauseCanvas;

    public Inventory inventory;
    public ItemClass selectedItem;
    public WorldGen terrainGenerator;
    public OptionsPopup opts;
    
    public int playerReach;
    public Vector2Int mousePos;

    public float movementSpeed = 4f;
    public float jumpForce = 10f;
    public bool onGround;
    public bool isRunning = false;

    private Rigidbody2D rb2;
    private Animator anim;

    public float horizontal;
    public bool hit;
    public bool place;

    [HideInInspector]
    public Vector2 spawnPos;


    private void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        inventory = GetComponent<Inventory>();
        //pauseCanvas = GetComponent<GameObject>();
    }

    public void Spawn()
    {
        GetComponent<Transform>().position = spawnPos;
    }

    private void FixedUpdate()
    {
        float jump = Input.GetAxisRaw("Jump");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontal * movementSpeed, rb2.velocity.y); //gay shit

        if(isRunning)
            movementSpeed = 8f;
        else
            movementSpeed = 4f;

        //flip player on rotation
        if(horizontal > 0)
            transform.localScale = new Vector3(-1, 1, 1);

        else if(horizontal < 0)
            transform.localScale = new Vector3(1, 1, 1);

        //jumping
        if(vertical > .1f || jump > .1f)
        {
            if(onGround)
                movement.y = jumpForce; 
        }

        //autojump
        if(FootRaycast() && !HeadRaycast() && movement.x != 0)
        {
            if(onGround)
                movement.y = jumpForce * .75f; //autojump multiplier
        }

        rb2.velocity = movement;
    }

    private void Update()
    {
        //TileClass tileClass;

        horizontal = Input.GetAxis("Horizontal");

        hit = Input.GetMouseButtonDown(0);
        place = Input.GetMouseButtonDown(1);

        //scrolls through hotbar slots
        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if(Time.timeScale == 1)
            {
                //increase but dont go over the limit
                if(selectedSlotIndex < inventory.inventoryWidth - 1)
                    selectedSlotIndex += 1;
            }
        }
        else if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if(Time.timeScale == 1)
            {
                //decrease down to zero
                if(selectedSlotIndex > 0)
                selectedSlotIndex -= 1;
            }
        }

        //use numbers 1-7 to select hotbar slot
        for (int number = 0; number <= 7; number++)
        {
            if(Input.GetKeyDown(number.ToString()))
                selectedSlotIndex = (number - 1);
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) /*&& horizontal != 0*/)
            isRunning = true;
        if(Input.GetKeyUp(KeyCode.LeftShift) /*&& horizontal == 0*/)
            isRunning = false;

        //set selected slot in hotbar UI
        hotBarSelector.transform.position = inventory.hotbarUISlots[selectedSlotIndex].transform.position;
        if(selectedItem != null)
        {
            heldItem.GetComponent<SpriteRenderer>().sprite = selectedItem.sprite;
            if(selectedItem.itemType == ItemClass.ItemType.block)
                heldItem.transform.localScale = new Vector3(-.5f, .5f, .5f);

            else
                heldItem.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
            heldItem.GetComponent<SpriteRenderer>().sprite = null;

        //set selected item
        if(inventory.inventorySlots[selectedSlotIndex, inventory.inventoryHeight - 1] != null)
            selectedItem = inventory.inventorySlots[selectedSlotIndex, inventory.inventoryHeight - 1].item;
        else
            selectedItem = null;

        //open inventory
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(pauseMenuShowing == false)
                inventoryShowing = !inventoryShowing;
        }

        if (Vector2.Distance(transform.position, mousePos) <= playerReach &&
            Vector2.Distance(transform.position, mousePos) > .25f)
        {
            if (place)
            {
                if(Time.timeScale == 1)
                {
                    if (selectedItem != null)
                    {
                        if(selectedItem.itemName.ToLower().Contains("ingot"))
                            return;
                        
                        else if (selectedItem.itemType == ItemClass.ItemType.block)
                        {
                            if(Input.GetKeyDown(KeyCode.Tab))
                            {
                                //find selected item
                                //turn it into a wall variant
                                //place it with correct lighting applied
                            }

                            if(terrainGenerator.CheckTile(selectedItem.tile, mousePos.x, mousePos.y, false))
                                    inventory.Remove(selectedItem);
                        }
                    }
                }
            }
        }
        
        if(Vector2.Distance(transform.position, mousePos) <= playerReach)
        {
            if(hit)
            {
                if(Time.timeScale == 1)
                {
                    //Debug.Log(hit);
                    terrainGenerator.RemoveTileWithTool(mousePos.x, mousePos.y, selectedItem);

                }
            }
        }

        mousePos.x = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - .5f);
        mousePos.y = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - .5f);

        inventory.inventoryUI.SetActive(inventoryShowing);

        anim.SetFloat("horizontal", horizontal);
        anim.SetBool("hit", hit || place);

        PauseScreen();
    }

    public void PauseScreen()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(pauseMenuShowing == false && opts.optionsMenuShowing == false)//if both are false
            {
                if(inventoryShowing == false)//then if inv is false
                {
                    //toggle canvas on + hide hotbar
                    pauseMenuShowing = true;
                    pauseCanvas.SetActive(true);
                    inventory.hotbarUI.SetActive(false);
                }
            }
            else if(pauseMenuShowing)//else if pause is true
            {
                //toggle canvas off + show hotbar
                pauseMenuShowing = !pauseMenuShowing;
                pauseCanvas.SetActive(false);
                inventory.hotbarUI.SetActive(true);
            }

            else if(opts.optionsMenuShowing && !pauseMenuShowing) //if options is true + pause is off
            {
                //set options false and hotbar true
                opts.optionsMenuShowing = !opts.optionsMenuShowing;
                opts.optionsMenu.SetActive(false);
                inventory.hotbarUI.SetActive(true);
            }
        }
    }

    /*private void OnValidate()
    {
        Debug.DrawRay(transform.position - (Vector3.up * .5f), -Vector2.right, Color.white, 10f); //draw ray from knees
        Debug.DrawRay(transform.position + (Vector3.up * .5f), -Vector2.right, Color.white, 10f); //draw ray from head
    }*/

    public bool FootRaycast()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position - (Vector3.up * .5f), -Vector2.right * transform.localScale.x, 1f, layerMask);
        return hit;
    }

    public bool HeadRaycast()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3.up * .5f), -Vector2.right * transform.localScale.x, 1f, layerMask);
        return hit;
    }
}