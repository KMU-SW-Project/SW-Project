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

        // 업데이트에서 true 되면 죽는 애니메이션 실행하는?게 어떨까
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
        currentState = duelstate.End;
        screenSignal.text = null;
        if(playerScore >= 3)
        {
            if(currentGameMode == gamemode.Bounty)
            {
                BackendServerManager.GetInstance().SetData(GameMode.Bounty, GameManager.GetInstance().modeData.currentPlayAiData.enemyID, (bool result) =>
                {
                    if (result)
                    {
                        GameManager.GetInstance().modeData.currentPlayAiData.bountyMoney = -1;
                    }
                    else print("저장 실패");
                });
            }
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


enum duelstate
{
    Start,
    Ready,
    Stady,
    Bang,
    End

}