using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel :BasePanelZ
{
    public Slider musicSlider;
    public Slider soundSlider;

    public Button backBtn;

    protected override void Init()
    {
        // 初始化音量滑块
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 0.5f);

        // 设置背景音乐音量并保存
        musicSlider.onValueChanged.AddListener((value) => {
            AudioManagerZ.Instance.SetBGMVolume(value);
            PlayerPrefs.SetFloat("MusicVolume", value);
        });
        
        // 设置音效音量并保存
        soundSlider.onValueChanged.AddListener((value) => {
            AudioManagerZ.Instance.SetSoundVolume(value);
            PlayerPrefs.SetFloat("SoundVolume", value);
        });

        backBtn.onClick.AddListener(() =>
        {
            // 返回按钮逻辑
            SaveSettings();
            UIManagerZ.Instance.HidePanel<SettingPanel>();
            UIManagerZ.Instance.ShowPanel<BeginPanel>();
        });
    }
    
    /// <summary>
    /// 保存所有设置到PlayerPrefs
    /// </summary>
    private void SaveSettings()
    {
        // 保存音量设置
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.Save();
    }

    // 重写Update方法，调用基类的Update以支持淡入淡出效果
    protected override void Update()
    {
        base.Update(); // 调用基类的Update以支持淡入淡出效果
    }
    
    /// <summary>
    /// 当面板隐藏时确保保存设置
    /// </summary>
    public override void HideMe(UnityEngine.Events.UnityAction callback)
    {
        SaveSettings();
        base.HideMe(callback);
    }
}