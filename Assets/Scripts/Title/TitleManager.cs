using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public enum UIList
{
    option,
    selectHand,
    nickname,
    error,
}

public enum HandType
{
    left,
    right,
}

public class TitleManager : MonoBehaviour
{
    public static TitleManager instance;

    [Header("UI")]
    public Transform UI;
    public Text mainText;
    public bool uiOn = false;
    GameObject optionUI;
    GameObject selectHandUI;
    GameObject nicknameUI;
    GameObject keyboardUI;
    GameObject errorUI;
    Text errorText;

    // 로컬 데이터 경로
    string path;

    private void Awake()
    {
        instance = this;

        path = Application.persistentDataPath + "/Handdata.txt";

        optionUI = UI.GetChild(0).gameObject;
        selectHandUI = UI.GetChild(1).gameObject;
        nicknameUI = UI.GetChild(2).gameObject;
        keyboardUI = UI.GetChild(3).gameObject;
        errorUI = UI.GetChild(4).gameObject;

        errorText = errorUI.transform.GetChild(2).GetComponent<Text>();

        HandCheck();
    }

    // 주 손을 선택했는지 판단
    void HandCheck()
    {
        if (BackendServerManager.GetInstance().CheckFile())
        {
            BackendServerManager.GetInstance().LoadJsonFile();
            print(BackendServerManager.GetInstance().accountData.handType);
            //string handType = File.ReadAllText(path);
            //GameManager.GetInstance().handType = handType;
            //print(handType);
            // 추후 작업 예정
            // 왼손 오른손 선택한 쪽에 총을 쥐어줌
            //if (handType == "left") HandType.instance.SetHandType(Valve.VR.SteamVR_Input_Sources.LeftHand);
            //else if (handType == "right") HandType.instance.SetHandType(Valve.VR.SteamVR_Input_Sources.RightHand);
            //else ActiveOnUI("SelectHand");            
        }
        else
        {
            ActiveUI(UIList.selectHand);
        }
    }



    public void ExitGame()
    {
        if (uiOn) return;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetHandType(int type)
    {
        try
        {
            if (type == 0) BackendServerManager.GetInstance().accountData.handType= HandType.left.ToString();// File.WriteAllText(path, HandType.left.ToString());
            else BackendServerManager.GetInstance().accountData.handType = HandType.right.ToString();// File.WriteAllText(path, HandType.right.ToString());

            BackendServerManager.GetInstance().CreateJsonFile();

            PassiveUI(UIList.selectHand);
        }
        catch (System.Exception e)
        {
            print("손 타입 파일로 쓰기 부분");
            print(e.Message);
        }
    }

    #region UI
    // UI 활성화
    public void ActiveUI(UIList list)
    {
        if (uiOn) return;

        uiOn = true;
        switch (list)
        {
            case UIList.option:
                optionUI.SetActive(true);
                break;
            case UIList.selectHand:
                selectHandUI.SetActive(true);
                break;
            case UIList.nickname:
                nicknameUI.SetActive(true);
                keyboardUI.SetActive(true);
                break;
            case UIList.error:
                errorUI.SetActive(true);
                break;
            default:
                break;
        }
    }

    // UI 비활성화
    public void PassiveUI(UIList list)
    {
        uiOn = false;

        switch (list)
        {
            case UIList.option:
                optionUI.SetActive(false);
                break;
            case UIList.selectHand:
                selectHandUI.SetActive(false);
                break;
            case UIList.nickname:
                nicknameUI.SetActive(false);
                keyboardUI.SetActive(false);
                break;
            case UIList.error:
                errorUI.SetActive(false);
                break;
            default:
                break;
        }
    }

    // 버튼 비활성화
    public void ButtonOff(UIList list, bool value)
    {
        switch (list)
        {
            case UIList.nickname:
                Button button = nicknameUI.transform.GetChild(3).GetComponent<Button>();
                button.interactable = value;
                break;
            default:
                break;
        }
    }

    // 상태 메시지 혹은 에러 메시지 셋팅
    public void SetTitleLog(string text) => mainText.text = text;
    public void SerErrorMessage(string error) => errorText.text = error;

    // 옵션 활성화
    public void ActiveOption(int type)
    {
        if (type == 0) ActiveUI(UIList.option);
        else PassiveUI(UIList.option);
    }

    // 데이터 폴더 오픈
    [ContextMenu("Show in Explorer")]
    void ShowExplorer() => System.Diagnostics.Process.Start(Application.persistentDataPath);
    #endregion
}
