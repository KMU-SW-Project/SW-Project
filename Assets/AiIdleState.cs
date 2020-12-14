using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiIdleState : StateMachineBehaviour
{
    public DuelManger duelManger;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        duelManger = GameObject.Find("Function").GetComponent<DuelManger>();
        animator.SetInteger("state", 0);

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameManager.GetInstance().modeData.currentPlayMode != GameMode.Bounty) return;

        if(duelManger == null)
        {
            duelManger = GameObject.Find("Function").GetComponent<DuelManger>();
        }

        if(duelManger.enemyBang)
        {
            animator.SetInteger("state", 1);
        }
        else if (duelManger.enemyDeath)
        {
            animator.SetInteger("state", 2);
            animator.SetTrigger("death");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

}
