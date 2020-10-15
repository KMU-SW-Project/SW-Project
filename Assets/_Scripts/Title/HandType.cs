using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandType : MonoBehaviour
{
    public GameObject left;
    public GameObject right;

    public Canvas canvas;

    LineRenderer _LR;

    public void SetHandType(SteamVR_Input_Sources type)
    {
        SteamInputModule.instance.m_Source = type;

        if(type == SteamVR_Input_Sources.LeftHand)
        {
            _LR = left.transform.GetChild(1).GetComponent<LineRenderer>();

            canvas.worldCamera = left.transform.GetChild(1).GetComponent<Camera>();
              SteamInputModule.instance.pointer = left.transform.GetChild(1).GetComponent<Pointer>();

            _LR.startColor = Color.red;

        }
        else
        {
            _LR = right.transform.GetChild(1).GetComponent<LineRenderer>();

            canvas.worldCamera = right.transform.GetChild(1).GetComponent<Camera>();
            SteamInputModule.instance.pointer = right.transform.GetChild(1).GetComponent<Pointer>();

            _LR.startColor = Color.red;
        }
    }
}
