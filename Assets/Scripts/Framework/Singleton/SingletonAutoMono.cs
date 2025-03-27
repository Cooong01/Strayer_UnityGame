using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 懒汉式自动挂载的Mono的单例模式基类
/// </summary>
public class SingletonAutoMono<T> : MonoBehaviour where T:MonoBehaviour
{
    private static T instance;
    private static readonly object lockObject = new object();

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).ToString(); //默认把脚本名作为载体对象名
                        instance = obj.AddComponent<T>();
                        DontDestroyOnLoad(obj);
                    }
                }

            }
            return instance;
        }
    }

}
