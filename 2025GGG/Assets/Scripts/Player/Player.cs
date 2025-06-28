using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int money;//玩家生命值和金币
    public int atk;//玩家攻击力
    public bool isHuihe;//是否为玩家回合


    void Start()
    {
        money = 100;
        atk = 10;
    }

    //添加金额的方法
    public void AddMoney(int addMoney)
    {
        //调用老虎机或者海克斯的addmoney方法

    }

    public void ReduceMoney()
    {
        //调用老虎机或者海克斯的reduceMoney方法
    }

    public void Atk()
    {
        // TODO: Attack 调用敌人的takeDamage方法


    }


}
