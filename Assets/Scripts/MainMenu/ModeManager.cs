using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class AIMoney
{
    public static int[] Money = { 10, 20, 30, 40, 50 };
}

public class ModeManager : MonoBehaviour
{
    public GameObject mainUI;

    [Header("vsAI")]
    public GameObject originCharacter;
    public GameObject aiCharacter;
    public GameObject aiUI;
    public Text aiMoneyText;
    GameObject[] aiCharacterList;
    int currentAI;

    private void Awake()
    {
        currentAI = 0;
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
            if (currentAI == 0)
                currentAI = aiCharacterList.Length - 1;
            else currentAI--;            
        }
        else if(type == 1)
        {
            if (currentAI == aiCharacterList.Length - 1)
                currentAI = 0;
            else currentAI++;
        }
        else
        {
            currentAI = BackendServerManager.GetInstance().UserInfoData.userClearAI;

            for (int i = 0; i < currentAI; i++)
                AIMoney.Money[i] = -1;
            
        }

        for (int i = 0; i < aiCharacterList.Length; i++)
            aiCharacterList[i].SetActive(false);

        if (AIMoney.Money[currentAI] != -1) aiMoneyText.text = $"$ {AIMoney.Money[currentAI]}";
        else aiMoneyText.text = "CLEAR";

        aiCharacterList[currentAI].SetActive(true);
    }

   public void StartAIMode()
    {
        GameManager.GetInstance().currentAIStage = currentAI;
        SceneManager.LoadSceneAsync("vsAI");
    }
    #endregion


    // 로드 한 데이터를 기반으로 모든 모드를 초기화
    public void ReSetMode()
    {
        ChangeAI(3);
    }
}
