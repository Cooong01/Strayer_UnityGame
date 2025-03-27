using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂在需要使用缓存池的预制体上，用于设定该预制体所用的池子大小
/// </summary>
public abstract class PoolObj : MonoBehaviour,IPoolObject
{
    public int maxNum;
    public abstract void Reset();
}
