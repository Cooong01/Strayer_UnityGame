using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

//����ؽṹ�������Ϊ���㣬��һ�㣨�������ǻ���ع��������󣬵ڶ��㣨�Ӷ�����ÿ����ͬԤ�����Ӧ�Ļ���أ������㣨������Ǿ���Ķ���

/// <summary>
/// ����ص��Ӷ���
/// </summary>
public class PoolData
{
    private Stack<GameObject> dataStack = new Stack<GameObject>(); //δʹ���������δʵ����
    private List<GameObject> usedList = new List<GameObject>(); //��ʹ�������,���Ѿ�ʵ������
    private int maxNum;//�Ӷ���������
    private GameObject rootObj;//�Ӷ���
    public int Count => dataStack.Count;//δʹ�����������
    public int UsedCount => usedList.Count;//��ʹ�����������
    public bool NeedCreate => usedList.Count < maxNum;//�Ƿ�ʵ�����������

    /// <summary>
    /// ���ӵĹ��캯��
    /// </summary>
    /// <param name="root">����ض���</param>
    /// <param name="name">�Ӷ�������</param>
    /// <param name="usedObj">�����</param>
    public PoolData(GameObject root, string name, GameObject usedObj)
    {
        if(PoolMgr.isOpenLayout)
        {
            rootObj = new GameObject(name);
            rootObj.transform.SetParent(root.transform);
        }
        PushUsedList(usedObj);
        PoolObj poolObj = usedObj.GetComponent<PoolObj>();
        if (poolObj == null)
        {
            Debug.LogError("��Ϊʹ�û���ع��ܵ�Ԥ���������ؼ̳���PoolObj�Ľű� ����������������,��ʵ�ֶ���ĳ�ʼ����ʧ�����");
            return;
        }
        maxNum = poolObj.maxNum;
    }
    /// <summary>
    /// ��������
    /// </summary>
    /// <returns></returns>
    public GameObject Pop()
    {
        GameObject obj;

        if (Count > 0)
        {
            obj = dataStack.Pop();
            usedList.Add(obj);
        }
        else //δʹ�õ���������������ʵ�����Ķ������ó���ʹ��
        {
            obj = usedList[0]; //�Ƚ��ȳ�
            usedList.RemoveAt(0);
            usedList.Add(obj);
        }
        
        obj.SetActive(true);
        if (PoolMgr.isOpenLayout)
            obj.transform.SetParent(null);

        return obj;
    }

    /// <summary>
    /// �������
    /// </summary>
    /// <param name="obj"></param>
    public void Push(GameObject obj)
    {
        obj.SetActive(false);
        if (PoolMgr.isOpenLayout)
            obj.transform.SetParent(rootObj.transform);
        dataStack.Push(obj);
        usedList.Remove(obj);
    }


    /// <summary>
    /// ʵ�����Ķ������
    /// </summary>
    /// <param name="obj"></param>
    public void PushUsedList(GameObject obj)
    {
        usedList.Add(obj);
    }
}

public abstract class PoolObjectBase { }

/// <summary>
/// ��Mono�Ķ���
/// </summary>
public class PoolObject<T> : PoolObjectBase where T:class
{
    public Queue<T> poolObjs = new Queue<T>();
}

/// <summary>
/// Mono�Ķ��󣬼̳д˽ӿ�
/// </summary>
public interface IPoolObject
{
    /// <summary>
    /// �������ݵķ���
    /// </summary>
    void Reset();
}

