using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ����ʽ�Զ����ص�Mono�ĵ���ģʽ����
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
                        obj.name = typeof(T).ToString(); //Ĭ�ϰѽű�����Ϊ���������
                        instance = obj.AddComponent<T>();
                        DontDestroyOnLoad(obj);
                    }
                }

            }
            return instance;
        }
    }

}
