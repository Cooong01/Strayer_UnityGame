using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//AB包资源加载
public class ABResMgr : BaseManager<ABResMgr>
{
    //通过硬编码切换资源加载方式。true为编辑器加载，false为AB包加载。
    private bool isDebug = false;

    private ABResMgr() { }

    /// <summary>
    /// 用来加载资源的API。
    /// </summary>
    /// <typeparam name="T">加载资源类型</typeparam>
    /// <param name="abName">资源所在文件夹路径（Assets/Editor/ArtRes/之下）</param>
    /// <param name="resName">资源的名称</param>
    /// <param name="callBack">回调函数</param>
    /// <param name="isSync">true为同步加载，false为异步（仅限AB包情况下可同步加载，编辑器模式总是异步）</param>
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack = null, bool isSync = false) where T:Object
    {
#if UNITY_EDITOR
        if(isDebug)
        {
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
