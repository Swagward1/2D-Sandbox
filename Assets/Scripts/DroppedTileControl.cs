using System.Collections;
using UnityEngine;

public class DroppedTileControl : MonoBehaviour
{
    public ItemClass item;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            //destroy + add to inventory
            if(col.GetComponent<Inventory>().Add(item))
            {
                Destroy(this.gameObject);
            }
        }

        if(col.gameObject.CompareTag("DeathZone"))
        {
            Debug.Log("Tile fallen through world, destroying object");
            Destroy(this.gameObject);
        }
    }
}
