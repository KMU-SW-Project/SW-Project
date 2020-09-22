using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public Text mainText;
    Animator textAm;

    [Header("Test")]
    public bool isTest = false;
    public InputField testId;

    public GameObject errorUI;
    Text errorText;
    public GameObject nicknameUI;
    InputField nicknameField;
   

    string path;
    string userID;
    bool canStart = false;

    public void TestError()
    {
        errorUI.SetActive(true);
    }

    private void Awake()
    {
        path = Application.persistentDataPath + "/data.txt";
        print(path);

        textAm = mainText.GetComponent<Animator>();
        nicknameField = nicknameUI.transform.GetChild(2).GetComponent<InputField>();
        errorText = errorUI.transform.GetChild(2).GetComponent<Text>();
    }

    private void Start()
    {
        mainText.text = "서버 연결 중...";

        userID = CreateUser();

        if (!isTest)
        LoginPreparations();
    }

    public void TestLogin()
    {
        if(BackendServerManager.GetInstance().ServerLogin(testId.text))
        {
            print("== 로그인 성공 ==");

            if (!BackendServerManager.GetInstance().NIcknameCheck())
            {
                mainText.text = "닉네임 설정중...";
                nicknameUI.SetActive(true);
            }
            else
            {
                mainText.text = "아무 버튼이나 눌러주세요!";
                print($"닉네임 : {BackendServerManager.GetInstance().UserInfoData.userNickname}");
                textAm.SetTrigger("isTouch");
                canStart = true;
            }
        }
    }

    private void Update()
    {
        if (canStart)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadSceneAsync("MatchLobby");
            }
        }
    }

    /// <summary>
    /// 로그인 시도
    /// </summary>
    void LoginPreparations()
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
                mainText.text = "닉네임 설정중...";
                nicknameUI.SetActive(true);
            }
            else
            {
                mainText.text = "아무 버튼이나 눌러주세요!";
                print($"닉네임 : {BackendServerManager.GetInstance().UserInfoData.userNickname}");
                textAm.SetTrigger("isTouch");
                canStart = true;
            }
       
    }

    public void SetNickname()
    {
        BackendServerManager.GetInstance().SetNickname(nicknameField.text, (bool result, string error) =>
        {
            if (!result)
            {
                errorUI.SetActive(true);
                errorText.text = error;
                return;
            }

                print("== 닉네임 설정완료 ==");
                SceneManager.LoadSceneAsync("MatchLobby");
           
        });
    }

    public void ActiveOff(string type)
    {
        switch (type)
        {
            case "error":
                errorUI.SetActive(false);
                break;
            default:
                break;
        }
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
            else if(!File.Exists(path) || (File.Exists(path) && reset))
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
