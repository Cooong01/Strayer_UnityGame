using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��ʱ�������� ��Ҫ���ڿ�����ֹͣ�����õȵȲ����������ʱ��
/// </summary>
public class TimerMgr : BaseManager<TimerMgr>
{
    private int TIMER_KEY = 0; //���ڴ�����ʱ��ΨһID
    private Dictionary<int, TimerItem> timerDic = new Dictionary<int, TimerItem>(); //��ͨ��ʱ����
    private Dictionary<int, TimerItem> realTimerDic = new Dictionary<int, TimerItem>(); //����Time.timeScaleӰ��ļ�ʱ����
    private List<TimerItem> delList = new List<TimerItem>(); //���Ƴ��ļ�ʱ����
    
    private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(intervalTime); //Ϊ�˽�ʡƵ�����ã�ֱ��ʹ��ͬһ�����󣬷���ö���Ƶ������
    private WaitForSeconds waitForSeconds = new WaitForSeconds(intervalTime); //ͬ��

    private Coroutine timer;
    private Coroutine realTimer;
    private const float intervalTime = 0.1f; //��ʱ��ʱ�����ȣ���λ����

    private TimerMgr() 
    {
        Start(); //Ĭ�Ͽ�����ʱ��
    }

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public void Start()
    {
        timer = MonoMgr.Instance.StartCoroutine(RunTimer(false, timerDic));
        realTimer = MonoMgr.Instance.StartCoroutine(RunTimer(true, realTimerDic));
    }

    /// <summary>
    /// ֹͣ��ʱ��
    /// </summary>
    public void Stop()
    {
        MonoMgr.Instance.StopCoroutine(timer);
        MonoMgr.Instance.StopCoroutine(realTimer);
    }


    //�������м�ʱ��
    IEnumerator RunTimer(bool isRealTime, Dictionary<int, TimerItem> timerDic)
    {
        while (true)
        {
            if (isRealTime)
                yield return waitForSecondsRealtime;
            else
                yield return waitForSeconds;
            foreach (TimerItem item in timerDic.Values)
            {
                if (!item.isRuning)
                    continue;
                if(item.callBack != null) //���м����ʱ
                {
                    item.intervalTime -= (int)(intervalTime*1000);
                    if(item.intervalTime <= 0)
                    {
                        item.callBack.Invoke();
                        item.intervalTime = item.maxIntervalTime; //���õ�ǰ���ʱ��
                    }
                }
                item.allTime -= (int)(intervalTime * 1000); //����Ŀ���ʱ
                if(item.allTime <= 0)
                {
                    item.overCallBack.Invoke();
                    delList.Add(item);
                }
            }
            for (int i = 0; i < delList.Count; i++)
            {
                timerDic.Remove(delList[i].keyID);
                PoolMgr.Instance.PushObj(delList[i]);
            }
            delList.Clear();
        }
    }

    /// <summary>
    /// ����������ʱ��
    /// </summary>
    /// <param name="isRealTime">true����Time.timeScaleӰ��</param>
    /// <param name="allTime">Ŀ��ʱ�䣬��λ������</param>
    /// <param name="overCallBack">�ﵽĿ��ʱ��Ļص�</param>
    /// <param name="intervalTime">���ʱ�䣬��λ������</param>
    /// <param name="callBack">ÿ�����ʱ��Ļص�</param>
    /// <returns>����ΨһID �����ⲿ���ƶ�Ӧ��ʱ��</returns>
    public int CreateTimer(bool isRealTime, int allTime, UnityAction overCallBack, int intervalTime = 0, UnityAction callBack = null)
    {
        int keyID = ++TIMER_KEY;
        TimerItem timerItem = PoolMgr.Instance.GetObj<TimerItem>();
        timerItem.InitInfo(keyID, allTime, overCallBack, intervalTime, callBack);
        if (isRealTime)
            realTimerDic.Add(keyID, timerItem);
        else
            timerDic.Add(keyID, timerItem);
        return keyID;
    }

    /// <summary>
    /// �Ƴ�ָ����ʱ��
    /// </summary>
    /// <param name="keyID">��ʱ��ΨһID</param>
    public void RemoveTimer(int keyID)
    {
        if(timerDic.ContainsKey(keyID))
        {
            PoolMgr.Instance.PushObj(timerDic[keyID]);
            timerDic.Remove(keyID);
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            PoolMgr.Instance.PushObj(realTimerDic[keyID]);
            realTimerDic.Remove(keyID);
        }
    }

    /// <summary>
    /// ����ָ����ʱ��
    /// </summary>
    /// <param name="keyID">��ʱ��ΨһID</param>
    public void ResetTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].ResetTimer();
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].ResetTimer();
        }
    }

    /// <summary>
    /// ����ָ����ʱ��
    /// </summary>
    /// <param name="keyID">��ʱ��ΨһID</param>
    public void StartTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].isRuning = true;
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].isRuning = true;
        }
    }

    /// <summary>
    /// ָֹͣ����ʱ��
    /// </summary>
    /// <param name="keyID">��ʱ��ΨһID</param>
    public void StopTimer(int keyID)
    {
        if (timerDic.ContainsKey(keyID))
        {
            timerDic[keyID].isRuning = false;
        }
        else if (realTimerDic.ContainsKey(keyID))
        {
            realTimerDic[keyID].isRuning = false;
        }
    }
}

