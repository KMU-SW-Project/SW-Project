using System.Collections;
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
    public bool start = false;
    public bool ready;
    public gamemode currentGameMode;

    Animator enemyanimator;
    duelstate currentState;
    int Round;

    private void Awake()
    {
        currentState = duelstate.Start;
        screenSignal.text = null;
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
            if (playerBang)
            {
                Enemywin();
                playerBang = false;
            }
        }
        else if(currentState == duelstate.Bang)
        {
            if (playerBang)
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
        readyButton.SetActive(false);
    }

    IEnumerator DuelReady()
    {
        //Ready신호음 출력
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
        Debug.Log("Bang");
        screenSignal.text = "Bang!";
        currentState = duelstate.Bang;
        float enemyFiretime;
        enemyFiretime = currentEnemyAI.GetFireTime();
        Debug.Log("enemyFireTime  :" + enemyFiretime);
        //플레이어가 쐈으면 playerwin 하고 return
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
        currentState = duelstate.End;
        screenSignal.text = null;
        if(playerScore >= 3)
        {
            Debug.Log("Playerwin");
            titleButton.SetActive(true);
            resultText.gameObject.SetActive(true);
            resultText.text = "You Win !!!!";

        }
        else if(enemyScore >= 3)
        {
            Debug.Log("EnemyWin");
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
        StopAllCoroutines();
        playerScore++;
        //적 애니메이션?
        DuelEnd();
    }

    private void Enemywin()
    {
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


enum duelstate
{
    Start,
    Ready,
    Stady,
    Bang,
    End

}