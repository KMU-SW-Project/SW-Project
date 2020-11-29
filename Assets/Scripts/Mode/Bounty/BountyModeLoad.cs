using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyModeLoad : MonoBehaviour
{
    public GameObject player;
    public GameObject nextScene;
    public Transform spawnPos;
    public EnemyAI[] ai;

    private void Awake()
    {
        if (GameManager.GetInstance().playMode == GameMode.VRTest) player.SetActive(false);

        GameManager.GetInstance().SetPlayerCameraPosition(player.transform);
    }

    private void Start()
    {
        Instantiate(GameManager.GetInstance().modeData.currentPlayAiData.model, spawnPos);
    }

    public void GoMainmenu() => nextScene.SetActive(true);

    public void AIClear()
    {
        BackendServerManager.GetInstance().SetData(GameMode.Bounty, GameManager.GetInstance().modeData.currentPlayAiData.enemyID, (bool result) =>
        {
            if (result)
            {
                GameManager.GetInstance().modeData.currentPlayAiData.bountyMoney = -1;
                GoMainmenu();
            }
            else print("저장 실패");
        });
    }
}
