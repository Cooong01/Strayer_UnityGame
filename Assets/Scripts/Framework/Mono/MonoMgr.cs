using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ����Monoģ�飬���з�Mono�ű���ִ��mono�ķ��������翪Э�̣���ʹ�ô˽ű�ִ��
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
