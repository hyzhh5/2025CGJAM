using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BasePanelZ:MonoBehaviour
{ 
    // 控制面板透明度的组件
    private CanvasGroup canvasGroup;
    //面板透明度改变的速度（淡入淡出的效果）
    private float alphaSpeed = 10;
    //面板当前状态的标识
    public bool isShow = false;
    //当隐藏完毕后的行为委托
    private UnityAction hideCallback = null;
   
    protected virtual void Awake()
    {
        //一开始获取面板上挂载的组件
        canvasGroup = GetComponent<CanvasGroup>();
        //如果没有添加canvasGroup则添加该脚本
        if(canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    protected virtual void Start()
    {
        Init();
    }

    /// <summary>
    /// 注册控制事件的方法 所有子面板都需要去注册一些控件事件
    /// 所以写成抽象方法 让子类必须去实现
    /// </summary>
    protected abstract void Init();
   
    public virtual void ShowMe()
    {
        canvasGroup.alpha = 0;
        isShow = true;
    }

    public virtual void HideMe(UnityAction callback)
    {
        canvasGroup.alpha = 1;
        isShow = false;
        
        hideCallback = callback;
    }

    protected virtual void Update()
    {
        //淡入
        if (isShow && alphaSpeed != 1)
        {
            canvasGroup.alpha += alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
                canvasGroup.alpha = 1;
        }
        //淡出
        else if (!isShow)
        {
            canvasGroup.alpha -= alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                //让面板透明度淡出完成后发生的行为委托
                hideCallback?.Invoke();
            }
        }
    }
    
}
