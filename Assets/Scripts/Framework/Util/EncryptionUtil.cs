using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �򵥵ĶԳƼ��ܹ���
/// </summary>
public class EncryptionUtil
{
    /// <summary>
    /// ��ȡ�����Կ����Ҫ������Կ�����ݸ���Կ���жԳƼ��ܺͽ��ܡ�
    /// </summary>
    /// <returns>��Կ</returns>
    public static int GetRandomKey()
    {
        return Random.Range(1, 10000) + 5;
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="value">����</param>
    /// <param name="key">��Կ</param>
    /// <returns>���ܺ�����</returns>
    public static int LockValue(int value, int key)
    {
        //������
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        value += key;
        return value;
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="value">����</param>
    /// <param name="key">��Կ</param>
    /// <returns>���ܺ�����</returns>
    public static long LockValue(long value, int key)
    {
        //������
        value = value ^ (key % 9);
        value = value ^ 0xADAD;
        value = value ^ (1 << 5);
        value += key;
        return value;
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="value">����</param>
    /// <param name="key">��Կ</param>
    /// <returns>ԭʼ����</returns>
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
    /// ��������
    /// </summary>
    /// <param name="value">����</param>
    /// <param name="key">��Կ</param>
    /// <returns>ԭʼ����</returns>
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
