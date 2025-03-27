using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 下落
/// </summary>
public class PlayerFallState : PlayerState
{
    public PlayerFallState(PlayerFSM manager, string animationName) : base(manager, animationName) { }

    public override void OnEnter()
    {
        base.OnEnter();
        //把现在的移动速度变成跳跃时允许的移速
        parameter.currentSpeed = parameter.jumpSpeed;
        //维持x方向速度不变
        SetVelocity(parameter.rb.velocity.x, 0);
        Debug.Log("正在下落状态");
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

