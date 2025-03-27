using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡
/// 没有死亡动画，因此死亡直接切到失败界面。
/// </summary>
public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(PlayerFSM manager, string animationName) : base(manager, animationName) { }
    public override void OnEnter()
    {
        base.OnEnter();
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
