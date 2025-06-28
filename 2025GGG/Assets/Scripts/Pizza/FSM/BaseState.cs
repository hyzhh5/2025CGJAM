using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//状态基类
public class BaseState
{

    //进入状态时执行
    public virtual void OnEnter() { }

    //状态中持续执行
    public virtual void OnUpdate() { }
    //退出状态时执行
    public virtual void OnExit() { }

}


