using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 有限状态机的基类，包含最基本的必要的方法
/// </summary>
public class FiniteStateMachine : MonoBehaviour
{
    private IState currentState;        // 当前状态接口
    protected Dictionary<E_PlayerStateType, IState> states = new Dictionary<E_PlayerStateType, IState>();  // 状态字典，存储各种状态

    /// <summary>
    /// 状态机生成的时候调用，也就是说一个角色出现的时候调用。
    /// 这里写的是空闲，也可以改成其他内容
    /// </summary>
    protected virtual void Awake()
    {
        TransitionState(E_PlayerStateType.Idle);
    }

    protected virtual void OnEnable()
    {
        currentState.OnEnter();
    }

    protected virtual void Update()
    {
        currentState.OnUpdate();
    }

    protected virtual void FixedUpdate()
    {
        currentState.OnFixedUpdate();
    }

    // 状态转换方法，先调用当前状态的退出方法，然后调用下一个状态的进入方法
    public void TransitionState(E_PlayerStateType type)
    {
        if (currentState != null)
            currentState.OnExit();

        currentState = states[type];
        currentState.OnEnter(); 
    }

    // 翻转角色
    public virtual void FlipTo() { }

}
