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
        SetVelocity(parameter.inputDirection.x * parameter.currentSpeed, parameter.jumpForce);// ������Ծ�ٶ�  
        Debug.Log("������Ծ״̬");
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
        //���������move��Ҳ���������ڼ���Խ����ƶ���ť�����벢�ҽ����ƶ�
        Move();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
