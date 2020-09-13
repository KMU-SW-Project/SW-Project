using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public  class Login : MonoBehaviour
{
    public Text mainText;
    Animator textAm;

    string path;
    string userID;
    bool canStart = false;

    private void Awake()
    {
        path = Application.persistentDataPath + "/data.txt";

        textAm = mainText.GetComponent<Animator>();

    }

    private void Start()
    {
        mainText.text = "서버 연결 중...";
        LoginPreparations();
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
        userID = CreateUser();

        if (!BackendServerManager.GetInstance().ServerLogin(userID))
        {
            userID = CreateUser(true);
            if (!BackendServerManager.GetInstance().ServerSignUp(userID))
            {
                print("로그인 실패, 다시 시도해 주세요!");
                return;
            }
        }

        mainText.text = "아무 버튼이나 눌러주세요!";
        textAm.SetTrigger("isTouch");
        canStart = true;
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
