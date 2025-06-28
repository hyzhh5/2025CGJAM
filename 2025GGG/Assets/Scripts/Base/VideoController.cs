using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string nextSceneName; // 下一个场景的名称

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished; // 当视频播放完毕时调用OnVideoFinished方法
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName); // 加载下一个场景
    }
}

