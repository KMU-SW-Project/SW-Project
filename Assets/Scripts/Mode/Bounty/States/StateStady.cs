using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateStady : DuelState
{
    DuelManger duelManager;
    public const string stady = "Stady";
    float stadyTimer;

    public StateStady(DuelManger manager)
    {
        duelManager = manager.GetComponent<DuelManger>();
    }
    public override void StateEnter()
    {
        duelManager.screenSignal.text = stady;
    }

    public override void StateExit()
    {
        duelManager.screenSignal.text = null;
    }

    public override void StateUpdate()
    {
        stadyTimer += Time.deltaTime;
        if(stadyTimer > 3.0f)
        {
            StateManager.Instance.ChageDuelState(duelManager.stateBang);
        }
        // WandController 에서 readyposition false될 경우 EnemyWin.
    }
}
