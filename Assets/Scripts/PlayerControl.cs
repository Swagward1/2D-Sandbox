using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public LayerMask layerMask;

    public int selectedSlotIndex = 0;
    public GameObject hotBarSelector;
    public GameObject heldItem;

    public Inventory inventory;
    public bool inventoryShowing = false;

    public ItemClass selectedItem;

    public int playerReach;
    public Vector2Int mousePos;

    public float movementSpeed = 2.5f;
    public float jumpForce = 2.5f;
    public bool onGround;

    private Rigidbody2D rb2;
    private Animator anim;

    public float horizontal;
    public bool hit;
    public bool place;

    [HideInInspector]
    public Vector2 spawnPos;
    public WorldGen terrainGenerator;

    private void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        inventory = GetComponent<Inventory>();
    }

    public void Spawn()
    {
        GetComponent<Transform>().position = spawnPos;
    }

    private void FixedUpdate()
    {
        float jump = Input.GetAxisRaw("Jump");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontal * movementSpeed, rb2.velocity.y); //error

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
        horizontal = Input.GetAxis("Horizontal");

        hit = Input.GetMouseButton(0);
        place = Input.GetMouseButton(1);

        //scrolls through hotbar slots
        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //increase but dont go over the limit
            if(selectedSlotIndex < inventory.inventoryWidth - 1)
                selectedSlotIndex += 1;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //decrease down to zero
            if(selectedSlotIndex > 0)
                selectedSlotIndex -= 1;
        }

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

        if(Input.GetKeyDown(KeyCode.E))
        {
            inventoryShowing = !inventoryShowing;
        }

        if (Vector2.Distance(transform.position, mousePos) <= playerReach &&
            Vector2.Distance(transform.position, mousePos) > .25f)
        {
            if (place)
            {
                if (selectedItem != null)
                {
                    if (selectedItem.itemType == ItemClass.ItemType.block)
                    {
                        if (terrainGenerator.CheckTile(selectedItem.tile, mousePos.x, mousePos.y, false))
                            inventory.Remove(selectedItem);
                    }
                }
            }
        }
        
        if(Vector2.Distance(transform.position, mousePos) <= playerReach)
        {
            if(hit)
            {
                Debug.Log(hit);
                //terrainGenerator.RemoveTile(mousePos.x, mousePos.y); //not needed
                terrainGenerator.RemoveTileWithTool(mousePos.x, mousePos.y, selectedItem);
            }
        }

        mousePos.x = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x - .5f);
        mousePos.y = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - .5f);

        inventory.inventoryUI.SetActive(inventoryShowing);

        anim.SetFloat("horizontal", horizontal);
        anim.SetBool("hit", hit || place);
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