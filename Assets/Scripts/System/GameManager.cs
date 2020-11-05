using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public string handType;

}
