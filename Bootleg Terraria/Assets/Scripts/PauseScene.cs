using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScene : MonoBehaviour
{
    [HideInInspector]
    public PlayerControl player;

    public void Pause()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(player.inventoryShowing == false)
            {
                if(Time.timeScale == 1)
                    Time.timeScale = 0;
                    //Debug.Log("Paused");

                else if(Time.timeScale == 0)
                    Time.timeScale = 1;
                    //Debug.Log("Unpaused");
            }
        }
    }
}