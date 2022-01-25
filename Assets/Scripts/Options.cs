using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    public GameObject options;
    public bool optionsMenuShowing;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(optionsMenuShowing == false)
            {
                optionsMenuShowing = true;
                options.SetActive(true);
            }
            else if(optionsMenuShowing)
            {
                optionsMenuShowing = !optionsMenuShowing;
                options.SetActive(false);
            }
        }
    }
}
