using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round_Parameter : FSM_Parameter
{

}
public class RoundManager : MonoBehaviour
{
    FSM fsm;
    public Round_Parameter round_Parameter = new Round_Parameter();

    RoundManager()
    {
        fsm = new FSM(round_Parameter);
        fsm.AddState(Estate.playerRound, new PlayerRoundState(fsm));
        fsm.AddState(Estate.enemyRound, new EnemyRoundState(fsm));
        fsm.AddState(Estate.selectRound, new SelectRoundState(fsm));
    }
    void Start()
    {
        
        fsm.SwitchState(Estate.playerRound);
    }
    void Update()
    {
        fsm.OnUpdate();
    }

}

