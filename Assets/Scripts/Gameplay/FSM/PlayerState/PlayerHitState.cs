using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitState : PlayerState
{
    public PlayerHitState(PlayerFSM manager, string animationName) : base(manager, animationName) { }
    public override void OnEnter()
    {
        base.OnEnter();
        parameter.HP--;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (parameter.HP <= 0) //Ѫ����0ֱ������
        {
            manager.TransitionState(E_PlayerStateType.Death);
        }
        if (parameter.isDash)
        {
            manager.TransitionState(E_PlayerStateType.Dash);
        }
        if (parameter.check.isTouchGround)
        {
            manager.TransitionState(E_PlayerStateType.Land);
        }
        if (parameter.isJump)
        {
            manager.TransitionState(E_PlayerStateType.Jump);
        }
        //Ĭ�Ͻ������״̬
        manager.TransitionState(E_PlayerStateType.Idle);

    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        //��״̬�������ƶ�����Ϊ�ܻ�����Ӳֱ�����ǵ������������ȡ��Ӳֱ

    }

    public override void OnExit()
    {
        base.OnExit();
    }

}
