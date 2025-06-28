using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 音频管理器
/// 用于统一管理游戏中的背景音乐和音效
/// </summary>
public class AudioManagerZ
{
    private static AudioManagerZ instance = new AudioManagerZ();
    public static AudioManagerZ Instance => instance;

    // 背景音乐播放器
    private AudioSource bgmAudioSource;
    // 音效播放器字典，用于管理多个音效
    private Dictionary<string, AudioSource> soundAudioSourceDic = new Dictionary<string, AudioSource>();
    // 音效对象的父物体
    private Transform soundRoot;

    // 默认音量设置
    private float bgmVolume = 0.6f;
    private float soundVolume = 1.0f;

    // 是否静音
    private bool isMuteBGM = false;
    private bool isMuteSound = false;

    // 缓存已加载的音频资源
    private Dictionary<string, AudioClip> audioClipDic = new Dictionary<string, AudioClip>();

    private AudioManagerZ()
    {
        // 创建一个空物体作为音频管理器的承载体
        GameObject audioObj = new GameObject("AudioManager");
        GameObject.DontDestroyOnLoad(audioObj);

        // 创建背景音乐播放器
        GameObject bgmObj = new GameObject("BGM");
        bgmObj.transform.SetParent(audioObj.transform);
        bgmAudioSource = bgmObj.AddComponent<AudioSource>();
        bgmAudioSource.playOnAwake = false;
        bgmAudioSource.loop = true;
        bgmAudioSource.volume = bgmVolume;

        // 创建音效根节点
        GameObject soundObj = new GameObject("Sounds");
        soundObj.transform.SetParent(audioObj.transform);
        soundRoot = soundObj.transform;
    }

