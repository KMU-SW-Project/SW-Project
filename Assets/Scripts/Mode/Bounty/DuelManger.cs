using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuelManger : MonoBehaviour
{
    public EnemyAI currentEnemyAI;
    public Text screenSignal;
    public int PlayerScore { get; private set; }
    public int EnemyScore { get; private set; }
    public const string Stady = "Stady";
    public const string Bang = "Bang";
    public DuelState stateReady;
    public DuelState stateStady;
    public DuelState stateBang;
    public bool playerBang;
    
    private void Awake()
    {
        stateReady = new StateReady(this);
        stateStady = new StateStady(this);
        stateBang = new StateBang(this);
    }

    public void GameInit()
    {
        PlayerScore = 0;
        EnemyScore = 0;
    }

    public void PlayerWin()
    {
        PlayerScore++;
    }
    public void EnemyWin()
    {
        EnemyScore++;
    }

}
