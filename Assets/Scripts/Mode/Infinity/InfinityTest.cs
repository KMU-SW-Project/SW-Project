using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinityTest : MonoBehaviour
{
    public GameObject loadScene;
    public GameObject player;
    public int score;

    private void Awake()
    {
        if (GameManager.GetInstance().playMode == GameMode.VRTest) player.SetActive(false);

        GameManager.GetInstance().SetPlayerCameraPosition(player.transform);
    }

    private void Start()
    {
        GameManager.GetInstance().SetBGM(GameMode.Infinity);
        GameManager.GetInstance().modeData.currentPlayMode = GameMode.Infinity;
    }

    public void GoMainMenu()
    {
        loadScene.SetActive(true);
    }

    public void JoinRanking()
    {
        if (!BackendServerManager.GetInstance().isConnected)
        {
            print("오프라인 모드입니다!");
            return;
        }

        if (score < GameManager.GetInstance().userData.userInfinityScore)
        {
            print("랭킹에 등록된 점수보다 낮기때문에 등록 불가");
            GoMainMenu();
            return;
        }

        BackendServerManager.GetInstance().SetData(GameMode.Infinity, score, (bool result) =>
         {
             if (!result)
             {
                 print("무한 모드 점수 등록 실패");
                 return;
             }
         });

        BackendServerManager.GetInstance().SetRankingScore(score, (bool result) =>
        {
            if (result)
            {
                print("등록 성공");
                GameManager.GetInstance().userData.userInfinityScore = score;
                GoMainMenu();
            }
            else print("등록 실패");
        });

    }
}
