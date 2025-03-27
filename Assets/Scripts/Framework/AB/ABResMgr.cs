using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//AB����Դ����
public class ABResMgr : BaseManager<ABResMgr>
{
    //ͨ��Ӳ�����л���Դ���ط�ʽ��trueΪ�༭�����أ�falseΪAB�����ء�
    private bool isDebug = false;

    private ABResMgr() { }

    /// <summary>
    /// ����������Դ��API��
    /// </summary>
    /// <typeparam name="T">������Դ����</typeparam>
    /// <param name="abName">��Դ�����ļ���·����Assets/Editor/ArtRes/֮�£�</param>
    /// <param name="resName">��Դ������</param>
    /// <param name="callBack">�ص�����</param>
    /// <param name="isSync">trueΪͬ�����أ�falseΪ�첽������AB������¿�ͬ�����أ��༭��ģʽ�����첽��</param>
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
