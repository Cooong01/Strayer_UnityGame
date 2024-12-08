using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ���ڽ��м���AB�����Դ������ �ڿ����п���ͨ��EditorResMgrȥ���ض�Ӧ��Դ���в���
/// </summary>
public class ABResMgr : BaseManager<ABResMgr>
{
    //�����true��ͨ��EditorResMgrȥ���� �����false��ͨ��ABMgr AB������ʽȥ����
    private bool isDebug = true;

    private ABResMgr() { }

    /// <summary>
    /// ����������Դ��API��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName">�ļ��������֣�Сд��Ҳ����˵��AB�����ļ�����Editor�ļ��µ�һ���ļ���</param>
    /// <param name="resName">��Դ������</param>
    /// <param name="callBack">�ص�����</param>
    /// <param name="isSync">trueΪͬ�����أ�falseΪ�첽������AB������£��༭��ģʽȫ���첽��</param>
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack = null, bool isSync = false) where T:Object
    {
#if UNITY_EDITOR
        if(isDebug)
        {
            //�����Զ�����һ��AB������Դ�Ĺ���ʽ ��Ӧ�ļ����� ���ǰ��� 
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
