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
        if (parameter.HP <= 0) //血量到0直接死了
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
        //默认进入空闲状态
        manager.TransitionState(E_PlayerStateType.Idle);

    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        //本状态不允许移动，因为受击会有硬直。但是点击其他操作会取消硬直

    }

    public override void OnExit()
    {
        base.OnExit();
    }

}
