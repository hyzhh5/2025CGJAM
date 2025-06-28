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

        //musicSlider.onValueChanged.AddListener(SetMusicVolume);
        //soundSlider.onValueChanged.AddListener(SetSoundVolume);

        backBtn.onClick.AddListener(() =>
        {
            // 返回按钮逻辑
            UIManagerZ.Instance.HidePanel<SettingPanel>();
            UIManagerZ.Instance.ShowPanel<BeginPanel>();
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
