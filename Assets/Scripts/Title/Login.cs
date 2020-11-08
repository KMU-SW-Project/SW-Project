using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using System.Text;

public class Login : MonoBehaviour
{
    #region const
    // 서버
    const string DEBUG_SERVER_LOGING = "<Color=red>상태</Color>\n로그인 시도";
    const string DEBUG_SERVER_LOGIN_SUCCESS = "<Color=red>상태</Color>\n로그인 성공";
    const string DEBUG_SERVER_SETTING_NICKNAME = "<Color=red >상태</Color>\n닉네임 설정 중";
    const string DEBUG_SERVER_WAITING = "<Color=red>상태</Color>\n잠시만 기다려주세요.";
    const string DEBUG_SERVER_ERROR = "<Color=red>상태</Color>\n예상치 못한 에러 발생";

    // 로컬
    const string DEBUG_LOCAL_CHECKDATA = "<Color=red>상태</Color>\n데이터 파일 확인중";
    const string DEBUG_LOCAL_CREATEDATA = "<Color=red>상태</Color>\n새로운 데이터 생성";
    #endregion

    // 닉네임
    public InputField nicknameField;
    public GameObject nextScene;

    // 컴포넌트
    public Text stateText;

    // 유저 계정 관련
    string path;
    string userID;

    // 로그인 시도
    IEnumerator StartLogin()
    {
        while (true)
        {
            if (BackendServerManager.GetInstance().isConnected)
            {
                // 아이디 생성 또는 재생성
                CheckID((bool result, string id) =>
                {
                    if (result) userID = id;
                    else CreateID();
                });

                stateText.text = DEBUG_SERVER_LOGING;

                // 로그인 - 중복, 실패시 아이디 재생성 후 재시도
                if (!BackendServerManager.GetInstance().ServerLogin(userID)) continue;
                else
                {
                    stateText.text = DEBUG_SERVER_LOGIN_SUCCESS;
                    BackendServerManager.GetInstance().CreateJsonFile();
                    yield return new WaitForSeconds(0.5f);

                    if (!BackendServerManager.GetInstance().NIcknameCheck())
                    {
                        stateText.text = DEBUG_SERVER_SETTING_NICKNAME;
                        TitleManager.instance.ActiveUI(UIList.nickname);
                        break;
                    }
                    else
                    {

                        stateText.text = DEBUG_SERVER_WAITING;
                        yield return new WaitForSeconds(0.3f);
                        BackendServerManager.GetInstance().CheckTableRow((bool result) =>
                        {

                            if (result)
                            {
                                nextScene.SetActive(true);
                            }
                            else stateText.text = DEBUG_SERVER_ERROR;

                        });
                        break;
                    }
                }
            }
            else
            {
                stateText.text = DEBUG_SERVER_WAITING;
                yield return new WaitForSeconds(0.3f);
                nextScene.SetActive(true);
                break;
            }
        }
    }

    // 로그인 버튼 누르면
    public void StartLoginBtn()
    {
        if (TitleManager.instance.uiOn) return;

        StartCoroutine(StartLogin());
    }

    // 닉네임 설정
    public void SetNickname()
    {
        TitleManager.instance.ButtonOff(UIList.nickname, false);
        BackendServerManager.GetInstance().SetNickname(nicknameField.text, (bool result, string error) =>
        {
            if (!result)
            {
                TitleManager.instance.ButtonOff(UIList.nickname, true);
                TitleManager.instance.ActiveUI(UIList.error);
                TitleManager.instance.SerErrorMessage(error);
                return;
            }

            BackendServerManager.GetInstance().UserInfoData.userNickname = nicknameField.text;
            nextScene.SetActive(true);
        });
    }

    // 로컬에 데이터가 있는지 확인
    private void CheckID(Action <bool, string> returnData)
    {
        stateText.text = DEBUG_LOCAL_CHECKDATA;

        //string userAccountInfo = "";

        //StreamReader textRead = File.OpenText(path);
        //userAccountInfo = textRead.ReadLine();
        //textRead.Dispose();
        //textRead.Close();
        string id = BackendServerManager.GetInstance().accountData.userID;
        if (id == "") returnData(false, "");
        else returnData(true, id);
    }

    // 랜덤 아이디생성후 파일 저장
    private string CreateID()
    {
        print("id 제작중");
        stateText.text = DEBUG_LOCAL_CREATEDATA;

        string newID;

        newID = GetRandomInfo();

        BackendServerManager.GetInstance().accountData.userID = newID;
        userID = newID;

        return newID;
    }

    /// 유저의 아이디를 랜덤한 값으로 생성함
    private string GetRandomInfo()
    {
        System.Random rand = new System.Random();

        string input = "abcdefghijklmnopqrstuvwxyz0123456789";
        var chars = Enumerable.Range(0, 15).Select(x => input[rand.Next(0, input.Length)]);

        return new string(chars.ToArray());
    }
}
