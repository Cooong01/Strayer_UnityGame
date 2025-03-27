using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//�¼����࣬�����¼���ͬʱװ�вκ��޲�ί��
public abstract class EventInfoBase{ }

//�в�ί��
public class EventInfo<T>:EventInfoBase
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

//�޲�ί��
public class EventInfo: EventInfoBase
{
    public UnityAction actions;
     
    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}


/// <summary>
/// �¼�����ģ�顣
/// ʹ�����̣��� E_EventType ����д�¼�����ö�٣�Ȼ����ô����ͼ�����������Ӽ�����ʱ���Զ�ע���¼���
/// </summary>
public class EventCenter: BaseManager<EventCenter>
{
    private Dictionary<E_EventType, EventInfoBase> eventDic = new Dictionary<E_EventType, EventInfoBase>(); //�¼���
    private EventCenter() { }

    /// <summary>
    /// �����в��¼��������Ҫ�����������ϲ�������Ԫ����ʽ����
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    public void EventTrigger<T>(E_EventType eventName, T info)
    {
        if(info.GetType().Name != "Vector3")
        {
            Debug.Log("�в��¼�����" + eventName + "��������Ϣ��" + info +"����Ϊ" + info.GetType().Name);
        }
        if(eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions?.Invoke(info);
        }
    }

    /// <summary>
    /// �����в��¼�
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    public void EventTrigger(E_EventType eventName)
    {
        Debug.Log("�޲��¼�����" + eventName + "��������");
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo).actions?.Invoke();
        }
    }


    /// <summary>
    /// ����в��¼���������Ҫ�������ϲ���ʱ��ͨ��Ԫ�����Ӳ�����
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    /// <param name="func">�ص�����</param>
    public void AddEventListener<T>(E_EventType eventName, UnityAction<T> func)
    {
        Debug.Log("�¼�����" + eventName + "������вμ�����������������"+func.Method.Name);
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
    /// ����޲��¼�������
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    /// <param name="func">�ص�����</param>
    public void AddEventListener(E_EventType eventName, UnityAction func)
    {
        Debug.Log("�¼�����" + eventName + "������޲μ�����������������" + func.Method.Name);
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
    /// �Ƴ��в��¼�����
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    /// <param name="func">�ص�����</param>
    public void RemoveEventListener<T>(E_EventType eventName, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo<T>).actions -= func;
    }
    
    /// <summary>
    /// �Ƴ��޲��¼�����
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    /// <param name="func">�ص�����</param>
    public void RemoveEventListener(E_EventType eventName, UnityAction func)
    {
        if (eventDic.ContainsKey(eventName))
            (eventDic[eventName] as EventInfo).actions -= func;
    }

    /// <summary>
    /// ��������¼�
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }

    /// <summary>
    /// ���ָ���¼�����
    /// </summary>
    /// <param name="eventName">�¼�����</param>
    public void Clear(E_EventType eventName)
    {
        if (eventDic.ContainsKey(eventName))
            eventDic.Remove(eventName);
    }
}