/// <summary>
/// �����(�����)������
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>(); //mono�Ķ����
    private Dictionary<string, PoolObjectBase> poolObjectDic = new Dictionary<string, PoolObjectBase>(); //��mono�Ķ����
    private GameObject poolObj;//������
    public static bool isOpenLayout = true; //�Ƿ���������Ӹ��Ӳ��ֹ���

    private PoolMgr() {
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool"); //��һ��GameObj��Ϊ�����������Ӳ���
    }

    /// <summary>
    /// �ö���ķ��������ڻ�ȡgameObj������Ҫ���أ����ʹ�ûص�����ȡObj��
    /// !!!�ö��������ؼ̳���PoolObj�Ľű�!!!
    /// </summary>
    /// <param name="ABname">���˶���������֮���·����</param>
    /// <param name="name">������</param>
    /// <param name="callBack">�ص�����</param>
    public void GetObj(string ABname,string name,UnityAction<GameObject> callBack = null)
    {
        GameObject obj;
        if(!poolDic.ContainsKey(name) ||
            (poolDic[name].Count == 0 && poolDic[name].NeedCreate))
        {
            ABResMgr.Instance.LoadResAsync<GameObject>(ABname,name, (t) =>
            {
                obj = GameObject.Instantiate(t);
                obj.name = name;//����(Clone)����

                if(!poolDic.ContainsKey(name))
                    poolDic.Add(name, new PoolData(poolObj, name, obj)); //���ӵ����־��Ƕ��������
                else
                    poolDic[name].PushUsedList(obj);
                callBack?.Invoke(obj);
            });

        }
        else
        {
            obj = poolDic[name].Pop();
            callBack?.Invoke(obj);
        }
    }

    /// <summary>
    /// ��ȡ��Mono����
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <returns></returns>
    public T GetObj<T>(string nameSpace = "") where T:class,IPoolObject,new()
    {
        string poolName = nameSpace + "_" + typeof(T).Name; //�ص����־��Ƕ�������
        if(poolObjectDic.ContainsKey(poolName))
        {
            PoolObject<T> pool = poolObjectDic[poolName] as PoolObject<T>;
            if(pool.poolObjs.Count > 0)
            {
                T obj = pool.poolObjs.Dequeue() as T;
                return obj;
            }
            else
            {
                T obj = new T();
                return obj;
            }
        }
        else
        {
            T obj = new T();
            return obj;
        }
        
    }

    /// <summary>
    /// ��������з������
    /// </summary>
    /// <param name="obj">ϣ������Ķ���</param>
    public void PushObj(GameObject obj)
    {
        //�����뵱�зŶ���
        poolDic[obj.name].Push(obj);
    }

    /// <summary>
    /// ������طŲ�Mono�Ķ���
    /// </summary>
    /// <typeparam name="T">��Ӧ����</typeparam>
    public void PushObj<T>(T obj, string nameSpace = "") where T:class,IPoolObject
    {
        if (obj == null)
            return;
        string poolName = nameSpace + "_" + typeof(T).Name;
        PoolObject<T> pool;
        if (poolObjectDic.ContainsKey(poolName))
            pool = poolObjectDic[poolName] as PoolObject<T>;
        else
        {
            pool = new PoolObject<T>();
            poolObjectDic.Add(poolName, pool);
        }
        obj.Reset();
        pool.poolObjs.Enqueue(obj);
    }
    
    /// <summary>
    /// ���ݳ������ƣ��������ƣ��Ƴ�����,�Ƿ�mono����
    /// </summary>
    /// <param name="name">�������ƣ��������ƣ�</param>
    public void RemovePool(string name)
    {
        if (poolDic.ContainsKey(name))
        {
            poolDic.Remove(name);
        }
        if (poolObjectDic.ContainsKey(name))
        {
            poolObjectDic.Remove(name);
        }
    }
    
    /// <summary>
    /// ���ݶ��������Ƴ�����,�Ƿ�mono����
    /// </summary>
    /// <param name="name">�������ƣ��������ƣ�</param>
    public void RemovePool(GameObject obj)
    {
        if (poolDic.ContainsKey(obj.name))
        {
            poolDic.Remove(obj.name);
        }
        if (poolObjectDic.ContainsKey(obj.name))
        {
            poolObjectDic.Remove(obj.name);
        }
    }
    /// <summary>
    /// ������г��ӣ�������ʹ�ô˹���
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj = null;
        poolObjectDic.Clear();
    }
}
