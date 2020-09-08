using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using static BackEnd.SendQueue;

public class BackendServerManager : MonoBehaviour
{
    private static BackendServerManager instance;   // 인스턴스

    private void Awake()
    {
        if(instance != null)
            Destroy(instance);

        instance = this;

        DontDestroyOnLoad(gameObject);        
    }

    /// <summary>
    /// Get BackendServerManager Instance
    /// </summary>
    /// <returns></returns>
    public static BackendServerManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("BackendServerManager 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return instance;
    }

    // 게임 종료, 에디터 종료 시 호출
    // 비동기 큐 쓰레드를 중지시킴
    // 해당 함수는 실제 안드로이드, iOS 환경에서 호출이 안될 수도 있다 (각 os의 특징 때문)
    void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        StopSendQueue();
    }

    void Start()
    {
        Backend.Initialize(() =>
        {
            // 초기화 성공한 경우 실행
            if (Backend.IsInitialized)
            {
                StartSendQueue(true);
            }
            // 초기화 실패한 경우 실행
            else
            {

            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
