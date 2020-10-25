using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlerReset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string handType = GameManager.GetInstance().handType;

        if (handType == "left") HandType.instance.SetHandType(Valve.VR.SteamVR_Input_Sources.LeftHand);
        else if (handType == "right") HandType.instance.SetHandType(Valve.VR.SteamVR_Input_Sources.RightHand);
    }

}
