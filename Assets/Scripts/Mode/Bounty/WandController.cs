using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class WandController : MonoBehaviour
{
    public LineRenderer gunLine;
    public SteamVR_Action_Boolean gunTrigger;
    public SteamVR_Input_Sources handType;

    public bool readyposition;

    void Start()
    {
        gunTrigger.AddOnStateDownListener(TriggerDown, handType);
        gunTrigger.AddOnStateUpListener(TriggerUp, handType);

        gunLine = this.GetComponent<LineRenderer>();
        gunLine.startColor = Color.red;
        gunLine.endColor = Color.yellow;
        gunLine.startWidth = 0.1f;
        gunLine.endWidth = 0.1f;

    }

    private void Update()
    {
        GunPositionChecker();
    }

    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("Trigger is up");
    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        FireGun();
        Debug.Log("Trigger is down");
    }

    void FireGun()
    {
        Debug.Log("Fired");
        
        gunLine.SetPosition(0, transform.position);
        gunLine.SetPosition(1, transform.position + transform.forward * 10f);

    }

    bool GunPositionChecker()
    {
        if (transform.rotation.x > 20 && transform.rotation.x < 60)
        {
            Debug.Log("ReadyPosition");
            return true;
        }
        return false;
    }




}
