using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputMgr : BaseManager<InputMgr>
{
    private Dictionary<E_EventType, InputInfo> inputDic = new Dictionary<E_EventType, InputInfo>(); //改键信息池
    private InputInfo nowInputInfo;

    private bool isStart;
    //用于在改建时获取输入信息的委托 只有当update中获取到信息的时候 再通过委托传递给外部
    private UnityAction<InputInfo> getInputInfoCallBack;
    private bool isBeginCheckInput = false; //是否开启输入检测

    private InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(InputUpdate);
    }

    /// <summary>
    /// 开启或者关闭输入管理模块的检测
    /// </summary>
    /// <param name="isStart"></param>
    public void StartOrCloseInputMgr(bool isStart)
    {
        this.isStart = isStart;
    }

    /// <summary>
    /// 键盘改键
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
    /// 鼠标改键
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
    /// 移除指定行为的输入监听
    /// </summary>
    /// <param name="eventType"></param>
    public void RemoveInputInfo(E_EventType eventType)
    {
        if (inputDic.ContainsKey(eventType))
            inputDic.Remove(eventType);
    }
    
    /// <summary>
    /// 获取下次输入信息
    /// </summary>
    /// <param name="callBack">获取输入以后执行的回调函数</param>
    public void GetInputInfo(UnityAction<InputInfo> callBack)
    {
        getInputInfoCallBack = callBack;
        MonoMgr.Instance.StartCoroutine(BeginCheckInput());
    }

    //检测下次输入
    private IEnumerator BeginCheckInput()
    {
        //因为是下次输入，等一帧
        yield return 0;
        isBeginCheckInput = true;
    }

    private void InputUpdate()
    {
        if(isBeginCheckInput) //如果在监听下次输入
        {
            if (Input.anyKeyDown)
            {
                InputInfo inputInfo = null;
                Array keyCodes = Enum.GetValues(typeof(KeyCode));
                foreach (KeyCode inputKey in keyCodes) //找出所有收到输入的键
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
        foreach (E_EventType eventType in inputDic.Keys) //触发改键后的事件
        {
            nowInputInfo = inputDic[eventType];
            if(nowInputInfo.keyOrMouse == InputInfo.E_KeyOrMouse.Key) //键盘输入
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
            else //鼠标输入
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
