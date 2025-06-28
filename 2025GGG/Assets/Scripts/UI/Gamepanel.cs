using System.Collections;
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


    protected override void Init()
    {

    }
    private void UpdateUI()
    {
        float[] gailv = DrawCardsManager.Instance.UpdateProbability();
        //atk.text = "攻击力：" + DrawCardsManager.Instance.atk;
        atkGailv.text = "攻击概率：" + gailv[0];
        moneyGailv.text = "金币概率：" + gailv[1];
        magicGailv.text = "特殊卡概率：" + gailv[2];
        atkUpGailv.text = "攻击力提升概率：" + gailv[3];
        roundEndGailv.text = "回合结束概率：" + gailv[4];
    }

}
