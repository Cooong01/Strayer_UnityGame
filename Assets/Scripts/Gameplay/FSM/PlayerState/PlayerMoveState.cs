using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动
/// </summary>
public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerFSM manager, string animationName) : base(manager, animationName) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("?????????");
    }

    public override void OnUpdate()
    {

        base.OnUpdate();
        if (parameter.isDash)
        {
            manager.TransitionState(E_PlayerStateType.Dash);
        }
        if (!parameter.isMove)
        {
            manager.TransitionState(E_PlayerStateType.Idle);
        }
        if (parameter.isJump)
        {
            manager.TransitionState(E_PlayerStateType.Jump);
        }
        if (parameter.isFall)
        {
            manager.TransitionState(E_PlayerStateType.Fall);
        }
        if (parameter.isInteractionPlace && parameter.canEnterMirrorCloseState)
        {
            manager.TransitionState(E_PlayerStateType.Mirror_CloseMirrorPlace);
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
        //????????????????????????????????
        parameter.currentSpeed = Mathf.MoveTowards(parameter.currentSpeed, parameter.normalSpeed, parameter.acceration * Time.deltaTime);
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

