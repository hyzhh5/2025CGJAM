using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//状态类型枚举
public enum Estate
{
    playerRound,//玩家回合
    enemyRound,//敌人回合
    selectRound,//选择回合
}


//所有状态共享的参数
public class FSM_Parameter 
{
    public Estate currentEState;
   
}


public class FSM
{

    //状态名枚举和具体状态类的索引
    public Dictionary<Estate, BaseState> StateDict;


    //每个状态共享的参数
    public FSM_Parameter parameter;

    //当前状态
    public BaseState currentState;


    public FSM(FSM_Parameter P)
    {

        parameter = P;
        StateDict = new Dictionary<Estate, BaseState>();
    }



    //添加状态
    public void AddState(Estate estate, BaseState baseState)
    {
        if (!StateDict.ContainsKey(estate))
        {
            StateDict.Add(estate, baseState);
        }
    }


    //切换状态
    public void SwitchState(Estate estate)
    {

        //目标状态是否已被添加
        if (!StateDict.ContainsKey(estate))
        {
            return;
        }

        //退出当前状态
        if (currentState != null)
        {
            currentState.OnExit();
        }

        //切换状态
        currentState = StateDict[estate];
        parameter.currentEState = estate;


        currentState?.OnEnter();

    }

    public void OnUpdate()
    {
        currentState?.OnUpdate();
    }

}

