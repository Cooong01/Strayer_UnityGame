using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 需要手动挂载的单例模式基类
/// </summary>
public class SingletonMono<T>: MonoBehaviour where T:MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this as T;
        DontDestroyOnLoad(this.gameObject);
    }
}
