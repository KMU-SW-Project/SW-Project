using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class TitleManager : MonoBehaviour
{
    public static TitleManager instance;

    [Header("UI")]
    public GameObject optionUI;
    public Text mainText;

    [Header("Component")]
    public HandType handType;

    private void Awake()
    {
        instance = this;
    }

    public void ActiveOnUI(string type)
    {
        switch (type)
        {
            case "Option":
                optionUI.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void ActiveOff()
    {
        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetHandType(int type)
    {
        string path = Application.persistentDataPath + "/Handdata.txt";
        
        // left // right
        if (type == 0)
        {
            handType.SetHandType(Valve.VR.SteamVR_Input_Sources.LeftHand);
            File.WriteAllText(path, "left");
        }
        else
        {
            handType.SetHandType(Valve.VR.SteamVR_Input_Sources.RightHand);
            File.WriteAllText(path, "right");
        }

        ActiveOff();
    }

    public void SetTitleLog(string text)
    {
        mainText.text = text;
    }

}
