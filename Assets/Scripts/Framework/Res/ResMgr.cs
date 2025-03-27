using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��Դ��Ϣ����
/// </summary>
public abstract class ResInfoBase {
    public int refCount; //���ü���
}

/// <summary>
/// ��Դ��Ϣ����
/// </summary>
/// <typeparam name="T">��Դ����</typeparam>
public class ResInfo<T> : ResInfoBase
{
    public T asset; //��Դ��Ϣ
    public UnityAction<T> callBack;
    public Coroutine coroutine; //Э�̵���Ϣ
    public bool isDel; //���ü���Ϊ0��ʱ��ſ�ж��
    

    //�������ü���
    public void AddRefCount()
    {
        ++refCount;
    }

    //�������ü���
    public void SubRefCount()
    {
        --refCount;
        if (refCount < 0)
            Debug.LogError("���ü���С��0�ˣ�����ʹ�ú�ж���Ƿ����ִ��");
    }
}


/// <summary>
/// Resources ��Դ���ع�����
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    //���ڴ洢���ع�����Դ���߼����е���Դ������
    private Dictionary<string, ResInfoBase> resDic = new Dictionary<string, ResInfoBase>();

    private ResMgr() { }

    /// <summary>
    /// ͬ��������Դ
    /// </summary>
    public T Load<T>(string path) where T : UnityEngine.Object
    {
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> info;
        if (!resDic.ContainsKey(resName))
        {
            T res = Resources.Load<T>(path);
            info = new ResInfo<T>();
            info.asset = res;
            info.AddRefCount();
            resDic.Add(resName, info);
            return res;
        }
        else
        {
            info = resDic[resName] as ResInfo<T>;
            info.AddRefCount();
            if (info.asset == null)
            {
                MonoMgr.Instance.StopCoroutine(info.coroutine);
                T res = Resources.Load<T>(path);
                info.asset = res;
                info.callBack?.Invoke(res);
                info.callBack = null;
                info.coroutine = null;
                return res;
            }
            else
            {
                return info.asset;
            }
        }
    }

    /// <summary>
    /// �첽����Resources��Դ
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·����Resources�ļ����£�</param>
    public void LoadAsync<T>(string path, UnityAction<T> callBack) where T: UnityEngine.Object
    {
        string resName = path + "_" + typeof(T).Name;
        ResInfo<T> info;
        if (!resDic.ContainsKey(resName))
        {
            info = new ResInfo<T>();
            info.AddRefCount();
            resDic.Add(resName, info);
            info.callBack += callBack;
            info.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path));
        }
        else
        {
            info = resDic[resName] as ResInfo<T>;
            info.AddRefCount();
            if (info.asset == null) //�п�����Ϊ��ʱ�������첽�������ڽ����У�������info���ǻ�δ���س�asset�������Ҫ��callBack�洢һ��
                info.callBack += callBack;
            else
                callBack?.Invoke(info.asset);
        }
    }

    private IEnumerator ReallyLoadAsync<T>(string path) where T : UnityEngine.Object
    {
        ResourceRequest rq = Resources.LoadAsync<T>(path);
        yield return rq;
        string resName = path + "_" + typeof(T).Name;
        if (resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            resInfo.asset = rq.asset as T;
            if (resInfo.refCount == 0)
                UnloadAsset<T>(path, resInfo.isDel, null, false);
            else
            {
                resInfo.callBack?.Invoke(resInfo.asset);
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
        
    }

    /// <summary>
    /// �첽������Դ
    /// </summary>
    /// <param name="path">��Դ·����Resources�ļ����£�</param>
    public void LoadAsync(string path, Type type, UnityAction<UnityEngine.Object> callBack) 
    {
        string resName = path + "_" + type.Name;
        ResInfo<UnityEngine.Object> info;
        if (!resDic.ContainsKey(resName))
        {
            info = new ResInfo<UnityEngine.Object>();
            info.AddRefCount();
            resDic.Add(resName, info);
            info.callBack += callBack;
            info.coroutine = MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path, type));
        }
        else
        {
            info = resDic[resName] as ResInfo<UnityEngine.Object>;
            info.AddRefCount();
            if (info.asset == null)
                info.callBack += callBack;
            else
                callBack?.Invoke(info.asset);
        }
    }

    private IEnumerator ReallyLoadAsync(string path, Type type)
    {
        ResourceRequest rq = Resources.LoadAsync(path, type);
        yield return rq;

        string resName = path + "_" + type.Name;
        if (resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            resInfo.asset = rq.asset;
            if (resInfo.refCount == 0)
                UnloadAsset(path, type, resInfo.isDel, null, false);
            else
            {
                resInfo.callBack?.Invoke(resInfo.asset);
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
    }

    /// <summary>
    /// ж��Resources��Դ���������Ϣ
    /// </summary>
    /// <param name="path">��Դ·����Resources�£�</param>
    /// <param name="isDel">�Ƿ��ͷ���Դ</param>
    /// <param name="callBack">Ҫɾ���Ļص�</param>
    /// <param name="isSub">�Ƿ�������ü���</param>
    /// <typeparam name="T">��Դ����</typeparam>
    public void UnloadAsset<T>(string path, bool isDel = false, UnityAction<T> callBack = null, bool isSub = true)
    {
        //ж����Դ��������������Դ���ڡ�����Ϊ0���ⲿҪ��ж�ء�
        string resName = path + "_" + typeof(T).Name;
        if(resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            if(isSub) //��ѡ�Ƿ���ټ���
                resInfo.SubRefCount();
            resInfo.isDel = isDel;
            if(resInfo.asset != null && resInfo.refCount == 0 && resInfo.isDel)
            {
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset as UnityEngine.Object);
            }
            else if(resInfo.asset == null) //˵����Դ�����첽����
            {
                if (callBack != null)
                    resInfo.callBack -= callBack;
            }
        }
    }

    //���ݷ�������ж����Դ
    public void UnloadAsset(string path, Type type, bool isDel = false, UnityAction<UnityEngine.Object> callBack = null, bool isSub = true)
    {
        string resName = path + "_" + type.Name;
        if (resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            if(isSub)
                resInfo.SubRefCount();
            resInfo.isDel = isDel;
            if (resInfo.asset != null && resInfo.refCount == 0 && resInfo.isDel)
            {
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset);
            }
            else if (resInfo.asset == null)
            {
                if (callBack != null)
                    resInfo.callBack -= callBack;
            }
        }
    }

    /// <summary>
    /// �첽ж�ض�Ӧû��ʹ�õ�Resources��ص���Դ
    /// </summary>
    /// <param name="callBack">�ص�����</param>
    public void UnloadUnusedAssets(UnityAction callBack)
    {
        MonoMgr.Instance.StartCoroutine(ReallyUnloadUnusedAssets(callBack));
    }

    private IEnumerator ReallyUnloadUnusedAssets(UnityAction callBack)
    {
        List<string> list = new List<string>();
        foreach (string path in resDic.Keys)
        {
            if (resDic[path].refCount == 0)
                list.Add(path);
        }
        foreach (string path in list)
        {
            resDic.Remove(path);
        }

        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        callBack();
    }

    /// <summary>
    /// ��ȡָ����Դ�����ü���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public int GetRefCount<T>(string path)
    {
        string resName = path + "_" + typeof(T).Name;
        if(resDic.ContainsKey(resName))
        {
            return (resDic[resName] as ResInfo<T>).refCount;
        }
        return 0;
    }


    /// <summary>
    /// �����Դ�ֵ䣨һ�㲻ʹ�ã�
    /// </summary>
    /// <param name="callBack"></param>
    public void ClearDic(UnityAction callBack)
    {
        MonoMgr.Instance.StartCoroutine(ReallyClearDic(callBack));
    }

    private IEnumerator ReallyClearDic(UnityAction callBack)
    {
        resDic.Clear();
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;
        callBack();
    }
}
