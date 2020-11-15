using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateReady : DuelState
{
    DuelManger duelManager;
    const string ready = "Ready";
    float readyTimer;
    public StateReady(DuelManger manager)
    {
        duelManager = manager.GetComponent<DuelManger>();
    }

    public override void StateEnter()
    {
        duelManager.screenSignal.text = ready;
    }

    public override void StateExit()
    {
        duelManager.screenSignal.text = null;
        readyTimer = 0;
    }

    public override void StateUpdate()
    {
        readyTimer += Time.deltaTime;
        if( readyTimer > 3.0f)
        {
            StateManager.Instance.ChageDuelState(duelManager.stateStady);
        }
        // wnadController에서 컨트롤러 상태 체크 후 
        // 컨트롤러 readyposition true일 경우 Stady로 전환
    }
}
