using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Login : MonoBehaviour
{
    // 에러 관련
    public GameObject errorUI;
    Text errorText;

    // 닉네임
    public GameObject nicknameUI;
    public GameObject keyboard;
    InputField nicknameField;

    // 주 손 선택
    public GameObject selectHandUI;
    string handType;

    // 컴포넌트
    HandType _HT;

    // 유저 계정 관련
    string path;
    string userID;

    private void Awake()
    {
        HandCheck();
    }

    private void Start()
    {
        userID = CreateUser();
    }


    // 초기화
    void Init()
    {
        path = Application.persistentDataPath + "/data.txt";
        print(path);

        nicknameField = nicknameUI.transform.GetChild(2).GetComponent<InputField>();
        errorText = errorUI.transform.GetChild(2).GetComponent<Text>();
    }

    // 주 손을 선택했는지 판단
    void HandCheck()
    {
        _HT = GetComponent<HandType>();
        path = Application.persistentDataPath + "/Handdata.txt";

        if (File.Exists(path))
        {
            handType = File.ReadAllText(path);
            if (handType == "left") _HT.SetHandType(Valve.VR.SteamVR_Input_Sources.LeftHand);
            else _HT.SetHandType(Valve.VR.SteamVR_Input_Sources.RightHand);

            Init();
        }
        else selectHandUI.SetActive(true);
    }

    /// <summary>
    /// 로그인 시도
    /// </summary>
    public void LoginPreparations()
    {
        if (!BackendServerManager.GetInstance().ServerLogin(userID))
        {
            userID = CreateUser(true);
            if (!BackendServerManager.GetInstance().ServerSignUp(userID))
            {
                print("로그인 실패, 다시 시도해 주세요!");
                return;
            }
        }

        print("== 로그인 성공 ==");

        if (!BackendServerManager.GetInstance().NIcknameCheck())
        {
            nicknameUI.SetActive(true);
            keyboard.SetActive(true);
        }
        else
        {
            print($"닉네임 : {BackendServerManager.GetInstance().UserInfoData.userNickname}");
            SceneManager.LoadSceneAsync("MainMenu");
        }

    }

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

            print("== 닉네임 설정완료 ==");
            SceneManager.LoadSceneAsync("MainMenu");

        });
    }

    /// <summary>
    /// 최초 실행시 회원가입을 위한 유저의 아이디 생성 후 리턴
    /// </summary>
    /// <param name="reset"> 중복 아이디가 생성 되었을 경우 새로 생성</param>
    /// <returns></returns>
    private string CreateUser(bool reset = false)
    {
        string userAccountInfo = "";

        try
        {
            if (File.Exists(path) && !reset)
            {
                StreamReader textRead = File.OpenText(path);
                userAccountInfo = textRead.ReadLine();
                textRead.Dispose();
            }
            else if (!File.Exists(path) || (File.Exists(path) && reset))
            {
                userAccountInfo = GetRandomInfo();
                if (File.Exists(path)) File.Delete(path);

                StreamWriter textWrite = File.CreateText(path);
                textWrite.WriteLine(userAccountInfo);
                textWrite.Dispose();
            }

            return userAccountInfo;
        }
        catch (System.Exception e)
        {
            print(e.Message);
        }

        return null;
    }

    public bool CheckUserInfoFile()
    {
        if (File.Exists(path)) return true;
        else return false;
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
