using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectRoundState : BaseState
{
    public FSM fsm;

    public SelectRoundState(FSM fsm)
    {
        this.fsm = fsm;
    }

    //进入状态时执行
    public override void OnEnter()
    {
        Debug.Log("进入选择回合状态");
        RoundManager.Instance.round_Parameter.currentEState = Estate.selectRound;
    }

    //状态中持续执行
    public override void OnUpdate()
    {
        
    }
    //退出状态时执行
    public override void OnExit()
    {
        Debug.Log("退出选择回合状态");        
    }
}
