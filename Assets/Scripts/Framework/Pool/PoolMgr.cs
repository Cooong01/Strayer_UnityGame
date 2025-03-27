using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

//缓存池结构：整体分为三层，第一层（父对象）是缓存池管理器对象，第二层（子对象）是每个不同预制体对应的缓存池，第三层（孙对象）是具体的对象。

/// <summary>
/// 缓存池的子对象
/// </summary>
public class PoolData
{
    private Stack<GameObject> dataStack = new Stack<GameObject>(); //未使用孙对象，暂未实例化
    private List<GameObject> usedList = new List<GameObject>(); //已使用孙对象,是已经实例化的
    private int maxNum;//子对象上限数
    private GameObject rootObj;//子对象
    public int Count => dataStack.Count;//未使用孙对象数量
    public int UsedCount => usedList.Count;//已使用孙对象数量
    public bool NeedCreate => usedList.Count < maxNum;//是否实例化新孙对象

    /// <summary>
    /// 池子的构造函数
    /// </summary>
    /// <param name="root">缓存池对象</param>
    /// <param name="name">子对象名字</param>
    /// <param name="usedObj">孙对象</param>
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
            Debug.LogError("请为使用缓存池功能的预设体对象挂载继承了PoolObj的脚本 用于设置数量上限,并实现对象的初始化和失活管理");
            return;
        }
        maxNum = poolObj.maxNum;
    }
    /// <summary>
    /// 弹出对象
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
        else //未使用的数量不够，从已实例化的对象中拿出来使用
        {
            obj = usedList[0]; //先进先出
            usedList.RemoveAt(0);
            usedList.Add(obj);
        }
        
        obj.SetActive(true);
        if (PoolMgr.isOpenLayout)
            obj.transform.SetParent(null);

        return obj;
    }

    /// <summary>
    /// 放入对象
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
    /// 实例化的对象入队
    /// </summary>
    /// <param name="obj"></param>
    public void PushUsedList(GameObject obj)
    {
        usedList.Add(obj);
    }
}

public abstract class PoolObjectBase { }

/// <summary>
/// 不Mono的对象
/// </summary>
public class PoolObject<T> : PoolObjectBase where T:class
{
    public Queue<T> poolObjs = new Queue<T>();
}

/// <summary>
/// Mono的对象，继承此接口
/// </summary>
public interface IPoolObject
{
    /// <summary>
    /// 重置数据的方法
    /// </summary>
    void Reset();
}

/// <summary>
/// 缓存池(对象池)管理器
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>(); //mono的对象池
    private Dictionary<string, PoolObjectBase> poolObjectDic = new Dictionary<string, PoolObjectBase>(); //不mono的对象池
    private GameObject poolObj;//父对象
    public static bool isOpenLayout = true; //是否开启管理池子父子布局功能

    private PoolMgr() {
        if (poolObj == null && isOpenLayout)
            poolObj = new GameObject("Pool"); //用一个GameObj作为根物体管理池子布局
    }

    /// <summary>
    /// 拿对象的方法。由于获取gameObj可能需要加载，因此使用回调来获取Obj。
    /// !!!该对象必须挂载继承了PoolObj的脚本!!!
    /// </summary>
    /// <param name="ABname">除了对象本身名称之外的路径名</param>
    /// <param name="name">对象名</param>
    /// <param name="callBack">回调函数</param>
    public void GetObj(string ABname,string name,UnityAction<GameObject> callBack = null)
    {
        GameObject obj;
        if(!poolDic.ContainsKey(name) ||
            (poolDic[name].Count == 0 && poolDic[name].NeedCreate))
        {
            ABResMgr.Instance.LoadResAsync<GameObject>(ABname,name, (t) =>
            {
                obj = GameObject.Instantiate(t);
                obj.name = name;//消除(Clone)字样

                if(!poolDic.ContainsKey(name))
                    poolDic.Add(name, new PoolData(poolObj, name, obj)); //池子的名字就是对象的名字
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
    /// 获取不Mono对象
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <returns></returns>
    public T GetObj<T>(string nameSpace = "") where T:class,IPoolObject,new()
    {
        string poolName = nameSpace + "_" + typeof(T).Name; //池的名字就是对象名字
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
    /// 往缓存池中放入对象
    /// </summary>
    /// <param name="obj">希望放入的对象</param>
    public void PushObj(GameObject obj)
    {
        //往抽屉当中放对象
        poolDic[obj.name].Push(obj);
    }

    /// <summary>
    /// 往缓存池放不Mono的对象
    /// </summary>
    /// <typeparam name="T">对应类型</typeparam>
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
    /// 根据池子名称（对象名称）移除池子,是否mono均可
    /// </summary>
    /// <param name="name">池子名称（对象名称）</param>
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
    /// 根据对象类型移除池子,是否mono均可
    /// </summary>
    /// <param name="name">池子名称（对象名称）</param>
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
    /// 清除所有池子，不建议使用此功能
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj = null;
        poolObjectDic.Clear();
    }
}
