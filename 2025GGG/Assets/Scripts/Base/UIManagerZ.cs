
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerZ
{
    private static UIManagerZ instance = new UIManagerZ();
    public static UIManagerZ Instance => instance;

    public int i = 2;

    //用于存储显示着的面板 每显示一个面板 就会存入这个字典
    //隐藏面板时 直接获取字典中的对应面板 进行隐藏
    private Dictionary<string, BasePanelZ> panelDic = new Dictionary<string, BasePanelZ>();

    //场景中的Canvas对象 用于设置为面板的父对象
    private Transform canvasTrans;

    private UIManagerZ()
    {
        //得到场景中的Canvas对象
        GameObject canvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/Canvas"));
        canvasTrans = canvas.transform;
        //通过过场景不移除该对象 保证这个游戏过程中只有一个canvas对象
        GameObject.DontDestroyOnLoad(canvas);
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板类名</typeparam>
    /// <returns></returns>
    public T ShowPanel<T>() where T : BasePanelZ
    {
        //人为保证 泛型T的类型 和面板预设体的名字一样  便于使用
        string panelName = typeof(T).Name;
        //判断字典中是否已经显示了这个面板
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        //根据面板名字 动态创建预设体 设置父对象
        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("UI/Panel/" + panelName));
        //把这个对象 放到场景中的Canvas下
        panelObj.transform.SetParent(canvasTrans, false);

        //执行面板显示逻辑
        T panel = panelObj.GetComponent<T>();
        //显示后讲面板存储到字典中 方便之后的 获取 和 隐藏 
        panelDic.Add(panelName, panel);
        //调用自己的显示逻辑
        panel.ShowMe();

        return panel;
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="isFade">是否淡出完毕过后才删除面板 默认true</param>
    /// <typeparam name="T">面板类名</typeparam>
    public void HidePanel<T>(bool isFade = true) where T : BasePanelZ
    {
        //根据泛型得名字
        string panelName = typeof(T).Name;
        //判断字典中是否有想要隐藏的面板
        if (panelDic.ContainsKey(panelName))
        {
            //让面板淡出完毕后删除
            if (isFade)
            {
                panelDic[panelName].HideMe(() =>
                {
                    //删除对象
                    GameObject.Destroy(panelDic[panelName].gameObject);
                    //删除字典里面存储的面板脚本
                    panelDic.Remove(panelName);
                });
            }
            //或者不让面板淡出 直接删除
            else
            {
                //删除对象
                GameObject.Destroy(panelDic[panelName].gameObject);
                //删除字典里面存储的面板脚本
                panelDic.Remove(panelName);
            }
        }
    }

    //得到面板
    public T GetPanel<T>() where T : BasePanelZ
    {
        string panelName = typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
            return panelDic[panelName] as T;
        //如果没有对应面版显示就返回空
        return null;
    }
}
