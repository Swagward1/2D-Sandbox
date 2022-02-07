using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour 
{	
    public PlayerControl player;
    public Button yourButton;
    public GameObject fpsMonitor;
    public GameObject OptionsScreen;
    //public bool autoJumpEnabled;
    public bool fpsIsShowing;

	void Start () 
    {
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(FPSTaskOnClick);
        btn.onClick.AddListener(OptionsTaskOnClick);
        //btn.onClick.AddListener(AutoJumpOnTaskClick);
        
        fpsIsShowing = false;
        //autoJumpEnabled = true;
	}

    public void OptionsTaskOnClick()
    {
        player.pauseCanvas.SetActive(false);
        OptionsScreen.SetActive(true);
    }

	public void FPSTaskOnClick()
    {
        if(fpsIsShowing)
        {
		    fpsMonitor.SetActive(false);
            fpsIsShowing = false;
        }

        else 
        {
            if(!fpsIsShowing)
            {
                fpsMonitor.SetActive(true);
                fpsIsShowing = true;
            }
        }
	}

    /*public void AutoJumpOnTaskClick()
    {
        if(autoJumpEnabled)
        {
            //player.TriggerAutoJump();
            autoJumpEnabled = false;
        }

        else
        {
            if(!autoJumpEnabled)
            {
                player.TriggerAutoJump();
                autoJumpEnabled = true;
            }
        }
    }*/
}