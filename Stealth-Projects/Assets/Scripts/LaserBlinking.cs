using UnityEngine;
using System.Collections;

public class LaserBlinking : MonoBehaviour 
{
    public float timeOn;
    public float timeOff;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if(renderer.enabled&&timer>=timeOn)
        {
            switchBeam();
        }
        else if(!renderer.enabled&&timer>=timeOff)
        {
            switchBeam();
        }
    }

    void switchBeam()
    {
        timer = 0f;
        renderer.enabled = !renderer.enabled;
        light.enabled = !light.enabled;
    }

}
