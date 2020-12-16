using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Valve.VR;

public enum GameMode
{
    Character,
    Infinity,
    Bounty,
    Training,
    Title,
    MainMenu,
    VRTest
}

public partial class ModeManager : MonoBehaviour
{
    [Header("Secne Manage")]
    [SerializeField] private SteamVR_LoadLevel nextScene;
    [SerializeField] private GameObject player;

    [Header("Main")]
    [SerializeField] private Text nicknameText;
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject characterChangeUI;
    [SerializeField] private Button offlineCharacterButton;
    private int _selectingCharacterIndex = 0;
    private GameObject[] _character;

    [Header("Infinity")]    
    [SerializeField] private GameObject infinityUI;
    [SerializeField] private Transform[] rankList;
    [SerializeField] private Transform rankBoard;
    [SerializeField] private Text bestScoreText;

    [Header("Bounty")]
    [SerializeField] private GameObject originCharacter;
    [SerializeField] private Transform bountyAiCharacter;
    [SerializeField] private GameObject bountyModeUI;
    [SerializeField] private Text bountyText;
    public EnemyAI[] bountyAiData;
    private int _seletingAIIndex;
    private Button _bountyModeStartButton;
    private GameObject[] _aiCharacterList;

    [Header("Training")]
    [SerializeField] private GameObject trainingModeUI;
    [SerializeField] private GameObject[] targetList;
    private bool _isSecondTarget;

    private void Awake()
    {
        if (GameManager.GetInstance().playMode == GameMode.VRTest) player.SetActive(false);

        GameManager.GetInstance().SetPlayerCameraPosition(player.transform);
        GameManager.GetInstance().modeData.currentPlayAiIndex = _seletingAIIndex;

        SetOfflineMode();
        UserCharacterInit();
        BountyModeInit();
        InfinityModeInit();
    }

    private void Start()
    {
        for (int i = 0; i < bountyAiData.Length; i++)
        {
            GameManager.GetInstance().modeData.enemyData.Add(bountyAiData[i]);
        }
        GameManager.GetInstance().SetBGM(GameMode.MainMenu);
        GameManager.GetInstance().modeData.currentPlayMode = GameMode.MainMenu;
        GameManager.GetInstance().modeData.currentPlayAiIndex = -1;
        
    }

    void SetOfflineMode()
    {
        if (BackendServerManager.GetInstance().isConnected)
        {
            nicknameText.text = GameManager.GetInstance().userData.userNickname;
            return;
        }

        // 닉네임
        nicknameText.text = "OFFLINE";

        // 랭킹
        rankBoard.GetChild(2).gameObject.SetActive(true);
        rankBoard.GetChild(3).gameObject.SetActive(false);

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
                infinityUI.SetActive(flag);
                originCharacter.SetActive(!flag);
                SetRankBoard();
                break;
            case GameMode.Bounty:
                originCharacter.SetActive(!flag);
                bountyModeUI.SetActive(flag);
                bountyAiCharacter.gameObject.SetActive(flag);
                SetBountyAIModel(_seletingAIIndex);
                break;
            case GameMode.Training:
                originCharacter.SetActive(!flag);
                trainingModeUI.SetActive(flag);
                TrainingModeInit(flag);
                break;
            default:
                break;
        }
    }

    private void LoadScene(GameMode mode)
    {
        switch (mode)
        {
            case GameMode.Infinity:
                nextScene.levelName = GameMode.Infinity.ToString();
                break;
            case GameMode.Bounty:
                nextScene.levelName = GameMode.Bounty.ToString();
                break;
            case GameMode.Training:
                nextScene.levelName = GameMode.Training.ToString();
                break;
            case GameMode.Title:
                Destroy(GameObject.Find("[SteamVR]"));
                Destroy(GameObject.Find("FollowHead"));
               // Destroy(GameManager.GetInstance().player);
                //Destroy(GameManager.GetInstance().gameObject);
                nextScene.levelName = GameMode.Title.ToString();
                break;
            case GameMode.MainMenu:
                nextScene.levelName = GameMode.MainMenu.ToString();
                break;
            default:
                break;
        }

        nextScene.gameObject.SetActive(true);
    }

    #region 무한 모드
    
    void InfinityModeInit()
    {
        for (int i = 0; i < rankList.Length; i++)
            rankList[i].gameObject.SetActive(false);

        bestScoreText.text = $"<color=#FFC5E7>{GameManager.GetInstance().userData.userInfinityScore}</color>승";
    }

    public void StartInfinityMode()
    {
        LoadScene(GameMode.Infinity);
    }

    void SetRankBoard()
    {
        BackendServerManager.GetInstance().GetRankingBoard((string[] data) =>
        {
            for (int i = 0; i < data.Length; i++)
            {
                string[] userRankData = data[i].Split('|');

                rankList[i].GetChild(0).GetComponent<Text>().text = userRankData[0];
                rankList[i].GetChild(1).GetComponent<Text>().text = userRankData[1];
                rankList[i].GetChild(2).GetComponent<Text>().text = userRankData[2];

                rankList[i].gameObject.SetActive(true);
            }
        });
    
    }

    #endregion

    #region 현상금 모드
    // 현상금 모드 초기화
    void BountyModeInit()
    {
     //   EnemyAI playedAI = GameManager.GetInstance().modeData.enemyData[GameManager.GetInstance().modeData.currentPlayAiIndex];
      //  _seletingAIIndex = playedAI == null ? 0 : --playedAI.enemyID;

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
        GameManager.GetInstance().modeData.currentPlayAiIndex = index;

        for (int i = 0; i < bountyAiData.Length; i++)
            _aiCharacterList[i].SetActive(false);

        var bountyMoney = bountyAiData[index].bountyMoney;

        bountyText.text = bountyMoney == -1 ? "CLEAR" : $"$ {bountyMoney}";

        _aiCharacterList[index].SetActive(true);
    }

    // 현상금모드 시작
    public void StartAIMode()
    {
       
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

    #region 사격장 모드

    void TrainingModeInit(bool flag)
    {
        if (!flag)
        {
            for (int i = 0; i < targetList.Length; i++)
                targetList[i].SetActive(false);

            return;
        }

        _isSecondTarget = false;

        targetList[0].SetActive(true);
    }

    public void ChangeTargetObject()
    {
        _isSecondTarget = !_isSecondTarget;

        targetList[0].SetActive(!_isSecondTarget);
        targetList[1].SetActive(_isSecondTarget);
    }

    public void StartTrainingMode()
    {
        GameManager.GetInstance().modeData.currentSelectedTarget = _isSecondTarget ? 1 : 0;
        LoadScene(GameMode.Training);
    }

    #endregion
}
