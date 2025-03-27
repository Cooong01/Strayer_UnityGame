using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 计时器对象
/// </summary>
public class TimerItem : IPoolObject
{
    public int keyID; //唯一ID
    
    public UnityAction overCallBack; //目标时间达到后的回调
    public UnityAction callBack; //间隔时间达到后的回调

    public int maxAllTime; //目标时间，单位：毫秒
    public int allTime; //当前已执行的目标时间
    public int maxIntervalTime; //间隔时间，单位：毫秒
    public int intervalTime; //当前已执行的间隔时间

    public bool isRuning; //是否正在计时

    /// <summary>
    /// 初始化计时器数据
    /// </summary>
    /// <param name="keyID">唯一ID</param>
    /// <param name="allTime">目标时间，单位：毫秒</param>
    /// <param name="overCallBack">达到目标时间后的回调</param>
    /// <param name="intervalTime">间隔时间，单位：毫秒</param>
    /// <param name="callBack">每到间隔时间后的回调</param>
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
    /// 重置计时器
    /// </summary>
    public void ResetTimer()
    {
        this.allTime = this.maxAllTime;
        this.intervalTime = this.maxIntervalTime;
        this.isRuning = true;
    }

    /// <summary>
    /// 清除引用
    /// </summary>
    public void Reset()
    {
        overCallBack = null;
        callBack = null;
    }
}
