﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinityTest : MonoBehaviour
{
    public GameObject loadScene;
    public GameObject player;
    public Transform spawnPos;

    private int score;
    private GameObject[] spawnList;

    private void Awake()
    {
        if (GameManager.GetInstance().playMode == GameMode.VRTest) player.SetActive(false);

        GameManager.GetInstance().SetPlayerCameraPosition(player.transform);
    }

    private void Start()
    {
        score = 0;
        GameManager.GetInstance().SetBGM(GameMode.Infinity);
        GameManager.GetInstance().modeData.currentPlayMode = GameMode.Infinity;

        var length = GameManager.GetInstance().modeData.enemyData.Count;

        spawnList = new GameObject[10];

        for (int i = 0; i < spawnList.Length; i++)
        {
            var index = Random.Range(0, length);

            var obj = Instantiate(GameManager.GetInstance().modeData.enemyData[index].model, spawnPos.GetChild(0).transform).gameObject;
          
            obj.SetActive(false);
            spawnList[i] = obj;
        }

        spawnList[0].SetActive(true);
    }

    /// <summary>
    /// 다음 소환
    /// </summary>
    public void ChangeEnemy()
    {
        for (int i = 0; i < spawnList.Length; i++)
        {
            if (spawnList[i].activeSelf)
            {
                spawnList[i].SetActive(false);
                spawnList[i + 1].SetActive(true);
                break;
            }
        }

        score++;
    }

    public void GoMainMenu()
    {
        loadScene.SetActive(true);
    }

    /// <summary>
    /// 랭킹 등록
    /// </summary>
    /// <param name="winScore"> 연승 수 </param>
    public void JoinRanking(int winScore)
    {
        if (!BackendServerManager.GetInstance().isConnected)
        {
            print("오프라인 모드입니다!");
            return;
        }

        if (winScore < GameManager.GetInstance().userData.userInfinityScore)
        {
            print("랭킹에 등록된 점수보다 낮기때문에 등록 불가");
            GoMainMenu();
            return;
        }

        BackendServerManager.GetInstance().SetData(GameMode.Infinity, winScore, (bool result) =>
          {
              if (!result)
              {
                  print("무한 모드 점수 등록 실패");
                  return;
              }
          });

        BackendServerManager.GetInstance().SetRankingScore(winScore, (bool result) =>
        {
            if (result)
            {
                print("등록 성공");
                GameManager.GetInstance().userData.userInfinityScore = winScore;
                GoMainMenu();
            }
            else print("등록 실패");
        });

    }
}
