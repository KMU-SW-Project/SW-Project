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
    // 에러 관련
    public GameObject errorUI;
    Text errorText;

    // 닉네임
    public GameObject nicknameUI;
    public GameObject keyboard;

    [SerializeField]
    InputField nicknameField;

    // 주 손 선택
    public GameObject selectHandUI;
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

        if (File.Exists(path))
        {
            handType = File.ReadAllText(path);
            GameManager.GetInstance().handType = handType;

            if (handType == "left") HandType.instance.SetHandType(Valve.VR.SteamVR_Input_Sources.LeftHand);
            else if (handType == "right") HandType.instance.SetHandType(Valve.VR.SteamVR_Input_Sources.RightHand);
            else selectHandUI.SetActive(true);

            Init();
        }
        else
        {
            selectHandUI.SetActive(true);
        }
    }

    // 초기화
    void Init()
    {
        path = Application.persistentDataPath + "/data.txt";


        nicknameField = nicknameUI.transform.GetChild(2).GetComponent<InputField>();
        errorText = errorUI.transform.GetChild(2).GetComponent<Text>();

        if (!File.Exists(path)) File.Create(path);

        stateText.text = "<Color=red>상태</Color>\n로그인 대기중";
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
                        nicknameUI.SetActive(true);
                        keyboard.SetActive(true);
                        break;
                    }
                    else
                    {
                        stateText.text = "<Color=red>상태</Color>\n잠시후 메인메뉴로 이동합니다.";
                        yield return new WaitForSeconds(0.3f);
                        SceneManager.LoadSceneAsync("MainMenu");
                        break;
                    }
                }
            }
            else
            {
                stateText.text = "<Color=red>상태</Color>\n[OFFLINE]\n잠시후 메인메뉴로 이동합니다.";
                yield return new WaitForSeconds(0.3f);
                SceneManager.LoadSceneAsync("MainMenu");
                break;
            }

        }
    }

    public void StartLoginBtn() => StartCoroutine(StartLogin());

    public void SetNickname()
    {
        Button btn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        btn.interactable = false;

        BackendServerManager.GetInstance().SetNickname(nicknameField.text, (bool result, string error) =>
        {
            if (!result)
            {
                btn.interactable = true;
                errorUI.SetActive(true);
                errorText.text = error;
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
