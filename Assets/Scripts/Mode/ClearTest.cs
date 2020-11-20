using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTest : MonoBehaviour
{
    public GameObject nextScene;

    public void AIClear()
    {
        BackendServerManager.GetInstance().SetData(GameMode.Bounty, GameManager.GetInstance().modeData.currentPlayAiData.enemyID, (bool result) =>
        {
            if (result)
            {
                GameManager.GetInstance().modeData.currentPlayAiData.bountyMoney = -1;
                nextScene.SetActive(true);
            }
            else print("저장 실패");
        });
    }
}
