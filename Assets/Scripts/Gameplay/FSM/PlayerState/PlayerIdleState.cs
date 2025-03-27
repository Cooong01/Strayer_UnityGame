using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// </summary>
public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerFSM manager, string animationName) : base(manager, animationName) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("���ڿ���״̬");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (parameter.isInteractionPlace && parameter.canEnterMirrorCloseState) //���뾵�ӽ�����Χ
        {
            manager.TransitionState(E_PlayerStateType.Mirror_CloseMirrorPlace);
        }
        if (parameter.isDash)
        {
            manager.TransitionState(E_PlayerStateType.Dash);
        }
        if (parameter.isMove)
        {
            manager.TransitionState(E_PlayerStateType.Move);
        }
        if (parameter.isJump)
        {
            parameter.currentSpeed = parameter.jumpSpeed;
            manager.TransitionState(E_PlayerStateType.Jump);
        }
        if (parameter.isChangeMirrorMode)
        {
            manager.TransitionState(E_PlayerStateType.Mirror_ChangeMirrorMode);
        }
        if (parameter.isEnteringNextLevel)
        {
            manager.TransitionState(E_PlayerStateType.enterNextLevel);
        }
        if (parameter.canTransfer)
        {
            manager.TransitionState(E_PlayerStateType.Mirror_transfer);
        }
        //����״̬����֡���ٶȣ�ֱ�����ٶȼ���0
        parameter.currentSpeed = 0;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        SetVelocity(0, parameter.rb.velocity.y);//ʵ�ּ���
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    
}

