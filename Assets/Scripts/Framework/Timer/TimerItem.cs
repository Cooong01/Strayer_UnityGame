using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��ʱ������
/// </summary>
public class TimerItem : IPoolObject
{
    public int keyID; //ΨһID
    
    public UnityAction overCallBack; //Ŀ��ʱ��ﵽ��Ļص�
    public UnityAction callBack; //���ʱ��ﵽ��Ļص�

    public int maxAllTime; //Ŀ��ʱ�䣬��λ������
    public int allTime; //��ǰ��ִ�е�Ŀ��ʱ��
    public int maxIntervalTime; //���ʱ�䣬��λ������
    public int intervalTime; //��ǰ��ִ�еļ��ʱ��

    public bool isRuning; //�Ƿ����ڼ�ʱ

    /// <summary>
    /// ��ʼ����ʱ������
    /// </summary>
    /// <param name="keyID">ΨһID</param>
    /// <param name="allTime">Ŀ��ʱ�䣬��λ������</param>
    /// <param name="overCallBack">�ﵽĿ��ʱ���Ļص�</param>
    /// <param name="intervalTime">���ʱ�䣬��λ������</param>
    /// <param name="callBack">ÿ�����ʱ���Ļص�</param>
    public void InitInfo(int keyID, int allTime, UnityAction overCallBack, int intervalTime = 0, UnityAction callBack = null)
    {
        this.keyID = keyID;
        this.maxAllTime = this.allTime = allTime;
        this.overCallBack = overCallBack;
        this.maxIntervalTime = this.intervalTime = intervalTime;
        this.callBack = callBack;
        this.isRuning = true;
    }

    /// <summary>
    /// ���ü�ʱ��
    /// </summary>
    public void ResetTimer()
    {
        this.allTime = this.maxAllTime;
        this.intervalTime = this.maxIntervalTime;
        this.isRuning = true;
    }

    /// <summary>
    /// �������
    /// </summary>
    public void Reset()
    {
        overCallBack = null;
        callBack = null;
    }
}
