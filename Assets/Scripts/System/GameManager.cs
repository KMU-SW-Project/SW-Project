using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData
{
    // 서버에 저장된 고유 테이블 번호
    public string dataIndate;

    // 유저 데이터
    public string userNickname;
    public int userCharacter;
    public int userClearAI;
}

public class ModeData
{
    public EnemyAI currentPlayAiData;
}

public class GameManager : MonoBehaviour
{
    #region 싱글톤
    private static GameManager instance;

    private void Awake()
    {
        if (instance != null) Destroy(instance);

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static GameManager GetInstance()
    {
        if(instance == null)
        {
            print("GameManager 인스턴스 없음");
            return null;
        }

        return instance;
    }
    #endregion

    public UserData userData = new UserData();
    public ModeData modeData = new ModeData();
}
