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
        // 播放背景音乐
        AudioManagerZ.Instance.PlayBGM("世界上最小的哈基米");

        // 从PlayerPrefs加载并应用音量设置
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float soundVolume = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
        AudioManagerZ.Instance.SetBGMVolume(musicVolume);
        AudioManagerZ.Instance.SetSoundVolume(soundVolume);
        
        startBtn.onClick.AddListener(() =>
        {
            //进入游戏
            UIManagerZ.Instance.HidePanel<BeginPanel>();
            SceneManager.LoadScene("GameScene");
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