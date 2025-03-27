using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器管理器 主要用于开启、停止、重置等等操作来管理计时器
/// </summary>
public class TimerMgr : BaseManager<TimerMgr>
{
    private int TIMER_KEY = 0; //用于创建计时器唯一ID
    private Dictionary<int, TimerItem> timerDic = new Dictionary<int, TimerItem>(); //普通计时器池
    private Dictionary<int, TimerItem> realTimerDic = new Dictionary<int, TimerItem>(); //不受Time.timeScale影响的计时器池
    private List<TimerItem> delList = new List<TimerItem>(); //待移除的计时器池
    
    private WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(intervalTime); //为了节省频繁调用，直接使用同一个对象，否则该对象将频繁声明
    private WaitForSeconds waitForSeconds = new WaitForSeconds(intervalTime); //同上

    private Coroutine timer;
    private Coroutine realTimer;
    private const float intervalTime = 0.1f; //计时的时间粒度，单位：秒

    private TimerMgr() 
    {
        Start(); //默认开启计时器
    }

    /// <summary>
    /// 开启计时器
    /// </summary>
    public void Start()
    {
        timer = MonoMgr.Instance.StartCoroutine(RunTimer(false, timerDic));
        realTimer = MonoMgr.Instance.StartCoroutine(RunTimer(true, realTimerDic));
    }

    /// <summary>
    /// 停止计时器
    /// </summary>
    public void Stop()
    {
        MonoMgr.Instance.StopCoroutine(timer);
        MonoMgr.Instance.StopCoroutine(realTimer);
    }


    //运行所有计时器
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
                if(item.callBack != null) //运行间隔计时
                {
                    item.intervalTime -= (int)(intervalTime*1000);
                    if(item.intervalTime <= 0)
                    {
                        item.callBack.Invoke();
                        item.intervalTime = item.maxIntervalTime; //重置当前间隔时间
                    }
                }
                item.allTime -= (int)(intervalTime * 1000); //运行目标计时
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
    /// 创建单个计时器
    /// </summary>
    /// <param name="isRealTime">true则不受Time.timeScale影响</param>
    /// <param name="allTime">目标时间，单位：毫秒</param>
    /// <param name="overCallBack">达到目标时间的回调</param>
    /// <param name="intervalTime">间隔时间，单位：毫秒</param>
    /// <param name="callBack">每到间隔时间的回调</param>
    /// <returns>返回唯一ID 用于外部控制对应计时器</returns>
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
    /// 移除指定计时器
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
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
    /// 重置指定计时器
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
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
    /// 开启指定计时器
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
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
    /// 停止指定计时器
    /// </summary>
    /// <param name="keyID">计时器唯一ID</param>
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

