using BackEnd;
using BackEnd.Tcp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackendMatchManager : MonoBehaviour
{
    // 콘솔에서 생성한 매칭 카드 정보
    public class MatchInfo
    {
        public string title;                // 매칭 명
        public string inDate;               // 매칭 inDate (UUID)
        public MatchType matchType;         // 매치 타입
        public MatchModeType matchModeType; // 매치 모드 타입
        public string headCount;            // 매칭 인원
        public bool isSandBoxEnable;        // 샌드박스 모드 (AI매칭)
    }

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

    // 게임 로그
    private string FAIL_ACCESS_INGAME = "인게임 접속 실패 : {0} - {1}";
    private string SUCCESS_ACCESS_INGAME = "유저 인게임 접속 성공 : {0}";
    private string NUM_INGAME_SESSION = "인게임 내 세션 갯수 : {0}";

    private static BackendMatchManager instance;

    bool isConnectMatchServer = false;
    bool isConnectIngameServer = false;
    private bool isJoinGameRoom = false;
    public bool isReconnectProcess { get; private set; } = false;
    public bool isSandBoxGame { get; private set; } = false;
    private string inGameRoomToken = string.Empty;  // 게임 룸 토큰 (인게임 접속 토큰)
    public List<MatchInfo> matchInfos { get; private set; } = new List<MatchInfo>();  // 콘솔에서 생성한 매칭 카드들의 리스트
    public MatchType nowMatchType { get; private set; } = MatchType.None;     // 현재 선택된 매치 타입
    public MatchModeType nowModeType { get; private set; } = MatchModeType.None; // 현재 선택된 매치 모드 타입
    public List<SessionId> sessionIdList { get; private set; }  // 매치에 참가중인 유저들의 세션 목록
    
    private int numOfClient = 2;                    // 매치에 참가한 유저의 총 수


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
        // 핸들러 작동
        MatchMakingHandler();
        OnlineHandler();
    }

    private void Update()
    {
        if (Backend.Match.Poll() > 0)
            print("서버로부터 데이터 받음");
    }

    #region 매치 서버
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
    #endregion

    #region 인게임 서버

    /// <summary>
    ///  방생성
    /// </summary>
    /// <returns></returns>
    public bool CreateMatchRoom()
    {
        if (!isConnectMatchServer)
        {
            Debug.Log(NOTCONNECT_MATCHSERVER);
            Debug.Log(RECONNECT_MATCHSERVER);
            JoinMatchServer();
            return false;
        }

        Debug.Log("방 생성 요청을 서버로 보냄");
        Backend.Match.CreateMatchRoom();
        return true;
    }

    public void RequestMatchMaking()
    {
        if (!isConnectMatchServer)
        {
            Debug.Log(NOTCONNECT_MATCHSERVER);
            Debug.Log(RECONNECT_MATCHSERVER);
            JoinMatchServer();
            return;
        }

        isConnectIngameServer = false;

        Backend.Match.RequestMatchMaking(MatchType.Random, MatchModeType.OneOnOne, "2020-09-13T14:21:51.325Z");

        if (isConnectIngameServer) Backend.Match.LeaveGameServer();



    }

    private void OnlineHandler()
    {
        Backend.Match.OnMatchMakingRoomCreate += (args) =>
        {
            Debug.Log("OnMatchMakingRoomCreate : " + args.ErrInfo + " : " + args.Reason);
        };

        Backend.Match.OnMatchMakingResponse += (args) =>
        {
            Debug.Log("OnMatchMakingResponse : " + args.ErrInfo + " : " + args.Reason);
            ProcessMatchMakingResponse(args);
        };
    }


    /*
	 * 매칭 신청에 대한 리턴값 (호출되는 종류)
	 * 매칭 신청 성공했을 때
	 * 매칭 성공했을 때
	 * 매칭 신청 실패했을 때
	*/
    private void ProcessMatchMakingResponse(MatchMakingResponseEventArgs args)
    {
        string debugLog = string.Empty;
        bool isError = false;

        switch (args.ErrInfo)
        {
            case ErrorCode.Success:
                // 매칭 성공했을 때
                debugLog = string.Format(SUCCESS_MATCHMAKE, args.Reason);
                LobbyManager.GetInstance().MatchDoneCallback();
                ProcessMatchSuccess(args);
                break;
            case ErrorCode.Match_InProgress:
                // 매칭 신청 성공했을 때 or 매칭 중일 때 매칭 신청을 시도했을 때

                // 매칭 신청 성공했을 때
                if (args.Reason == string.Empty)
                {
                    debugLog = SUCCESS_REGIST_MATCHMAKE;

                    LobbyManager.GetInstance().MatchRequestCallback(true);
                }
                break;
            case ErrorCode.Match_MatchMakingCanceled:
                // 매칭 신청이 취소되었을 때
                debugLog = string.Format(CANCEL_MATCHMAKE, args.Reason);

                LobbyManager.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_InvalidMatchType:
                isError = true;
                // 매치 타입을 잘못 전송했을 때
                debugLog = string.Format(FAIL_REGIST_MATCHMAKE, INVAILD_MATCHTYPE);

                LobbyManager.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_InvalidModeType:
                isError = true;
                // 매치 모드를 잘못 전송했을 때
                debugLog = string.Format(FAIL_REGIST_MATCHMAKE, INVALID_MODETYPE);

                LobbyManager.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.InvalidOperation:
                isError = true;
                // 잘못된 요청을 전송했을 때
                debugLog = string.Format(INVALID_OPERATION, args.Reason);
                LobbyManager.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_Making_InvalidRoom:
                isError = true;
                // 잘못된 요청을 전송했을 때
                debugLog = string.Format(INVALID_OPERATION, args.Reason);
                LobbyManager.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Exception:
                isError = true;
                // 매칭 되고, 서버에서 방 생성할 때 에러 발생 시 exception이 리턴됨
                // 이 경우 다시 매칭 신청해야 됨
                debugLog = string.Format(EXCEPTION_OCCUR, args.Reason);

                RequestMatchMaking();
                break;
        }

    }
        // 매칭 성공했을 때
        // 인게임 서버로 접속해야 한다.
        private void ProcessMatchSuccess(MatchMakingResponseEventArgs args)
        {
            ErrorInfo errorInfo;
            if (sessionIdList != null)
            {
                Debug.Log("이전 세션 저장 정보");
                sessionIdList.Clear();
            }

            if (!Backend.Match.JoinGameServer(args.RoomInfo.m_inGameServerEndPoint.m_address, args.RoomInfo.m_inGameServerEndPoint.m_port, false, out errorInfo))
            {
                var debugLog = string.Format(FAIL_ACCESS_INGAME, errorInfo.ToString(), string.Empty);
                Debug.Log(debugLog);
            }
            // 인자값에서 인게임 룸토큰을 저장해두어야 한다.
            // 인게임 서버에서 룸에 접속할 때 필요
            // 1분 내에 모든 유저가 룸에 접속하지 않으면 해당 룸은 파기된다.
            isConnectIngameServer = true;
            isJoinGameRoom = false;
            isReconnectProcess = false;
            inGameRoomToken = args.RoomInfo.m_inGameRoomToken;
            isSandBoxGame = args.RoomInfo.m_enableSandbox;
            var info = GetMatchInfo(args.MatchCardIndate);
            if (info == null)
            {
                Debug.LogError("매치 정보를 불러오는 데 실패했습니다.");
                return;
            }

            nowMatchType = info.matchType;
            nowModeType = info.matchModeType;
            numOfClient = int.Parse(info.headCount);
        }

        public MatchInfo GetMatchInfo(string indate)
        {
            var result = matchInfos.FirstOrDefault(x => x.inDate == indate);
            if (result.Equals(default(MatchInfo)) == true)
            {
                return null;
            }
            return result;
        }

        #endregion
    
}