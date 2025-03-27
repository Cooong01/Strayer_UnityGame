using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// �����л�������
/// </summary>
public class SceneMgr : BaseManager<SceneMgr>
{
    private SceneMgr() { }
    
    /// <summary>
    /// ͬ���л�����
    /// </summary>
    /// <param name="name">��������</param>
    public void LoadScene(string name, UnityAction callBack = null)
    {
        SceneManager.LoadScene(name);
        callBack?.Invoke();
        callBack = null;
    }

    /// <summary>
    /// �첽�л�����
    /// </summary>
    /// <param name="name">��������</param>
    public void LoadSceneAsyn(string name, UnityAction callBack = null)
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsyn(name, callBack));
    }

    private IEnumerator ReallyLoadSceneAsyn(string name, UnityAction callBack)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        while (!ao.isDone)
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_SceneLoadChange, ao.progress); //�ѳ������ؽ��ȴ���ȥ��д����������ʹ�ø��¼�
            yield return 0;
        }
        EventCenter.Instance.EventTrigger(E_EventType.E_SceneLoadChange, 1f);

        callBack?.Invoke();
    }
}
