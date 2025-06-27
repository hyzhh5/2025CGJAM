using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerRoundState : BaseState
{
    public FSM fsm;
    Round_Parameter round_Parameter;

    public PlayerRoundState(FSM fsm)
    {
        this.fsm = fsm;
        round_Parameter = fsm.parameter as Round_Parameter;


    }
    
    //进入状态时执行
    public override void OnEnter()
    {

    }

    //状态中持续执行
    public override void OnUpdate()
    {
        
    }
    //退出状态时执行
    public override void OnExit()
    {
        
        
    }
}
