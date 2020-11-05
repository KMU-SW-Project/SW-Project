using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BackEnd;
using LitJson;
using System;

public class UserInfoData
{
    public string userNickname;
    public int userCharacter;
    public string charIndate;
}

public partial class BackendServerManager : MonoBehaviour
{
    #region 싱글톤
    private static BackendServerManager instance;

    private void Awake()
    {
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
    #endregion

    #region const

    // 서버 상태
    const string DEBUG_SERVER_ONLINE = "<Color=red>서버 연결 성공</Color>\n사용 가능한 서비스\n-랭킹\n- 데이터 저장";
    const string DEBUG_SERVER_OFFLINE = "<Color=red>서버 연결 실패</Color>\nOFFLINE";

    // 데이터
    const string DATA_SERVER_JSONROWS = "rows";
    const string DATA_SERVER_TABLENAME = "characterInfo";
    const string DATA_SERVER_SCHEMANAME_CHARACTER = "character";
    const string DATA_SERVER_PLEYER_NICKNAME = "nickname";

    #endregion

    public UserInfoData UserInfoData = new UserInfoData();

    string userAccountInfo;
    public bool isConnected = false;

    void Start()
    {
        Backend.Initialize(() =>
        {
            if (Backend.IsInitialized)
            {
                isConnected = true;
                TitleManager.instance.SetTitleLog(DEBUG_SERVER_ONLINE);
            }
            else
            {
                isConnected = false;
                TitleManager.instance.SetTitleLog(DEBUG_SERVER_OFFLINE);
            }
        });
    }

    #region 로그인 및 회원가입
    // 로그인
    public bool ServerLogin(string ID)
    {
        print("로그인 시도 ID : " + ID);

        BackendReturnObject BRO = Backend.BMember.CustomLogin(ID, ID);

        if (BRO.IsSuccess()) return true;
        else return ServerSignUp(ID);

    }

    // 회원가입
    public bool ServerSignUp(string ID)
    {
        BackendReturnObject BRO = Backend.BMember.CustomSignUp(ID, ID);

        if (BRO.IsSuccess()) return ServerLogin(ID);
        else return false;
    }

    // 닉네임 유무 체크
    public bool NIcknameCheck()
    {
        BackendReturnObject BRO = Backend.BMember.GetUserInfo();

        if (BRO.IsSuccess())
        {
            JsonData data = BRO.GetReturnValuetoJSON()["row"];

            if (data[DATA_SERVER_PLEYER_NICKNAME] == null) return false;
            else
            {
                UserInfoData.userNickname = data[DATA_SERVER_PLEYER_NICKNAME].ToString();
                return true;
            }
        }

        return false;
    }

    // 닉네임 생성
    public void SetNickname(string nickname, Action<bool, string> func)
    {
        BackendReturnObject BRO = Backend.BMember.UpdateNickname(nickname);

        if (BRO.IsSuccess())
        {
            func(true, string.Empty);
            return;
        }

        func(false, string.Format(BRO.GetMessage()));

    }

    // 닉네임 가져오기
    public string GetNickname()
    {
        return UserInfoData.userNickname;
    }
    #endregion

    #region 데이터 통신
    // 계정에 기본적인 데이터 정보가 있는지 확인
    public void CheckTableRow(Action<bool> func)
    {
        Param where = new Param(); // 검색용 where절 생성
        BackendReturnObject bro = Backend.GameSchemaInfo.Get(DATA_SERVER_TABLENAME, where, 10);

        if (bro.GetErrorCode() == "NotFoundException")
            func(CreateDataRow());
        else if (bro.IsSuccess())
        {
            UserInfoData.charIndate = bro.GetReturnValuetoJSON()[DATA_SERVER_JSONROWS][0]["inDate"]["S"].ToString();

            bro = Backend.GameSchemaInfo.Get(DATA_SERVER_TABLENAME, UserInfoData.charIndate);

            string data = bro.GetReturnValuetoJSON()[DATA_SERVER_JSONROWS][0][DATA_SERVER_SCHEMANAME_CHARACTER]["N"].ToString();

            UserInfoData.userCharacter = int.Parse(data);

            func(true);
        }
    }

    // 계졍에 기본적인 데이터 삽입
    bool CreateDataRow()
    {
        Param data = new Param();
        data.Add(DATA_SERVER_SCHEMANAME_CHARACTER, 0);

        BackendReturnObject bro = Backend.GameSchemaInfo.Insert(DATA_SERVER_TABLENAME, data);
        if (bro.IsSuccess())
        {
            UserInfoData.userCharacter = 0;
            print("생성 성공" + bro.GetErrorCode());
            return true;
        }
        else
        {
            print(bro.GetErrorCode());
            return false;
        }
    }

    // 데이터 수정
    public void SetData(string tableName, Action<bool> func)
    {
        string inDate = string.Empty;
        Param param = new Param();

        param.Add(DATA_SERVER_SCHEMANAME_CHARACTER, UserInfoData.userCharacter);
        inDate = UserInfoData.charIndate;

        BackendReturnObject bro = Backend.GameSchemaInfo.Update(tableName, inDate, param);

        func(bro.IsSuccess());
    }
    #endregion
}
