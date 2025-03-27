using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 有限状态机基类
/// </summary>
public class FiniteStateMachine : MonoBehaviour
{
    private IState currentState; //当前状态接口
    protected Dictionary<E_PlayerStateType, IState> states = new Dictionary<E_PlayerStateType, IState>(); //状态池
    
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

    public void TransitionState(E_PlayerStateType type)
    {
        if (currentState != null)
            currentState.OnExit();

        currentState = states[type];
        currentState.OnEnter(); 
    }

    //翻转角色
    public virtual void FlipTo() { }

}
