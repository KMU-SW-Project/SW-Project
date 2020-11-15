using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateManager : MonoBehaviour
{
    public DuelState currentState { get; private set; }
    private static StateManager instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static StateManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void Update()
    {
        StateUpdate();
    }

    public void SetState(DuelState nextDuelState)
    {
        currentState = nextDuelState;
    }
    
    public void ChageDuelState( DuelState nextDuelState)
    {
        currentState.StateExit();

        currentState = nextDuelState;

        currentState.StateEnter();
    }

    public void StateUpdate()
    {
        if(currentState == null)
        {
            Debug.LogError("CurrenState null");
            return;
        }
        currentState.StateUpdate();
    }
}
