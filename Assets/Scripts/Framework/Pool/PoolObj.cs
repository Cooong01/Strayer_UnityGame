using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ҫʹ�û���ص�Ԥ�����ϣ������趨��Ԥ�������õĳ��Ӵ�С
/// </summary>
public abstract class PoolObj : MonoBehaviour,IPoolObject
{
    public int maxNum;
    public abstract void Reset();
}
