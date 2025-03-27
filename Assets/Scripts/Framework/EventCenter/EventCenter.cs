using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//事件基类，便于事件池同时装有参和无参委托
public abstract class EventInfoBase{ }

//有参委托
public class EventInfo<T>:EventInfoBase
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

//无参委托
public class EventInfo: EventInfoBase
{
    public UnityAction actions;
     
    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}


/// <summary>
/// 事件中心模块。
/// 使用流程：在 E_EventType 里填写事件类型枚举，然后调用触发和监听方法。添加监听的时候自动注册事件。
/// </summary>
public class EventCenter: BaseManager<EventCenter>
{
    private Dictionary<E_EventType, EventInfoBase> eventDic = new Dictionary<E_EventType, EventInfoBase>(); //事件池
    private EventCenter() { }

    /// <summary>
    /// 触发有参事件；如果需要传入两个以上参数，以元组形式传入
    /// </summary>
    /// <param name="eventName">事件类型</param>
    public void EventTrigger<T>(E_EventType eventName, T info)
    {
        if(info.GetType().Name != "Vector3")
        {
            Debug.Log("有参事件名：" + eventName + "传入了信息：" + info +"类型为" + info.GetType().Name);
        }
        if(eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions?.Invoke(info);
        }
    }

    /// <summary>
    /// 触发有参事件
    /// </summary>
    /// <param name="eventName">事件类型</param>
    public void EventTrigger(E_EventType eventName)
    {
        Debug.Log("无参事件名：" + eventName + "被触发了");
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions?.Invoke();
        }
    }


    /// <summary>
    /// 添加有参事件监听；需要两个以上参数时，通过元组来接参数。
    /// </summary>
    /// <param name="eventName">事件类型</param>
    /// <param name="func">回调函数</param>
    public void AddEventListener<T>(E_EventType eventName, UnityAction<T> func)
    {
        Debug.Log("事件名：" + eventName + "添加了有参监听函数，函数名："+func.Method.Name);
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T>(func));
        }
    }

    /// <summary>
    /// 添加无参事件监听；
    /// </summary>
    /// <param name="eventName">事件类型</param>
    /// <param name="func">回调函数</param>
    public void AddEventListener(E_EventType eventName, UnityAction func)
    {
        Debug.Log("事件名：" + eventName + "添加了无参监听函数，函数名：" + func.Method.Name);
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo(func));
        }
    }

    /// <summary>
    /// 移除有参事件监听
    /// </summary>
    /// <param name="eventName">事件类型</param>
    /// <param name="func">回调函数</param>
    public void RemoveEventListener<T>(E_EventType eventName, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T>).actions -= func;
    }
    
    /// <summary>
    /// 移除无参事件监听
    /// </summary>
    /// <param name="eventName">事件类型</param>
    /// <param name="func">回调函数</param>
    public void RemoveEventListener(E_EventType eventName, UnityAction func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo).actions -= func;
    }

    /// <summary>
    /// 清空所有事件
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }

    /// <summary>
    /// 清除指定事件监听
    /// </summary>
    /// <param name="eventName">事件类型</param>
    public void Clear(E_EventType eventName)
    {
        if (eventDic.ContainsKey(eventName))
            eventDic.Remove(eventName);
    }
}
