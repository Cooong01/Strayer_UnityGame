using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 简单的对称加密工具
/// </summary>
public class EncryptionUtil
{
    /// <summary>
    /// 获取随机密钥。需要管理密钥，根据该密钥进行对称加密和解密。
    /// </summary>
    /// <returns>密钥</returns>
    public static int GetRandomKey()
    {
        return Random.Range(1, 10000) + 5;
    }

    /// <summary>
    /// 加密数据
    /// </summary>
    /// <param name="value">数据</param>
    /// <param name="key">密钥</param>
    /// <returns>加密后数据</returns>
    public static int LockValue(int value, int key)
    {
        //异或加密
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        value += key;
        return value;
    }

    /// <summary>
    /// 加密数据
    /// </summary>
    /// <param name="value">数据</param>
    /// <param name="key">密钥</param>
    /// <returns>加密后数据</returns>
    public static long LockValue(long value, int key)
    {
        //异或加密
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        value += key;
        return value;
    }

    /// <summary>
    /// 解密数据
    /// </summary>
    /// <param name="value">数据</param>
    /// <param name="key">密钥</param>
    /// <returns>原始数据</returns>
    public static int UnLoackValue(int value, int key)
    {
        if (value == 0)
            return value;
        value -= key;
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        return value;
    }

    /// <summary>
    /// 解密数据
    /// </summary>
    /// <param name="value">数据</param>
    /// <param name="key">密钥</param>
    /// <returns>原始数据</returns>
    public static long UnLoackValue(long value, int key)
    {
        if (value == 0)
            return value;
        value -= key;
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        return value;
    }
}
