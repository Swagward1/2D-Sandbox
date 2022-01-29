using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.CompareTag("Ground"))
           transform.parent.GetComponent<PlayerControl>().onGround = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if(col.CompareTag("Ground"))
            transform.parent.GetComponent<PlayerControl>().onGround = false;
    }
}