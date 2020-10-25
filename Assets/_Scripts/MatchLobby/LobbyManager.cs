using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    private static LobbyManager instance;

    public Text nicknameText;
    Text loadingText;

    private void Awake()
    {
        if (instance != null) Destroy(instance);

        instance = this;
        
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
        nicknameText.text = BackendServerManager.GetInstance().GetNickname();
    }

    public void GoTitle()
    {
        SceneManager.LoadSceneAsync("Title");
    }

}
