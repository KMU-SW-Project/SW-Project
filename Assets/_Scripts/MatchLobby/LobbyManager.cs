using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    private static LobbyManager instance;

    public Text nicknameText;
    public GameObject loadingUI;
    Text loadingText;

    private void Awake()
    {
        if (instance != null) Destroy(instance);

        instance = this;

        if (loadingUI.activeSelf) loadingUI.SetActive(false);
    }

    public static LobbyManager GetInstance()
    {
        if(instance == null)
        {
            print("LobbyManager 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return instance;
    }

    void Start()
    {
        loadingText = loadingUI.transform.GetChild(1).GetComponent<Text>();

        BackendMatchManager.GetInstance().JoinMatchServer();

        if (BackendServerManager.GetInstance().UserInfoData.userNickname != null)
            nicknameText.text = BackendServerManager.GetInstance().UserInfoData.userNickname;
    }

    // 방 생성
    public void OpenRoomUI()
    {
        if (BackendMatchManager.GetInstance().CreateMatchRoom())
            BackendMatchManager.GetInstance().RequestMatchMaking();
    }

    // 매치 할시 UI 창 제어
    public void MatchRequestCallback(bool result)
    {
        if (!result)
        {
            loadingUI.SetActive(false);
            return;
        }

        loadingUI.SetActive(true);
    }

    // 매치 성공시 텍스트 바꿈
    public void MatchDoneCallback()
    {
        loadingText.text = "매치 상대 발견!";
    }

}
