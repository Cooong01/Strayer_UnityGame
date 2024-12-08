using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����״̬���Ļ��࣬����������ı�Ҫ�ķ���
/// </summary>
public class FiniteStateMachine : MonoBehaviour
{
    private IState currentState;        // ��ǰ״̬�ӿ�
    protected Dictionary<E_PlayerStateType, IState> states = new Dictionary<E_PlayerStateType, IState>();  // ״̬�ֵ䣬�洢����״̬

    /// <summary>
    /// ״̬�����ɵ�ʱ����ã�Ҳ����˵һ����ɫ���ֵ�ʱ����á�
    /// ����д���ǿ��У�Ҳ���Ըĳ���������
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

    // ״̬ת���������ȵ��õ�ǰ״̬���˳�������Ȼ�������һ��״̬�Ľ��뷽��
    public void TransitionState(E_PlayerStateType type)
    {
        if (currentState != null)
            currentState.OnExit();

        currentState = states[type];
        currentState.OnEnter(); 
    }

    // ��ת��ɫ
    public virtual void FlipTo() { }

}
