using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameMode
{
    Character,
    Infinity,
    Bounty,
    Training,
    Title,
    MainMenu
}

public class ModeManager : MonoBehaviour
{
    public GameObject mainUI;

    [Header("Bounty")]
    public GameObject originCharacter;
    public Transform bountyAiCharacter;
    public GameObject bountyModeUI;
    public Text bountyText;
    public EnemyAI[] bountyAiData;

    private Button _bountyModeStartButton;
    private GameObject[] _aiCharacterList;

    private void Awake()
    {
        BountyModeInit();
    }

    // ui 활성화 및 비활성화
    public void UIButtonEvent(string typeAndSwitch)
    {
        int split = typeAndSwitch.IndexOf("|");
        string _type = typeAndSwitch.Substring(0, split);
        string _switch = typeAndSwitch.Substring(split);

        bool _flag = _switch == "on" ? true : false;

        if (_type == GameMode.Character.ToString()) UiManager(GameMode.Character, _flag);
        else if (_type == GameMode.Infinity.ToString()) UiManager(GameMode.Infinity, _flag);
        else if (_type == GameMode.Bounty.ToString()) UiManager(GameMode.Bounty, _flag);
        else UiManager(GameMode.Training, _flag);
    }

    void UiManager(GameMode modeName, bool flag)
    {
        switch (modeName)
        {
            case GameMode.Character:
                break;
            case GameMode.Infinity:
                break;
            case GameMode.Bounty:
                mainUI.SetActive(!flag);
                originCharacter.SetActive(!flag);
                bountyModeUI.SetActive(flag);
                bountyAiCharacter.gameObject.SetActive(flag);
                break;
            case GameMode.Training:
                break;
            default:
                break;
        }
    }

    public void LoadScene(GameMode mode)
    {
        switch (mode)
        {
            case GameMode.Infinity:
                SceneManager.LoadSceneAsync(GameMode.Infinity.ToString());
                break;
            case GameMode.Bounty:
                SceneManager.LoadSceneAsync(GameMode.Bounty.ToString());
                break;
            case GameMode.Training:
                SceneManager.LoadSceneAsync(GameMode.Training.ToString());
                break;
            case GameMode.Title:
                SceneManager.LoadSceneAsync(GameMode.Title.ToString());
                break;
            case GameMode.MainMenu:
                SceneManager.LoadSceneAsync(GameMode.MainMenu.ToString());
                break;
            default:
                break;
        }
    }

    #region 현상금 모드
    // 현상금 모드 초기화
    void BountyModeInit(bool reset = false)
    {
        if (reset)
        {
            return;
        }

        GameManager.GetInstance().modeData.currentPlayAI = 0;

        _aiCharacterList = new GameObject[bountyAiCharacter.childCount];

        for (int i = 0; i < bountyAiData.Length; i++)
        {
            GameObject _object = Instantiate(bountyAiData[i].model, bountyAiCharacter);

            _object.transform.localScale = bountyAiData[i].scale;
            _object.SetActive(false);
            _aiCharacterList[i] = _object;
        }
    }
    // 현상금 모드에서 AI 변경
    // 왼쪽 0 오른쪽 1
    public void ChangeAI(bool right)
    {
        int currentPlayAI = GameManager.GetInstance().modeData.currentPlayAI;

        _aiCharacterList[currentPlayAI].SetActive(false);

        if (!right)
        {
            if (currentPlayAI == 0)
                GameManager.GetInstance().modeData.currentPlayAI = _aiCharacterList.Length - 1;
            else GameManager.GetInstance().modeData.currentPlayAI--;
        }
        else 
        {
            if (currentPlayAI == _aiCharacterList.Length - 1)
                GameManager.GetInstance().modeData.currentPlayAI = 0;
            else GameManager.GetInstance().modeData.currentPlayAI++;
        }
        //else
        //{
        //    AIMode.currentAI = GameManager.GetInstance().userData.userClearAI;

        //    for (int i = 0; i < AIMode.currentAI; i++)
        //        AIMode.Money[i] = -1;
        //}

        var bountyMoney = bountyAiData[currentPlayAI].bountyMoney;

        if (bountyMoney != -1) bountyText.text = $"$ {bountyMoney}";
        else bountyText.text = "CLEAR";

        _aiCharacterList[currentPlayAI].SetActive(true);
    }
    

    public void StartAIMode()
    {
        GameManager.GetInstance().currentAIStage = GameManager.GetInstance().modeData.currentPlayAI;
        LoadScene(GameMode.Bounty);
    }
    #endregion


    // 로드 한 데이터를 기반으로 모든 모드를 초기화
    public void ReSetMode()
    {
       // ChangeAI(3);
    }
}
