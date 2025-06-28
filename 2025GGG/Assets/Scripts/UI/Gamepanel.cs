using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamePanel : BasePanelZ
{
    public TMP_Text atk;//攻击力
    public TMP_Text money;//金币

    public TMP_Text atkGailv;//攻击概率
    public TMP_Text moneyGailv;//金币概率

    public TMP_Text atkUpGailv;//攻击力提升概率
    public TMP_Text roundEndGailv;//回合结束概率
    public TMP_Text magicGailv;//特殊卡概率

    public static GamePanel Instance{get; private set;}
    protected override void Awake()
    {
        base.Awake();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    protected override void Update()
    {

    }

    protected override void Init()
    {

    }
    private void OnEnable()
    {     

    }
    private void OnDisable()
    {

    }
    public void UpdateUI()
    {
        float[] gailv = DrawCardsManager.Instance.UpdateProbability();
        atk.text = "攻击力：" + Player.Instance.attackPower;
        money.text = "金币：" + Player.Instance.coin;
        atkGailv.text = "攻击概率：" + gailv[0];
        moneyGailv.text = "金币概率：" + gailv[1];
        magicGailv.text = "特殊卡概率：" + gailv[2];
        atkUpGailv.text = "攻击力提升概率：" + gailv[3];
        roundEndGailv.text = "回合结束概率：" + gailv[4];
    }

}
