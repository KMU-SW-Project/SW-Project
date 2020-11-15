using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBang : DuelState
{
    DuelManger duelManager;
    float Timer;
    float enemyBangTime;
    float bangTime;

    private const string bang = "Bang!";
    public StateBang(DuelManger manager)
    {
        duelManager = manager.GetComponent<DuelManger>();
        bangTime = Random.Range(0.5f, 10.0f);
        enemyBangTime = duelManager.currentEnemyAI.fireTime + Random.Range(0, 0.3f);
    }

    public override void StateEnter()
    {

    }

    public override void StateExit()
    {
        duelManager.playerBang = false;
    }

    public override void StateUpdate()
    {
        Timer += Time.deltaTime;
        if(Timer > bangTime)
        {
            duelManager.screenSignal.text = bang;
        }
        else if(Timer > enemyBangTime)
        {
            //duelManagerEnemyFier();
            duelManager.EnemyWin();
        }
        else if(duelManager.playerBang == true)
        {
            if ( Timer < enemyBangTime)
            {
                //플레이어 승
                //결과 스테이트로 변경
            }
            else
            {
                //플레이어 패배
            }

        }
        

        
    }
}
