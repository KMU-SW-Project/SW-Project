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

public class TitleManager : MonoBehaviour
{
    public static TitleManager instance;

    [Header("UI")]
    public Transform UI;
    public Text mainText;
    public bool uiOn = false;
    public Button startButton;

    private GameObject _optionUI;
    private GameObject _selectHandUI;
    private GameObject _nicknameUI;
    private GameObject _keyboardUI;
    private GameObject _errorUI;
    private Text _errorText;

    // 로컬 데이터 경로
    private string _path;

    private void Awake()
    {
        instance = this;

        _path = Application.persistentDataPath + "/Handdata.txt";

        _optionUI = UI.GetChild(0).gameObject;
        _selectHandUI = UI.GetChild(1).gameObject;
        _nicknameUI = UI.GetChild(2).gameObject;
        _keyboardUI = UI.GetChild(3).gameObject;
        _errorUI = UI.GetChild(4).gameObject;

        _errorText = _errorUI.transform.GetChild(2).GetComponent<Text>();

        HandCheck();
    }

    // 주 손을 선택했는지 판단
    void HandCheck()
    {
        if (BackendServerManager.GetInstance().CheckFile())
        {
            if (BackendServerManager.GetInstance().LoadJsonFile())
            {
                var userHandType = BackendServerManager.GetInstance().accountData.handType;

                if (userHandType == HandType.left.ToString()) GameManager.GetInstance().SetUserControllerModel(HandType.left);
                else GameManager.GetInstance().SetUserControllerModel(HandType.right);
            }
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
            if (type == 0) BackendServerManager.GetInstance().accountData.handType = HandType.left.ToString();
            else BackendServerManager.GetInstance().accountData.handType = HandType.right.ToString();

            BackendServerManager.GetInstance().CreateJsonFile();

            PassiveUI(UIList.selectHand);

            HandCheck();
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
                _optionUI.SetActive(true);
                break;
            case UIList.selectHand:
                _selectHandUI.SetActive(true);
                break;
            case UIList.nickname:
                _nicknameUI.SetActive(true);
                _keyboardUI.SetActive(true);
                break;
            case UIList.error:
                _errorUI.SetActive(true);
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
                _optionUI.SetActive(false);
                break;
            case UIList.selectHand:
                _selectHandUI.SetActive(false);
                break;
            case UIList.nickname:
                _nicknameUI.SetActive(false);
                _keyboardUI.SetActive(false);
                break;
            case UIList.error:
                _errorUI.SetActive(false);
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
                Button button = _nicknameUI.transform.GetChild(3).GetComponent<Button>();
                button.interactable = value;
                break;
            default:
                break;
        }
    }

    // 상태 메시지 혹은 에러 메시지 셋팅
    public void SetTitleLog(string text) => mainText.text = text;
    public void SerErrorMessage(string error) => _errorText.text = error;

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
