using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTest : MonoBehaviour
{
    public GameObject nextScene;

    public void AIClear()
    {
        BackendServerManager.GetInstance().SetData(GameMode.Bounty, GameManager.GetInstance().currentAIStage+1, (bool result) =>
        {
            if (result)
            {
               // AIMode.Money[GameManager.GetInstance().currentAIStage] = -1;
                GameManager.GetInstance().userData.userClearAI = GameManager.GetInstance().currentAIStage + 1;
                nextScene.SetActive(true);
            }
            else print("저장 실패");
        });
    }
}
