using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class Login : MonoBehaviour
{
    // 닉네임
    public InputField nicknameField;
    public GameObject test;

    // 주 손 선택
    string handType;

    // 컴포넌트
    public Text stateText;

    // 유저 계정 관련
    string path;
    string userID;

    private void Awake() => HandCheck();

    // 주 손을 선택했는지 판단
    void HandCheck()
    {
        path = Application.persistentDataPath + "/Handdata.txt";
        print(path);

        if (File.Exists(path))
        {
            handType = File.ReadAllText(path);
            GameManager.GetInstance().handType = handType;

            if (handType == "left") HandType.instance.SetHandType(Valve.VR.SteamVR_Input_Sources.LeftHand);
            else if (handType == "right") HandType.instance.SetHandType(Valve.VR.SteamVR_Input_Sources.RightHand);
            else TitleManager.instance.ActiveOnUI("SelectHand");

            Init();
        }
        else
        {
            TitleManager.instance.ActiveOnUI("SelectHand");
        }
    }

    // 초기화
    void Init()
    {
        path = Application.persistentDataPath + "/data.txt";
                

        if (!File.Exists(path)) File.Create(path);

        stateText.text = "<Color=red>상태</Color>\n대기중";
        //  userID = CreateUser(true);
    }


    /// <summary>
    /// 로그인 시도
    /// </summary>
    IEnumerator StartLogin()
    {
        while (true)
        {
            if (BackendServerManager.GetInstance().isConnected)
            {
                if (CheckID() == null)
                {
                    userID = CreateID();
                }
                else userID = CheckID();

                stateText.text = "<Color=red>상태</Color>\n로그인 시도";
                if (!BackendServerManager.GetInstance().ServerLogin(userID))
                {
                    userID = CreateID();

                    continue;
                }
                else
                {
                    stateText.text = "<Color=red>상태</Color>\n로그인 성공";
                    yield return new WaitForSeconds(0.5f);
                    if (!BackendServerManager.GetInstance().NIcknameCheck())
                    {
                        stateText.text = "<Color=red>상태</Color>\n닉네임 설정 중";
                        TitleManager.instance.ActiveOnUI("Nickname");
                        break;
                    }
                    else
                    {
                        stateText.text = "<Color=red>상태</Color>\n잠시만 기다려주세요.";
                        yield return new WaitForSeconds(0.5f);
                        BackendServerManager.GetInstance().CheckTableRow((bool result) =>
                        {
                            if(result) SceneManager.LoadSceneAsync("MainMenu");
                            else stateText.text = "<Color=red>상태</Color>\n예상치 못한 에러 발생";

                        });
                        break;
                    }
                }
            }
            else
            {
                stateText.text = "<Color=red>상태</Color>\n[OFFLINE]\n잠시만 기다려주세요.";
                yield return new WaitForSeconds(0.3f);
                test.SetActive(true);
               // SceneManager.LoadSceneAsync("MainMenu");
                break;
            }

        }
    }

    public void StartLoginBtn()
    {
        if (TitleManager.instance.uiOn) return;

        StartCoroutine(StartLogin());
    }

    public void SetNickname()
    {
        TitleManager.instance.ButtonOff("Nickname", false);

        BackendServerManager.GetInstance().SetNickname(nicknameField.text, (bool result, string error) =>
        {
            if (!result)
            {
                TitleManager.instance.ButtonOff("Nickname", true);
                TitleManager.instance.ActiveOnUI("Error");
                TitleManager.instance.SerErrorMessage(error);
                return;
            }

            SceneManager.LoadSceneAsync("MainMenu");
        });
    }

    private string CheckID()
    {
        stateText.text = "<Color=red>상태</Color>\n데이터 파일 확인중";

        string userAccountInfo = "";

        StreamReader textRead = File.OpenText(path);
        userAccountInfo = textRead.ReadLine();
        textRead.Dispose();

        return userAccountInfo;
    }

    private string CreateID()
    {
        stateText.text = "<Color=red>상태</Color>\n새로운 데이터 생성";

        string newID;

        newID = GetRandomInfo();
        File.WriteAllText(path, newID);

        return newID;
    }

    /// <summary>
    /// 유저의 아이디를 랜덤한 값으로 생성함
    /// </summary>
    /// <returns></returns>
    private string GetRandomInfo()
    {
        System.Random rand = new System.Random();

        string input = "abcdefghijklmnopqrstuvwxyz0123456789";
        var chars = Enumerable.Range(0, 15).Select(x => input[rand.Next(0, input.Length)]);

        return new string(chars.ToArray());
    }
}
