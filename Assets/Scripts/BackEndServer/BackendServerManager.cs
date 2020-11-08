using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BackEnd;
using LitJson;
using System;
using System.IO;

public enum GameMode
{
    Character,
    Infinity,
    vsAI,
    Training
}

public class UserInfoData
{
    public string userNickname;
    public int userCharacter;
    public int userClearAI;
    public string charIndate;
}

[System.Serializable]
public  class AccountData
{
    public string userID;
    public string handType;
}

public class BackendServerManager : MonoBehaviour
{
    #region 싱글톤
    private static BackendServerManager instance;

    private void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;
        path = Application.dataPath + "/data.json";

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
    const string DATA_SERVER_SCHEMANAME_AI = "vsAI";
    const string DATA_SERVER_SCHEMANAME_INFINITY = "infinity";
    const string DATA_SERVER_PLEYER_NICKNAME = "nickname";
    #endregion
    
    public UserInfoData UserInfoData = new UserInfoData();
    public AccountData accountData = new AccountData();

    string userAccountInfo;
    public bool isConnected = false;
    string path;

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

        try
        {
            BackendReturnObject BRO = Backend.BMember.CustomLogin(ID, ID);

            if (BRO.IsSuccess()) return true;
            else return ServerSignUp(ID);
        }
        catch (Exception e)
        {
            print($"로그인 부분 : {e}");
            return false;
        }
    }

    // 회원가입
    public bool ServerSignUp(string ID)
    {
        try
        {
            BackendReturnObject BRO = Backend.BMember.CustomSignUp(ID, ID);

            if (BRO.IsSuccess()) return ServerLogin(ID);
            else return false;
        }
        catch (Exception e)
        {
            print($"회원가입 부분 : {e}");
            return false;
        }

    }

    // 닉네임 유무 체크
    public bool NIcknameCheck()
    {
        try
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
            else return false;
        }
        catch (Exception e)
        {
            print($"닉네임 체크 부분 : {e}");
            return false;
        }
    }

    // 닉네임 생성
    public void SetNickname(string nickname, Action<bool, string> func)
    {
        try
        {
            BackendReturnObject BRO = Backend.BMember.UpdateNickname(nickname);

            if (BRO.IsSuccess())
            {
                func(true, string.Empty);               
                return;
            }

            func(false, string.Format(BRO.GetMessage()));
        }
        catch (Exception e)
        {
            print($"닉네임 생성 부분 : {e}");
        }
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
        try
        {
            Param where = new Param(); // 검색용 where절 생성
            BackendReturnObject bro = Backend.GameSchemaInfo.Get(DATA_SERVER_TABLENAME, where, 10);

            if (bro.GetErrorCode() == "NotFoundException")
                func(CreateDataRow());
            else if (bro.IsSuccess())
            {
                UserInfoData.charIndate = bro.GetReturnValuetoJSON()[DATA_SERVER_JSONROWS][0]["inDate"]["S"].ToString();

                bro = Backend.GameSchemaInfo.Get(DATA_SERVER_TABLENAME, UserInfoData.charIndate);

                JsonData data = bro.GetReturnValuetoJSON()[DATA_SERVER_JSONROWS][0];
                UserInfoData.userCharacter = int.Parse(data[DATA_SERVER_SCHEMANAME_CHARACTER]["N"].ToString());
                func(GetUserData());
            }
        }
        catch (Exception e)
        {
            print($"최초 테이블 체크 부분 : {e}");
        }

    }

    public bool GetUserData()
    {
        try
        {
            BackendReturnObject bro = Backend.GameSchemaInfo.Get(DATA_SERVER_TABLENAME, UserInfoData.charIndate);

            if (bro.IsSuccess())
            {
                JsonData data = bro.GetReturnValuetoJSON()[DATA_SERVER_JSONROWS][0];
                UserInfoData.userClearAI = int.Parse(data[DATA_SERVER_SCHEMANAME_AI]["N"].ToString());
                return true;
            }
            else return false;
        }
        catch (Exception e)
        {           
            print($"데이터 가져오는 부분 : {e}");
            return false;
        }
    }

    // 계졍에 기본적인 데이터 삽입
    bool CreateDataRow()
    {
        try
        {
            Param data = new Param();
            data.Add(DATA_SERVER_SCHEMANAME_CHARACTER, 0);
            data.Add(DATA_SERVER_SCHEMANAME_AI, 0);

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
        catch (Exception e)
        {
            print($"데이터 삽입 부분 : {e}");
            return false;
        }

    }

    // 데이터 수정
    public void SetData(GameMode mode, Action<bool> func)
    {
        try
        {
            string inDate = string.Empty;
            Param param = new Param();

            if (mode == GameMode.Character)
                param.Add(DATA_SERVER_SCHEMANAME_CHARACTER, UserInfoData.userCharacter);
            
            inDate = UserInfoData.charIndate;

            BackendReturnObject bro = Backend.GameSchemaInfo.Update(DATA_SERVER_TABLENAME, inDate, param);

            func(bro.IsSuccess());
        }
        catch (Exception e)
        {
            print($"데이터 수정 부분 : {e}");
        }
    }

    public void SetData(GameMode mode,int value, Action<bool> func)
    {
        try
        {
            string inDate = string.Empty;
            Param param = new Param();

            if (mode == GameMode.vsAI)
                param.Add(DATA_SERVER_SCHEMANAME_AI, value);
            else if (mode == GameMode.Infinity)
                param.Add(DATA_SERVER_SCHEMANAME_INFINITY, value);

            inDate = UserInfoData.charIndate;

            BackendReturnObject bro = Backend.GameSchemaInfo.Update(DATA_SERVER_TABLENAME, inDate, param);

            func(bro.IsSuccess());
        }
        catch (Exception e)
        {
            print($"데이터 수정 부분 : {e}");
        }
    }
    #endregion

    #region 로컬 데이터 관련
    // json 파일 저장
    public void CreateJsonFile()
    {
        try
        {
            // json 데이터 제작
            string jsonData = JsonUtility.ToJson(accountData);

            // 파일로 저장
            File.WriteAllText(path, jsonData);
        }
        catch (Exception e)
        {
            print("json 파일 만드는 곳 :" + e);
        }
    }

   public bool LoadJsonFile()
    {
        try
        {
            string data = File.ReadAllText(path);

            accountData = JsonUtility.FromJson<AccountData>(data);

            return true;
        }
        catch (Exception e)
        {
            print("json 파일 로드하는 곳 :" + e);
            return false;
        }
       
    }

    public bool CheckFile()
    {
        if (File.Exists(path)) return true;
        else return false;
    }

    #endregion
}
