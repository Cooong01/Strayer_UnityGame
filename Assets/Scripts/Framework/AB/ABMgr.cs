using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;


public class ABMgr : SingletonAutoMono<ABMgr>
{
    private AssetBundle mainAB = null; //主包
    private AssetBundleManifest manifest = null; //依赖清单

    //AB包信息类
    private class ABInfo
    {
        public AssetBundle ab;
        public int refCount; //引用计数

        public ABInfo(AssetBundle bundle)
        {
            ab = bundle;
            refCount = 1; // 初始加载时引用计数为1
        }
    }

    private Dictionary<string, ABInfo> abDic = new Dictionary<string, ABInfo>(); //AB包池

    private void Awake()
    {
        if( mainAB == null )
        {
            mainAB = AssetBundle.LoadFromFile( PathUrl + MainName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }

    //AB包主包加载路径
    private string PathUrl
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    //根据平台不同，主包名不同
    private string MainName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }


    //加载依赖包
    private void LoadDependencies(string abName)
    {
        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                abDic.Add(strs[i], new ABInfo(ab));
            }
        }
    }

    //根据泛型加载资源
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack = null, bool isSync = false) where T:Object
    {
        StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack, isSync));
    }


    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack = null, bool isSync = false) where T : Object
    {
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            string depName = strs[i];
            if (!abDic.ContainsKey(depName))
            {
                if(isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + depName);
                    abDic.Add(depName, new ABInfo(ab));
                }
                else
                {
                    abDic.Add(depName, null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + depName);
                    yield return req;
                    abDic[depName] = new ABInfo(req.assetBundle);
                }
            }
            else
            {
                while (abDic[depName] == null) yield return 0;
                abDic[depName].refCount++; // 增加依赖包的引用计数
            }
        }

        if (!abDic.ContainsKey(abName))
        {
            if (isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, new ABInfo(ab));
            }
            else
            {
                abDic.Add(abName, null);
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return req;
                abDic[abName] = new ABInfo(req.assetBundle);
            }
        }
        else
        {
            while (abDic[abName] == null) yield return 0;
            abDic[abName].refCount++; // 增加主包的引用计数
        }

        if(isSync)
        {
            T res = abDic[abName].ab.LoadAsset<T>(resName);
            callBack?.Invoke(res);
        }
        else
        {
            AssetBundleRequest abq = abDic[abName].ab.LoadAssetAsync<T>(resName);
            yield return abq;
            callBack?.Invoke(abq.asset as T);
        }
    }

    //根据反射Type加载资源
    public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack, bool isSync = false)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, type, callBack, isSync));
    }

    private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack, bool isSync)
    {
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                if (isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i], new ABInfo(ab));
                }
                else
                {
                    abDic.Add(strs[i], null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                    yield return req;
                    abDic[strs[i]] = new ABInfo(req.assetBundle);
                }
            }
            else
            {
                while (abDic[strs[i]] == null)
                {
                    yield return 0;
                }
                abDic[strs[i]].refCount++; // 增加依赖引用计数
            }
        }
        if (!abDic.ContainsKey(abName))
        {
            if (isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, new ABInfo(ab));
            }
            else
            {
                abDic.Add(abName, null);
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return req;
                abDic[abName] = new ABInfo(req.assetBundle);
            }
        }
        else
        {
            while (abDic[abName] == null)
            {
                yield return 0;
            }
            abDic[abName].refCount++; // 增加主包引用计数
        }

        if(isSync)
        {
            Object res = abDic[abName].ab.LoadAsset(resName, type);
            callBack(res);
        }
        else
        {
            AssetBundleRequest abq = abDic[abName].ab.LoadAssetAsync(resName, type);
            yield return abq;

            callBack(abq.asset);
        }
        
    }

    //资源直接加载为object
    public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack, bool isSync = false)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, callBack, isSync));
    }

    private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callBack, bool isSync)
    {
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                if (isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                    abDic.Add(strs[i], new ABInfo(ab));
                }
                else
                {
                    abDic.Add(strs[i], null);
                    AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + strs[i]);
                    yield return req;
                    abDic[strs[i]] = new ABInfo(req.assetBundle);
                }
            }
            else
            {
                while (abDic[strs[i]] == null)
                {
                    yield return 0;
                }
                abDic[strs[i]].refCount++; // 增加依赖引用计数
            }
        }
        if (!abDic.ContainsKey(abName))
        {
            if (isSync)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
                abDic.Add(abName, new ABInfo(ab));
            }
            else
            {
                abDic.Add(abName, null);
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(PathUrl + abName);
                yield return req;
                abDic[abName] = new ABInfo(req.assetBundle);
            }
        }
        else
        {
            while (abDic[abName] == null)
            {
                yield return 0;
            }
            abDic[abName].refCount++; // 增加主包引用计数
        }

        if(isSync)
        {
            Object obj = abDic[abName].ab.LoadAsset(resName);
            callBack(obj);
        }
        else
        {

            AssetBundleRequest abq = abDic[abName].ab.LoadAssetAsync(resName);
            yield return abq;
            callBack(abq.asset);
        }

    }

    //卸载AB包
    public void UnLoadAB(string name, UnityAction<bool> callBackResult)
    {
        if(abDic.TryGetValue(name, out ABInfo info))
        {
            info.refCount--; // 减少引用计数
            if (info.refCount <= 0)
            {
                info.ab.Unload(false);
                abDic.Remove(name);
                callBackResult?.Invoke(true);
            }
            else
            {
                callBackResult?.Invoke(false);
            }
        }
        else
        {
            callBackResult?.Invoke(false);
        }
    }

    //清空AB包
    public void ClearAB()
    {
        StopAllCoroutines();
        foreach (var pair in abDic)
        {
            pair.Value.ab.Unload(false);
        }
        abDic.Clear();
    }

    // 卸载引用计数为0的AB包
    public void UnloadUnusedABs()
    {
        List<string> toRemove = new List<string>();
        foreach (var pair in abDic)
        {
            if (pair.Value.refCount <= 0)
            {
                pair.Value.ab.Unload(false);
                toRemove.Add(pair.Key);
            }
        }
        foreach (string key in toRemove)
        {
            abDic.Remove(key);
        }
    }
}
