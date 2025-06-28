using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeginPanel : BasePanelZ
{
    public Button startBtn;
    public Button exitBtn;
    public Button settingBtn;

    protected override void Init()
    {
        startBtn.onClick.AddListener(() =>
        {
            //进入游戏
            UIManagerZ.Instance.HidePanel<BeginPanel>();
            SceneManager.LoadScene("GameScene");
            LevelManager.Instance.NextEnemy(false);
            //UIManagerZ.Instance.ShowPanel<GamePanel>();
        });

        exitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        
        settingBtn.onClick.AddListener(() =>
        {
            UIManagerZ.Instance.HidePanel<BeginPanel>();
            UIManagerZ.Instance.ShowPanel<SettingPanel>();
        });
    }

}