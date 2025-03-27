using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputMgr : BaseManager<InputMgr>
{
    private Dictionary<E_EventType, InputInfo> inputDic = new Dictionary<E_EventType, InputInfo>(); //�ļ���Ϣ��
    private InputInfo nowInputInfo;

    private bool isStart;
    //�����ڸĽ�ʱ��ȡ������Ϣ��ί�� ֻ�е�update�л�ȡ����Ϣ��ʱ�� ��ͨ��ί�д��ݸ��ⲿ
    private UnityAction<InputInfo> getInputInfoCallBack;
    private bool isBeginCheckInput = false; //�Ƿ���������

    private InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(InputUpdate);
    }

    /// <summary>
    /// �������߹ر��������ģ��ļ��
    /// </summary>
    /// <param name="isStart"></param>
    public void StartOrCloseInputMgr(bool isStart)
    {
        this.isStart = isStart;
    }

    /// <summary>
    /// ���̸ļ�
    /// </summary>
    /// <param name="key"></param>
    /// <param name="inputType"></param>
    public void ChangeKeyboardInfo(E_EventType eventType, KeyCode key, InputInfo.E_InputType inputType)
    {
        if(!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType, new InputInfo(inputType, key));
        }
        else
        {
            inputDic[eventType].keyOrMouse = InputInfo.E_KeyOrMouse.Key;
            inputDic[eventType].key = key;
            inputDic[eventType].inputType = inputType;
        }
    }

    /// <summary>
    /// ���ļ�
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="mouseID"></param>
    /// <param name="inputType"></param>
    public void ChangeMouseInfo(E_EventType eventType, int mouseID, InputInfo.E_InputType inputType)
    {
        if (!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType, new InputInfo(inputType, mouseID));
        }
        else
        {
            inputDic[eventType].keyOrMouse = InputInfo.E_KeyOrMouse.Mouse;
            inputDic[eventType].mouseID = mouseID;
            inputDic[eventType].inputType = inputType;
        }
    }

    /// <summary>
    /// �Ƴ�ָ����Ϊ���������
    /// </summary>
    /// <param name="eventType"></param>
    public void RemoveInputInfo(E_EventType eventType)
    {
        if (inputDic.ContainsKey(eventType))
            inputDic.Remove(eventType);
    }
    
    /// <summary>
    /// ��ȡ�´�������Ϣ
    /// </summary>
    /// <param name="callBack">��ȡ�����Ժ�ִ�еĻص�����</param>
    public void GetInputInfo(UnityAction<InputInfo> callBack)
    {
        getInputInfoCallBack = callBack;
        MonoMgr.Instance.StartCoroutine(BeginCheckInput());
    }

    //����´�����
    private IEnumerator BeginCheckInput()
    {
        //��Ϊ���´����룬��һ֡
        yield return 0;
        isBeginCheckInput = true;
    }

    private void InputUpdate()
    {
        if(isBeginCheckInput) //����ڼ����´�����
        {
            if (Input.anyKeyDown)
            {
                InputInfo inputInfo = null;
                Array keyCodes = Enum.GetValues(typeof(KeyCode));
                foreach (KeyCode inputKey in keyCodes) //�ҳ������յ�����ļ�
                {
                    if (Input.GetKeyDown(inputKey))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down, inputKey);
                        break;
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    if (Input.GetMouseButtonDown(i))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down, i);
                        break;
                    }
                }
                getInputInfoCallBack.Invoke(inputInfo);
                getInputInfoCallBack = null;
                isBeginCheckInput = false;
            }
        }
        if (!isStart)
            return;
        foreach (E_EventType eventType in inputDic.Keys) //�����ļ�����¼�
        {
            nowInputInfo = inputDic[eventType];
            if(nowInputInfo.keyOrMouse == InputInfo.E_KeyOrMouse.Key) //��������
            {
                switch (nowInputInfo.inputType)
                {
                    case InputInfo.E_InputType.Down:
                        if (Input.GetKeyDown(nowInputInfo.key))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Up:
                        if (Input.GetKeyUp(nowInputInfo.key))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Always:
                        if (Input.GetKey(nowInputInfo.key))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    default:
                        break;
                }
            }
            else //�������
            {
                switch (nowInputInfo.inputType)
                {
                    case InputInfo.E_InputType.Down:
                        if (Input.GetMouseButtonDown(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Up:
                        if (Input.GetMouseButtonUp(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Always:
                        if (Input.GetMouseButton(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    default:
                        break;
                }
            }
        }

        EventCenter.Instance.EventTrigger(E_EventType.E_Input_Horizontal, Input.GetAxis("Horizontal"));
        EventCenter.Instance.EventTrigger(E_EventType.E_Input_Vertical, Input.GetAxis("Vertical"));
    }

}
