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
    public GameObject selectHandUI;
    public GameObject nicknameUI;
    public GameObject keyboard;
    public GameObject errorUI;
    public Text mainText;
    Text errorText;
    public bool uiOn = false;

    private void Awake()
    {
        instance = this;

        errorText = errorUI.transform.GetChild(2).GetComponent<Text>();
    }

    public void ActiveOnUI(string type)
    {
        if (uiOn) return;

        uiOn = true;
        switch (type)
        {
            case "Option":
                optionUI.SetActive(true);
                break;
            case "SelectHand":
                selectHandUI.SetActive(true);
                break;
            case "Nickname":
                nicknameUI.SetActive(true);
                keyboard.SetActive(true);
                break;
            case "Error":
                errorUI.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void ActiveOff(string type)
    {
        uiOn = false;

        switch (type)
        {
            case "Option":
                optionUI.SetActive(false);
                break;
            case "SelectHand":
                selectHandUI.SetActive(false);
                break;
            case "Nickname":
                nicknameUI.SetActive(false);
                keyboard.SetActive(false);
                break;
            case "Error":
                errorUI.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void ButtonOff(string type, bool value)
    {
        switch (type)
        {
            case "Nickname":
                Button button = nicknameUI.transform.GetChild(3).GetComponent<Button>();
                button.interactable = value;
                break;
            default:
                break;
        }
    }

    public void ExitGame()
    {
        if (uiOn) return;
        Application.Quit();
    }

    public void SerErrorMessage(string error) => errorText.text = error;

    public void SetHandType(int type)
    {
        string path = Application.persistentDataPath + "/Handdata.txt";

        // left // right
        if (type == 0)
        {
            File.WriteAllText(path, "left");
        }
        else
        {
            File.WriteAllText(path, "right");
        }

        ActiveOff("SelectHand");
    }

    public void SetTitleLog(string text)
    {
        mainText.text = text;
    }

}
