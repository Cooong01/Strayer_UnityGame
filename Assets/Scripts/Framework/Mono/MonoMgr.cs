using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 公共Mono模块，所有非Mono脚本想执行mono的方法（比如开协程）可使用此脚本执行
/// </summary>
public class MonoMgr : SingletonAutoMono<MonoMgr>
{
    private event UnityAction updateEvent;
    private event UnityAction fixedUpdateEvent;
    private event UnityAction lateUpdateEvent;

    public void AddUpdateListener(UnityAction updateFun)
    {
        updateEvent += updateFun;
    }
    public void RemoveUpdateListener(UnityAction updateFun)
    {
        updateEvent -= updateFun;
    }

    public void AddFixedUpdateListener(UnityAction updateFun)
    {
        fixedUpdateEvent += updateFun;
    }
    public void RemoveFixedUpdateListener(UnityAction updateFun)
    {
        fixedUpdateEvent -= updateFun;
    }

    public void AddLateUpdateListener(UnityAction updateFun)
    {
        lateUpdateEvent += updateFun;
    }

    public void RemoveLateUpdateListener(UnityAction updateFun)
    {
        lateUpdateEvent -= updateFun;
    }


    private void Update()
    {
        updateEvent?.Invoke();
    }

    private void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        lateUpdateEvent?.Invoke();
    }
}
