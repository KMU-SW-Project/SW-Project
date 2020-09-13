using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BackEnd;

public class BackendServerManager : MonoBehaviour
{
    private static BackendServerManager instance;
    
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
                print("서버 연결 : 정상");
            }
            else
            {
                // 인터넷 연결 혹은 서버 오프라고 메시지 띄우기
                print("서버 연결 : 실패");
            }
        });
    }

    #region 로그인 및 회원가입
    public bool ServerLogin(string ID)
    {
        print("로그인 시도 : " + ID);

        BackendReturnObject BRO = Backend.BMember.CustomLogin(ID, ID);

        if (BRO.IsSuccess())
        {
            print("== 로그인 성공 ==");
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

        if (BRO.IsSuccess()) ServerLogin(ID);
        else
        {
            AccountException(BRO.GetErrorCode());
            return false;
        }
        return true;
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
