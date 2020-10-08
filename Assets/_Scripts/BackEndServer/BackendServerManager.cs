using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BackEnd;
using LitJson;
using System;

public class UserInfoData
{
    public string userNickname;
}

public class BackendServerManager : MonoBehaviour
{
    private static BackendServerManager instance;
    public UserInfoData UserInfoData = new UserInfoData();

    string userAccountInfo;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static BackendServerManager GetInstance()
    {
        if (instance.Equals(null))
        {
            print("BackendServerManager 의 인스턴스가 존재하지 않음");
            return null;
        }

        return instance;
    }

    void Start()
    {
        Backend.Initialize(() =>
        {
            if (Backend.IsInitialized)
            {
                TitleManager.instance.SetTitleLog("<Color=red>서버 연결 성공</Color>\n사용 가능한 서비스\n-랭킹\n- 데이터 저장");
            }
            else
            {
                // 인터넷 연결 혹은 서버 오프라고 메시지 띄우기
                TitleManager.instance.SetTitleLog("<Color=red>서버 연결 실패</Color>\nOFFLINE");
            }
        });
    }

    #region 로그인 및 회원가입
    public bool ServerLogin(string ID)
    {
        print("로그인 시도 ID : " + ID);

        BackendReturnObject BRO = Backend.BMember.CustomLogin(ID, ID);

        if (BRO.IsSuccess())
        {
            return true;
        }
        else
        {
            AccountException(BRO.GetErrorCode());
            return false;
        }
    }

    public bool ServerSignUp(string ID)
    {
        BackendReturnObject BRO = Backend.BMember.CustomSignUp(ID, ID);

        if (BRO.IsSuccess()) return ServerLogin(ID);
        else
        {
            AccountException(BRO.GetErrorCode());
            return false;
        }
    }

    // 닉네임 유무 체크
    public bool NIcknameCheck()
    {
        BackendReturnObject BRO = Backend.BMember.GetUserInfo();

        if (BRO.IsSuccess())
        {
            JsonData data = BRO.GetReturnValuetoJSON()["row"];
            
            if (data["nickname"] == null) return false;
            else
            {
                UserInfoData.userNickname = data["nickname"].ToString();
                return true;
            }
        }

        return false;
    }

    // 닉네임 생성
    public void SetNickname(string nickname, Action<bool,string> func)
    {
        BackendReturnObject BRO = Backend.BMember.UpdateNickname(nickname);

        if (BRO.IsSuccess())
        {
            func(true, string.Empty);
            return;
        }

        func(false, string.Format(BRO.GetMessage()));
        
    }

    /// <summary>
    /// 로그인, 회원가입에 대한 예외처리
    /// </summary>
    /// <param name="exception"> 에러코드 </param>
    void AccountException(string exception)
    {
        // ID가 중복
        if (exception.Equals("DuplicatedParameterException")) print("아이디 중복");
        // 아이디, 비번 없거나 틀림
        else if (exception.Equals("BadUnauthorizedException")) print("아이디가 없습니다.");
    }
    #endregion




}
