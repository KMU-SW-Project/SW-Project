﻿using System.Collections;
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

public partial class ModeManager : MonoBehaviour
{
    [Header("UI")]
    public Text nicknameText;
    public GameObject mainUI;
    public GameObject characterChangeUI;
    public Button offlineCharacterButton;

    [Header("Bounty")]
    public GameObject originCharacter;
    public Transform bountyAiCharacter;
    public GameObject bountyModeUI;
    public Text bountyText;
    public EnemyAI[] bountyAiData;

    // 현상금 모드
    private int _seletingAIIndex;
    private Button _bountyModeStartButton;
    private GameObject[] _aiCharacterList;

    // 캐릭터 변경
    private int _selectingCharacterIndex = 0;
    private GameObject[] _character;

    private void Awake()
    {
        SetOfflineMode();
        UserCharacterInit();
        BountyModeInit();
    }

    void SetOfflineMode()
    {
        if (BackendServerManager.GetInstance().isConnected)
        {
            nicknameText.text = GameManager.GetInstance().userData.userNickname;
            return;
        }

        nicknameText.text = "OFFLINE";
        
            offlineCharacterButton.interactable = false;
    }

    // ui 활성화 및 비활성화
    public void UIButtonEvent(string typeAndSwitch)
    {
        int split = typeAndSwitch.IndexOf("|");
        string type = typeAndSwitch.Substring(0, split);
        string switchValue = typeAndSwitch.Substring(++split);

        bool flag = switchValue == "on" ? true : false;

        if (type == GameMode.Character.ToString()) UiManager(GameMode.Character, flag);
        else if (type == GameMode.Infinity.ToString()) UiManager(GameMode.Infinity, flag);
        else if (type == GameMode.Bounty.ToString()) UiManager(GameMode.Bounty, flag);
        else if (type == GameMode.Training.ToString()) UiManager(GameMode.Training, flag);
        else LoadScene(GameMode.Title);
    }

    void UiManager(GameMode modeName, bool flag)
    {
        mainUI.SetActive(!flag);

        switch (modeName)
        {
            case GameMode.Character:
                if (!flag) _selectingCharacterIndex = GameManager.GetInstance().userData.userCharacter;
                characterChangeUI.SetActive(flag);
                SetCharacter(_selectingCharacterIndex);
                break;
            case GameMode.Infinity:
                break;
            case GameMode.Bounty:
                originCharacter.SetActive(!flag);
                bountyModeUI.SetActive(flag);
                bountyAiCharacter.gameObject.SetActive(flag);                
                SetBountyAIModel(_seletingAIIndex);
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
    void BountyModeInit()
    {
        EnemyAI playedAI = GameManager.GetInstance().modeData.currentPlayAiData;
        _seletingAIIndex = playedAI == null ? 0 : --playedAI.enemyID;

        _aiCharacterList = new GameObject[bountyAiData.Length];

        for (int i = 0; i < bountyAiData.Length; i++)
        {
            GameObject _object = Instantiate(bountyAiData[i].model, bountyAiCharacter);

            _object.transform.localScale = bountyAiData[i].scale;
            _object.SetActive(false);
            _aiCharacterList[i] = _object;
        }

        // 클리어 한 AI 데이터 수정
        for (int i = 0; i < GameManager.GetInstance().userData.userClearAI; i++)
            bountyAiData[i].bountyMoney = -1;

    }

    // 현상금 모드에서 AI 변경
    public void ChangeAI(bool right)
    {
        if (right) _seletingAIIndex = _seletingAIIndex == _aiCharacterList.Length - 1 ? 0 : ++_seletingAIIndex;
        else _seletingAIIndex = _seletingAIIndex == 0 ? _aiCharacterList.Length - 1 : --_seletingAIIndex;

        SetBountyAIModel(_seletingAIIndex);
    }

    // 현상금 AI 정보를 UI에 셋팅
    void SetBountyAIModel(int index)
    {
        for (int i = 0; i < bountyAiData.Length; i++)
            _aiCharacterList[i].SetActive(false);

        var bountyMoney = bountyAiData[index].bountyMoney;

        bountyText.text = bountyMoney == -1 ? "CLEAR" : $"$ {bountyMoney}";
        
        _aiCharacterList[index].SetActive(true);
    }

    // 현상금모드 시작
    public void StartAIMode()
    {
        GameManager.GetInstance().modeData.currentPlayAiData = bountyAiData[_seletingAIIndex];
        LoadScene(GameMode.Bounty);
    }
    #endregion

    #region 캐릭터 변경
    // 캐릭터 초기화
    void UserCharacterInit()
    {
        _character = new GameObject[originCharacter.transform.childCount];

        for (int i = 0; i < originCharacter.transform.childCount; i++)
            _character[i] = originCharacter.transform.GetChild(i).gameObject;

        _selectingCharacterIndex = GameManager.GetInstance().userData.userCharacter;

        SetCharacter(_selectingCharacterIndex);
    }

    // 캐릭터 선택
    public void SelectedCharacter()
    {
        GameManager.GetInstance().userData.userCharacter = _selectingCharacterIndex;

        BackendServerManager.GetInstance().SetData(GameMode.Character, (bool result) =>
        {
            if (result) UiManager(GameMode.Character, false);
            else print("[캐릭터 저장] 예기치 못한 에러로 저장 못함");
        });
    }

    // 캐릭터 좌우 변경
    public void changeCharater(bool right)
    {
        if (right) _selectingCharacterIndex = (_selectingCharacterIndex == _character.Length - 1) ? 0 : ++_selectingCharacterIndex;
        else _selectingCharacterIndex = (_selectingCharacterIndex == 0) ? _character.Length - 1 : --_selectingCharacterIndex;

        SetCharacter(_selectingCharacterIndex);
    }

    // 캐릭터 셋팅
    void SetCharacter(int charType)
    {
        for (int i = 0; i < _character.Length; i++)
            _character[i].SetActive(false);

        _character[charType].SetActive(true);
    }
    #endregion

}
