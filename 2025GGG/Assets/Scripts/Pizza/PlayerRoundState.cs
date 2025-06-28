using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerRoundState : BaseState
{
    public FSM fsm;

    public PlayerRoundState(FSM fsm)
    {
        this.fsm = fsm;
    }

    //进入状态时执行
    public override void OnEnter()
    {
        Debug.Log("进入玩家回合状态");
        RoundManager.Instance.round_Parameter.currentEState = Estate.playerRound;
        
    }

    //状态中持续执行
    public override void OnUpdate()
    {
        
    }
    //退出状态时执行
    public override void OnExit()
    {
        Debug.Log("退出玩家回合状态");
    }
}
