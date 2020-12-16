﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuelManger : MonoBehaviour
{
    public EnemyAI currentEnemyAI;  
    public GameObject readyButton;
    public GameObject titleButton;
    public Text playerName;
    public Text EnemyName;
    public Text playerScoreText; 
    public Text EnemyScoreText;
    public Text screenSignal;
    public Text resultText;

    public int playerScore { get; private set; }
    public int enemyScore { get; private set; }
    public bool playerBang = false;
    public bool enemyBang = false;
    public bool enemyDeath = false;
    public bool start = false;
    public bool ready;
    public gamemode currentGameMode;
    public duelstate currentState;

    int Round;
    private void Awake()
    {
        currentState = duelstate.Start;
        screenSignal.text = null;

        GameManager.GetInstance().SetBGM(GameMode.Bounty);
        GameManager.GetInstance().modeData.currentPlayMode = GameMode.Bounty;
    }

    private void Update()
    {
       // playerName.text = GameManager.GetInstance().userData.userNickname;
        EnemyName.text = currentEnemyAI.enemyName;
        playerScoreText.text = playerScore.ToString();
        EnemyScoreText.text = enemyScore.ToString();

        if( start == false && currentState == duelstate.Start )
        {
            DuelStart();
        }

        if (currentState == duelstate.Ready || currentState == duelstate.Stady)
        {
            //플레이어 자세 쳌
            if (playerBang || GameManager.GetInstance().hitEnemy || GameManager.GetInstance().isShot)
            {
                Enemywin();
                playerBang = false;
            }
        }
        else if(currentState == duelstate.Bang)
        {
            if (playerBang || GameManager.GetInstance().hitEnemy)
            {
                Playerwin();
                playerBang = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerBang();
        }
    }

    private void DuelStart()
    {
        start = true;
        if(currentGameMode == gamemode.Bounty)
        {
            Round = 5;
        }
        else if( currentGameMode == gamemode.Infinity)
        {
            Round = 1;
        }

        //적 애니메이션 전환

    }

    public void DuelReadyCall()
    {
        StartCoroutine(DuelReady());

        // 총 쏘는거 초기화
        GameManager.GetInstance().isShot = false;
        GameManager.GetInstance().hitEnemy = false;

        readyButton.SetActive(false);
    }

    IEnumerator DuelReady()
    {
        //Ready신호음 출력
        SFXManager.Instance.PlaySFX(vfx.Ready);
        //signal 출력
        currentState = duelstate.Ready;
        screenSignal.text = "Ready";
        Debug.Log("Ready");
        //자세 체크
        yield return new WaitForSeconds(2f);
        StartCoroutine(DuelStady());
    }

    IEnumerator DuelStady()
    {
        //Stady 신호음
        SFXManager.Instance.PlaySFX(vfx.Stady);
        //signa
        currentState = duelstate.Stady;
        screenSignal.text = "Stady";
        float bangTime;
        bangTime = Random.Range(1f, 10.0f);
        Debug.Log("Stady");
        Debug.Log("BangTime " + bangTime);
        yield return new WaitForSeconds(bangTime);
        StartCoroutine(DuelBang());
    }

    IEnumerator DuelBang()
    {
        SFXManager.Instance.PlaySFX(vfx.Bang);
        enemyBang = true;
        currentState = duelstate.Bang;
        screenSignal.text = "Bang!";
        Debug.Log("Bang");
        
        float enemyFiretime;
        enemyFiretime = currentEnemyAI.GetFireTime();
        Debug.Log("enemyFireTime  :" + enemyFiretime);

        yield return new WaitForSeconds(enemyFiretime);
       
        Debug.Log("EnemyBang");
        //적 발사 anime
        //스크린 빨갛게?
        Enemywin();
    }

    void PlayerBang()
    {
        playerBang = true;
    }
   
    private void DuelEnd()
    {
        var index = GameManager.GetInstance().modeData.currentPlayAiIndex;

        currentState = duelstate.End;
        screenSignal.text = null;
        enemyBang = false;
        if(playerScore >= 3)
        {
            if(currentGameMode == gamemode.Bounty)
            {
                BackendServerManager.GetInstance().SetData(GameMode.Bounty, GameManager.GetInstance().modeData.enemyData[index].enemyID, (bool result) =>
                {
                    if (result)
                    {
                        GameManager.GetInstance().modeData.enemyData[index].bountyMoney = -1;
                    }
                    else print("저장 실패");
                });
            }
            enemyDeath = true;
            Debug.Log("Playerwin");
            SFXManager.Instance.PlaySFX(vfx.Victory);
            titleButton.SetActive(true);
            resultText.gameObject.SetActive(true);
            resultText.text = "You Win !!!!";

        }
        else if(enemyScore >= 3)
        {
            Debug.Log("EnemyWin");
            SFXManager.Instance.PlaySFX(vfx.Lose);
            titleButton.SetActive(true);
            resultText.gameObject.SetActive(true);
            resultText.text = "You Lose !!!!";
        }
        else
        {
            readyButton.SetActive(true);
        }
        
    }

    private void Playerwin()
    {
        SFXManager.Instance.PlaySFX(vfx.Victory);
        StopAllCoroutines();
        playerScore++;
        //적 애니메이션?
        DuelEnd();
    }

    private void Enemywin()
    {
        SFXManager.Instance.PlaySFX(vfx.Lose);
        StopAllCoroutines();
        enemyScore++;
        //전광판 표시?
        //적 애니메이션?
        DuelEnd();
    }

    

}

public enum gamemode
{
    Bounty,
    Infinity
}


public enum duelstate
{
    Start,
    Ready,
    Stady,
    Bang,
    End

}