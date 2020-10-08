using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SteamInputModule : VRInputModule
{
    public static SteamInputModule instance;

    protected override void Awake()
    {
        instance = this;
    }

    public SteamVR_Input_Sources m_Source;
    public SteamVR_Action_Boolean m_Click = null;

    public override void Process()
    {
        base.Process();


        //  Press
        if (m_Click.GetStateDown(m_Source))
            Press();
        // Release
        if (m_Click.GetStateUp(m_Source))
            Release();
    }
    
}
