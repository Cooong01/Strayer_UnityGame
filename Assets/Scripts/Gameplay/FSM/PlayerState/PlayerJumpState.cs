using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerFSM manager, string animationName) : base(manager, animationName) { }

    public override void OnEnter()
    {
        base.OnEnter();
        MusicMgr.Instance.PlaySound("Whoosh Reactive Jump 003");
        //parameter.HasJumpInputBuffer = false;
        parameter.isClickJump = false;
        parameter.currentJumpNum++;
        SetVelocity(parameter.inputDirection.x * parameter.currentSpeed, parameter.jumpForce);// 设置跳跃速度  
        Debug.Log("正在跳跃状态");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (parameter.isDash)
        {
            manager.TransitionState(E_PlayerStateType.Dash);
        }
        if (parameter.isFall)
        {
            manager.TransitionState(E_PlayerStateType.Fall);
        }
        if (parameter.rb.velocity.y < 0.1f && parameter.check.isTouchGround)
        {
            manager.TransitionState(E_PlayerStateType.Land);
        }
        if (parameter.canTransfer)
        {
            manager.TransitionState(E_PlayerStateType.Mirror_transfer);
        }

        parameter.currentSpeed = Mathf.MoveTowards(parameter.currentSpeed, parameter.normalSpeed, parameter.acceration * Time.deltaTime);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        //在这里调用move，也就是在这期间可以接收移动按钮的输入并且进行移动
        Move();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
