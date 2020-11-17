using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class AIMode
{
    public static int[] Money = { 10, 20, 30, 40, 50 };
    public static int currentAI;
}

public class ModeManager : MonoBehaviour
{
    public GameObject mainUI;

    [Header("vsAI")]
    public GameObject originCharacter;
    public GameObject aiCharacter;
    public EnemyAI[] aiData;
    public GameObject aiUI;
    public Text aiMoneyText;

    GameObject[] aiCharacterList;   

    private void Awake()
    {
        AIMode.currentAI = 0;
        aiCharacterList = new GameObject[aiCharacter.transform.childCount];

        for (int i = 0; i < aiCharacter.transform.childCount; i++)
            aiCharacterList[i] = aiCharacter.transform.GetChild(i).gameObject;
    }

    // 무한, 현상금, 사격장
    // ui 활성화
    public void ActiveUI(int type)
    {
        if(type == 0)
        {

        }
        else if(type == 1)
        {
            mainUI.SetActive(false);
            originCharacter.SetActive(false);
            aiUI.SetActive(true);
            aiCharacter.SetActive(true);
        }
        else if(type == 3)
        {

        }      
    }

    public void PassiveUI(int type)
    {
        if (type == 0)
        {

        }
        else if (type == 1)
        {
            mainUI.SetActive(true);
            originCharacter.SetActive(true);
            aiUI.SetActive(false);
            aiCharacter.SetActive(false);
        }
        else if (type == 3)
        {

        }
    }

    #region 현상금 모드
    // 현상금 모드에서 AI 변경
    // 왼쪽 0 오른쪽 1
    public void ChangeAI(int type)
    {
        if(type == 0)
        {
            if (AIMode.currentAI == 0)
                AIMode.currentAI = aiCharacterList.Length - 1;
            else AIMode.currentAI--;            
        }
        else if(type == 1)
        {
            if (AIMode.currentAI == aiCharacterList.Length - 1)
                AIMode.currentAI = 0;
            else AIMode.currentAI++;
        }
        else
        {
            AIMode.currentAI = BackendServerManager.GetInstance().UserInfoData.userClearAI;

            for (int i = 0; i < AIMode.currentAI; i++)
                AIMode.Money[i] = -1;            
        }

        for (int i = 0; i < aiCharacterList.Length; i++)
            aiCharacterList[i].SetActive(false);

        if (AIMode.Money[AIMode.currentAI] != -1) aiMoneyText.text = $"$ {aiData[AIMode.currentAI]}";
        else aiMoneyText.text = "CLEAR";

        aiCharacterList[AIMode.currentAI].SetActive(true);
    }

   public void StartAIMode()
    {
        GameManager.GetInstance().currentAIStage = AIMode.currentAI;
        SceneManager.LoadSceneAsync("vsAI");
    }
    #endregion


    // 로드 한 데이터를 기반으로 모든 모드를 초기화
    public void ReSetMode()
    {
        ChangeAI(3);
    }
}
