using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyModeLoad : MonoBehaviour
{
    public GameObject player;
    public GameObject nextScene;
    public Transform spawnPos;
    public EnemyAI[] ai;
    int index;

    private void Awake()
    {
        if (GameManager.GetInstance().playMode == GameMode.VRTest) player.SetActive(false);

        GameManager.GetInstance().SetPlayerCameraPosition(player.transform);
    }

    private void Start()
    {
       index = GameManager.GetInstance().modeData.currentPlayAiIndex;
      Instantiate(GameManager.GetInstance().modeData.enemyData[index].model, spawnPos);
    }

    public void GoMainmenu() => nextScene.SetActive(true);

    public void AIClear()
    {
        BackendServerManager.GetInstance().SetData(GameMode.Bounty, GameManager.GetInstance().modeData.enemyData[index].enemyID, (bool result) =>
        {
            if (result)
            {
                GameManager.GetInstance().modeData.enemyData[index].bountyMoney = -1;
                GoMainmenu();
            }
            else print("저장 실패");
        });
    }
}
