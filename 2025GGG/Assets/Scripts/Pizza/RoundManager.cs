using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round_Parameter : FSM_Parameter
{

}
public class RoundManager : MonoBehaviour
{
    FSM fsm;
    public Round_Parameter round_Parameter = new Round_Parameter();//only

    public static RoundManager Instance { get; private set; }

    public int maxNumDraws = 3;//最大抽卡次数
    // 声明事件
    public static event System.Action OnEnemyRoundStart;
    public static event System.Action OnEnemyRoundEnd;


    RoundManager()
    {
        fsm = new FSM(round_Parameter);
        fsm.AddState(Estate.playerRound, new PlayerRoundState(fsm));
        fsm.AddState(Estate.enemyRound, new EnemyRoundState(fsm));
        fsm.AddState(Estate.selectRound, new SelectRoundState(fsm));
    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {

        fsm.SwitchState(Estate.playerRound);
    }
    void Update()
    {
        fsm.OnUpdate();
    }

    [ContextMenu("Switch to Player Round")]
    public void SwitchToPlayerRound()
    {
        fsm.SwitchState(Estate.playerRound);
    }
    [ContextMenu("Switch to Enemy Round")]
    public void SwitchToEnemyRound()
    {
        fsm.SwitchState(Estate.enemyRound);
    }
    [ContextMenu("Switch to Select Round")]
    public void SwitchToSelectRound()
    {
        fsm.SwitchState(Estate.selectRound);
    }
    #region 事件订阅
    public void TriggerEnemyRoundStart()
    {
        OnEnemyRoundStart?.Invoke();
    }
    #endregion
}

