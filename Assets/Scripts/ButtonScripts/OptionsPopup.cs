using System.Collections;
using UnityEngine;

public class OptionsPopup : MonoBehaviour
{
    public GameObject optionsMenu;
    public PlayerControl player;
    public bool optionsMenuShowing;

    public void OptionsButtonClicked()
    {
        if(player.pauseMenuShowing)
        {
            player.pauseMenuShowing = !player.pauseMenuShowing;
            optionsMenuShowing = true;
            Time.timeScale = 0;

            player.pauseCanvas.SetActive(false);
            optionsMenu.SetActive(true);
        }
    }
}
