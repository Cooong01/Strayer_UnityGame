using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(PlayerFSM manager, string animationName) : base(manager, animationName) { }

    public override void OnEnter()
    {
        base.OnEnter();
        MusicMgr.Instance.PlaySound("rush");
        parameter.isClickDash = false;
        parameter.isDashing = true;
        parameter.isWaitDash = true;
        manager.OnStartCoroutine(DashExitTime());
        //设置冲锋的冷却时间
        manager.OnStartCoroutine(DashWaitTime());
        Debug.Log("正在冲锋状态");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        //向着目前的朝向冲锋
        SetVelocity(parameter.dashPostion.x * parameter.facingDirection, parameter.dashPostion.y);

    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();


        ////生成影子
        //ObjectPool.GetObject(parameter.shadowPrefab);
        //此处没有切换逻辑，因为冲锋过程中无视其他操作，冲锋完自动进入空闲状态
    }

    public override void OnExit()
    {
        base.OnExit();
        parameter.isDashing = false;
        parameter.isClickDash = false; //取消冲刺存指令
    }

    IEnumerator DashExitTime()
    {
        yield return new WaitForSeconds(parameter.dashTime); // 等待

        parameter.isClickDash = false; //取消冲刺存指令
        manager.TransitionState(E_PlayerStateType.Idle);
    }

    IEnumerator DashWaitTime()
    {
        yield return new WaitForSeconds(parameter.dashCD); // 等待

        parameter.isClickDash = false; //取消冲刺存指令
        parameter.isWaitDash = false;
    }
}

