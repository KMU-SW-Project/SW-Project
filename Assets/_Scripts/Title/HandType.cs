using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandType : MonoBehaviour
{
    public static HandType instance;

    public GameObject left;
    public GameObject right;

    public Canvas canvas;

    LineRenderer _LR;

    private void Awake()
    {
        instance = this;
    }

    void GetAllComponent()
    {
        left = GameObject.Find("Controller (left)");
        right = GameObject.Find("Controller (right)");
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    public void SetHandType(SteamVR_Input_Sources type)
    {
        GetAllComponent();

        SteamInputModule.instance.m_Source = type;

        if (type == SteamVR_Input_Sources.LeftHand)
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
