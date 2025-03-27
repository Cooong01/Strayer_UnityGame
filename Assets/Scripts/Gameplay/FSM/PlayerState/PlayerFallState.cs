using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// </summary>
public class PlayerFallState : PlayerState
{
    public PlayerFallState(PlayerFSM manager, string animationName) : base(manager, animationName) { }

    public override void OnEnter()
    {
        base.OnEnter();
        //�����ڵ��ƶ��ٶȱ����Ծʱ���������
        parameter.currentSpeed = parameter.jumpSpeed;
        //ά��x�����ٶȲ���
        SetVelocity(parameter.rb.velocity.x, 0);
        Debug.Log("��������״̬");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
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
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        Move();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

