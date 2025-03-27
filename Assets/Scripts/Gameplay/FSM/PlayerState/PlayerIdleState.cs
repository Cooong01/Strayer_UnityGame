using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 待机
/// </summary>
public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerFSM manager, string animationName) : base(manager, animationName) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("正在空闲状态");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (parameter.isInteractionPlace && parameter.canEnterMirrorCloseState) //进入镜子交互范围
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
        //空闲状态，逐帧设速度，直到把速度减到0
        parameter.currentSpeed = 0;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        SetVelocity(0, parameter.rb.velocity.y);//实现减速
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    
}

