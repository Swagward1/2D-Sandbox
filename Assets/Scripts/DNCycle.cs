using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DNCycle : MonoBehaviour
{
    [SerializeField] private Gradient lightCol;
    [SerializeField] private GameObject lightObj;

    private int days;

    public int Days => days;
    private float time = 50;
    private bool canChangeDay = true;
    
    private void Update()
    {
        if(time > 500)
        {
            time = 0;
        }

        if((int)time == 250 && canChangeDay)
        {
            canChangeDay = false;

            days++;
        }

        if ((int)time == 255)
            canChangeDay = true;

        time += Time.deltaTime;
        lightObj.GetComponent<Light2D>().color = lightCol.Evaluate(time * .002f);
    }
}