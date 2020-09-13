using BackEnd;
using BackEnd.Tcp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendMatchManager : MonoBehaviour
{
    // 디버그 로그
    private string NOTCONNECT_MATCHSERVER = "매치 서버에 연결되어 있지 않습니다.";
    private string RECONNECT_MATCHSERVER = "매치 서버에 접속을 시도합니다.";
    private string FAIL_CONNECT_MATCHSERVER = "매치 서버 접속 실패 : {0}";
    private string SUCCESS_CONNECT_MATCHSERVER = "매치 서버 접속 성공";
    private string SUCCESS_MATCHMAKE = "매칭 성공 : {0}";
    private string SUCCESS_REGIST_MATCHMAKE = "매칭 대기열에 등록되었습니다.";
    private string FAIL_REGIST_MATCHMAKE = "매칭 실패 : {0}";
    private string CANCEL_MATCHMAKE = "매칭 신청 취소 : {0}";
    private string INVAILD_MATCHTYPE = "잘못된 매치 타입입니다.";
    private string INVALID_MODETYPE = "잘못된 모드 타입입니다.";
    private string INVALID_OPERATION = "잘못된 요청입니다\n{0}";
    private string EXCEPTION_OCCUR = "예외 발생 : {0}\n다시 매칭을 시도합니다.";

    private static BackendMatchManager instance;

    bool isConnectMatchServer = false;

    private void Awake()
    {
        if (instance != null) Destroy(instance);

        instance = this;
    }

    public static BackendMatchManager GetInstance()
    {
        if (instance.Equals(null))
        {
            print("BackendMatchManager 의 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return instance;
    }
    

    void Start()
    {
        // 매치 관련 핸들러 작동
        MatchMakingHandler();
    }

    private void Update()
    {
        if (Backend.Match.Poll() > 0)
            print("서버로부터 데이터 받음");
    }

    /// <summary>
    /// 매치 서버 접속
    /// </summary>
    public void JoinMatchServer()
    {
        ErrorInfo errorInfo;
        Backend.Match.JoinMatchMakingServer(out errorInfo);
    }

    /// <summary>
    /// 매칭 서버 관련 이벤트 핸들러
    /// </summary>
    private void MatchMakingHandler()
    {
        Backend.Match.OnJoinMatchMakingServer += (args) =>
        {
            Debug.Log("OnJoinMatchMakingServer : " + args.ErrInfo);
            ProcessAccessMatchMakingServer(args.ErrInfo);
        };
    }

    /// <summary>
    /// 매칭 서버 접속에 대한 리턴값
    /// </summary>
    /// <param name="errInfo"></param>
    private void ProcessAccessMatchMakingServer(ErrorInfo errInfo)
    {
        if (errInfo != ErrorInfo.Success)
        {
            // 접속 실패
            isConnectMatchServer = false;

            var errorLog = string.Format(FAIL_CONNECT_MATCHSERVER, errInfo.ToString());          
            Debug.Log(errorLog);
        }
        else
        {
            //접속 성공
            isConnectMatchServer = true;            
            Debug.Log(SUCCESS_CONNECT_MATCHSERVER);
        }
    }
}
