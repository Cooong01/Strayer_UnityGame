using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源信息基类
/// </summary>
public abstract class ResInfoBase {
    public int refCount; //引用计数
}

/// <summary>
/// 资源信息对象
/// </summary>
/// <typeparam name="T">资源类型</typeparam>
public class ResInfo<T> : ResInfoBase
{
    public T asset; //资源信息
    public UnityAction<T> callBack;
    public Coroutine coroutine; //协程的信息
    public bool isDel; //引用计数为0的时候才可卸载
    

    //增加引用计数
    public void AddRefCount()
    {
        ++refCount;
    }

    //减少引用计数
    public void SubRefCount()
    {
        --refCount;
        if (refCount < 0)
            Debug.LogError("引用计数小于0了，请检查使用和卸载是否配对执行");
    }
}


/// <summary>
/// Resources 资源加载管理器
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    //用于存储加载过的资源或者加载中的资源的容器
    private Dictionary<string, ResInfoBase> resDic = new Dictionary<string, ResInfoBase>();

    private ResMgr() { }

    /// <summary>
    /// 同步加载资源
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
    /// 异步加载Resources资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径（Resources文件夹下）</param>
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
            if (info.asset == null) //判空是因为此时可能有异步加载正在进行中，创建了info但是还未加载出asset，因此需要把callBack存储一下
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
    /// 异步加载资源
    /// </summary>
    /// <param name="path">资源路径（Resources文件夹下）</param>
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
    /// 卸载Resources资源或者相关信息
    /// </summary>
    /// <param name="path">资源路径（Resources下）</param>
    /// <param name="isDel">是否释放资源</param>
    /// <param name="callBack">要删掉的回调</param>
    /// <param name="isSub">是否减少引用计数</param>
    /// <typeparam name="T">资源类型</typeparam>
    public void UnloadAsset<T>(string path, bool isDel = false, UnityAction<T> callBack = null, bool isSub = true)
    {
        //卸载资源有三个条件：资源存在、计数为0、外部要求卸载。
        string resName = path + "_" + typeof(T).Name;
        if(resDic.ContainsKey(resName))
        {
            ResInfo<T> resInfo = resDic[resName] as ResInfo<T>;
            if(isSub) //可选是否减少计数
                resInfo.SubRefCount();
            resInfo.isDel = isDel;
            if(resInfo.asset != null && resInfo.refCount == 0 && resInfo.isDel)
            {
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset as UnityEngine.Object);
            }
            else if(resInfo.asset == null) //说明资源还在异步加载
            {
                if (callBack != null)
                    resInfo.callBack -= callBack;
            }
        }
    }

    //根据反射类型卸载资源
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
    /// 异步卸载对应没有使用的Resources相关的资源
    /// </summary>
    /// <param name="callBack">回调函数</param>
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
    /// 获取指定资源的引用计数
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
    /// 清空资源字典（一般不使用）
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
