using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 用于进行加载AB相关资源的整合 在开发中可以通过EditorResMgr去加载对应资源进行测试
/// </summary>
public class ABResMgr : BaseManager<ABResMgr>
{
    //如果是true会通过EditorResMgr去加载 如果是false会通过ABMgr AB包的形式去加载
    private bool isDebug = true;

    private ABResMgr() { }

    /// <summary>
    /// 用来加载资源的API。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName">文件包的名字，小写。也就是说，AB包的文件放在Editor文件下第一级文件夹</param>
    /// <param name="resName">资源的名字</param>
    /// <param name="callBack">回调函数</param>
    /// <param name="isSync">true为同步加载，false为异步（仅限AB包情况下，编辑器模式全是异步）</param>
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack = null, bool isSync = false) where T:Object
    {
#if UNITY_EDITOR
        if(isDebug)
        {
            //我们自定义了一个AB包中资源的管理方式 对应文件夹名 就是包名 
            T res = EditorResMgr.Instance.LoadEditorRes<T>($"{abName}/{resName}");
            if (callBack != null)
            {
                callBack?.Invoke(res as T);
            }
        }
        else
        {
            ABMgr.Instance.LoadResAsync<T>(abName, resName, callBack, isSync);
        }
#else
        ABMgr.Instance.LoadResAsync<T>(abName, resName, callBack, isSync);
#endif
    }
}
