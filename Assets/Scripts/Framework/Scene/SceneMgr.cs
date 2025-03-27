using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换管理器
/// </summary>
public class SceneMgr : BaseManager<SceneMgr>
{
    private SceneMgr() { }
    
    /// <summary>
    /// 同步切换场景
    /// </summary>
    /// <param name="name">场景名称</param>
    public void LoadScene(string name, UnityAction callBack = null)
    {
        SceneManager.LoadScene(name);
        callBack?.Invoke();
        callBack = null;
    }

    /// <summary>
    /// 异步切换场景
    /// </summary>
    /// <param name="name">场景名称</param>
    public void LoadSceneAsyn(string name, UnityAction callBack = null)
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsyn(name, callBack));
    }

    private IEnumerator ReallyLoadSceneAsyn(string name, UnityAction callBack)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        while (!ao.isDone)
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_SceneLoadChange, ao.progress); //把场景加载进度传出去，写进度条可以使用该事件
            yield return 0;
        }
        EventCenter.Instance.EventTrigger(E_EventType.E_SceneLoadChange, 1f);

        callBack?.Invoke();
    }
}
