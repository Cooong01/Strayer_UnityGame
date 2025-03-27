using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����״̬������
/// </summary>
public class FiniteStateMachine : MonoBehaviour
{
    private IState currentState; //��ǰ״̬�ӿ�
    protected Dictionary<E_PlayerStateType, IState> states = new Dictionary<E_PlayerStateType, IState>(); //״̬��
    
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

    //��ת��ɫ
    public virtual void FlipTo() { }

}
