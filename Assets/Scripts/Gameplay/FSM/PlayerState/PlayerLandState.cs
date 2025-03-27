using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 落地
/// </summary>
public class PlayerLandState : PlayerState
{
    public PlayerLandState(PlayerFSM manager, string animationName) : base(manager, animationName) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("正在落地状态");
        parameter.isClickJump = false; //取消了跳跃存指令
        if (parameter.HasJumpInputBuffer || parameter.isJump)
        {
            manager.TransitionState(E_PlayerStateType.Jump);
        }
        else if (parameter.isMove)
        {
            manager.TransitionState(E_PlayerStateType.Move);
        }
        else if(parameter.isClickDash)
        {
            manager.TransitionState(E_PlayerStateType.Dash);
        }
        else if(parameter.isChangeMirrorMode)
        {
            manager.TransitionState(E_PlayerStateType.Mirror_ChangeMirrorMode);
        }
        else
        {
            manager.TransitionState(E_PlayerStateType.Idle);
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}

