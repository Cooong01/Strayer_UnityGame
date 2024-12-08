using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void OnEnter();// 进入状态时的方法
    void OnUpdate();// 更新方法
    void OnFixedUpdate();// 固定更新方法
    void OnExit();// 退出状态时的方法
}

