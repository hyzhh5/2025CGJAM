using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRoundState : BaseState
{
    public FSM fsm;

    public EnemyRoundState(FSM fsm)
    {
        this.fsm = fsm;
    }

    //进入状态时执行
    public override void OnEnter()
    {
        Debug.Log("进入敌人回合状态");
    }

    //状态中持续执行
    public override void OnUpdate()
    {
        
    }
    //退出状态时执行
    public override void OnExit()
    {
        Debug.Log("退出敌人回合状态");        
    }
}