    #region 背景音乐相关方法

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="bgmName">背景音乐资源名称</param>
    /// <param name="isLoop">是否循环播放</param>
    public void PlayBGM(string bgmName, bool isLoop = true)
    {
        if (string.IsNullOrEmpty(bgmName))
            return;

        // 如果当前正在播放相同的BGM，不做处理
        if (bgmAudioSource.clip != null && bgmAudioSource.clip.name == bgmName && bgmAudioSource.isPlaying)
            return;

        // 加载并播放背景音乐
        AudioClip clip = LoadAudioClip(bgmName, "BGM/");
        if (clip == null)
            return;

        bgmAudioSource.clip = clip;
        bgmAudioSource.loop = isLoop;
        bgmAudioSource.volume = isMuteBGM ? 0 : bgmVolume;
        bgmAudioSource.Play();
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Pause();
        }
    }

    /// <summary>
    /// 恢复播放背景音乐
    /// </summary>
    public void UnPauseBGM()
    {
        if (bgmAudioSource != null && !bgmAudioSource.isPlaying)
        {
            bgmAudioSource.UnPause();
        }
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBGM()
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop();
            bgmAudioSource.clip = null;
        }
    }

    /// <summary>
    /// 设置背景音乐音量
    /// </summary>
    /// <param name="volume">音量大小 0-1</param>
    public void SetBGMVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        bgmVolume = volume;
        if (bgmAudioSource != null && !isMuteBGM)
        {
            bgmAudioSource.volume = bgmVolume;
        }
    }

    /// <summary>
    /// 背景音乐静音切换
    /// </summary>
    /// <param name="isMute">是否静音</param>
    public void MuteBGM(bool isMute)
    {
        isMuteBGM = isMute;
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = isMuteBGM ? 0 : bgmVolume;
        }
    }

    #endregion

    #region 音效相关方法

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="soundName">音效资源名称</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="callback">播放完成回调</param>
    /// <returns>音效ID，用于后续控制该音效</returns>
    public string PlaySound(string soundName, bool isLoop = false, Action callback = null)
    {
        if (string.IsNullOrEmpty(soundName) || isMuteSound)
            return null;

        // 加载音效资源
        AudioClip clip = LoadAudioClip(soundName, "Sounds/");
        if (clip == null)
            return null;

        // 创建一个唯一ID
        string soundID = Guid.NewGuid().ToString();

        // 创建音效播放器
        GameObject soundObj = new GameObject(soundName);
        soundObj.transform.SetParent(soundRoot);
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = isLoop;
        audioSource.volume = soundVolume;
        audioSource.Play();

        // 存储到字典中
        soundAudioSourceDic[soundID] = audioSource;

        // 处理非循环音效的自动销毁
        if (!isLoop)
        {
            MonoBehaviour monoInstance = soundObj.AddComponent<MonoBehaviourInstance>();
            monoInstance.StartCoroutine(WaitForSoundFinish(soundID, clip.length, callback));
        }

        return soundID;
    }

    /// <summary>
    /// 等待音效播放结束
    /// </summary>
    private IEnumerator WaitForSoundFinish(string soundID, float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);

        // 执行回调
        callback?.Invoke();

        // 移除并销毁音效对象
        if (soundAudioSourceDic.TryGetValue(soundID, out AudioSource audioSource))
        {
            soundAudioSourceDic.Remove(soundID);
            if (audioSource != null && audioSource.gameObject != null)
            {
                GameObject.Destroy(audioSource.gameObject);
            }
        }
    }

    /// <summary>
    /// 停止指定音效
    /// </summary>
    /// <param name="soundID">音效ID</param>
    public void StopSound(string soundID)
    {
        if (string.IsNullOrEmpty(soundID))
            return;

        if (soundAudioSourceDic.TryGetValue(soundID, out AudioSource audioSource))
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                GameObject.Destroy(audioSource.gameObject);
            }
            soundAudioSourceDic.Remove(soundID);
        }
    }

    /// <summary>
    /// 停止所有音效
    /// </summary>
    public void StopAllSounds()
    {
        foreach (var audioSource in soundAudioSourceDic.Values)
        {
            if (audioSource != null && audioSource.gameObject != null)
            {
                GameObject.Destroy(audioSource.gameObject);
            }
        }
        soundAudioSourceDic.Clear();
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="volume">音量大小 0-1</param>
    public void SetSoundVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        soundVolume = volume;
        foreach (var audioSource in soundAudioSourceDic.Values)
        {
            if (audioSource != null && !isMuteSound)
            {
                audioSource.volume = soundVolume;
            }
        }
    }

    /// <summary>
    /// 音效静音切换
    /// </summary>
    /// <param name="isMute">是否静音</param>
    public void MuteSound(bool isMute)
    {
        isMuteSound = isMute;
        float targetVolume = isMuteSound ? 0 : soundVolume;
        foreach (var audioSource in soundAudioSourceDic.Values)
        {
            if (audioSource != null)
            {
                audioSource.volume = targetVolume;
            }
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 加载音频资源
    /// </summary>
    /// <param name="clipName">资源名称</param>
    /// <param name="path">相对路径</param>
    /// <returns>音频资源</returns>
    private AudioClip LoadAudioClip(string clipName, string path)
    {
        // 资源缓存检查
        string fullName = path + clipName;
        if (audioClipDic.TryGetValue(fullName, out AudioClip clip))
        {
            return clip;
        }

        // 从Resources加载资源
        clip = Resources.Load<AudioClip>(fullName);
        if (clip != null)
        {
            audioClipDic[fullName] = clip;
        }
        else
        {
            Debug.LogWarning($"音频资源加载失败: {fullName}");
        }

        return clip;
    }

    /// <summary>
    /// 释放音频资源缓存
    /// </summary>
    public void ClearAudioCache()
    {
        audioClipDic.Clear();
        Resources.UnloadUnusedAssets();
    }

    #endregion
}

/// <summary>
/// 辅助类，用于在AudioSource上执行协程
/// </summary>
public class MonoBehaviourInstance : MonoBehaviour
{
    // 仅用于执行协程
}
