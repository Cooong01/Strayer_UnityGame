using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����
/// û�������������������ֱ���е�ʧ�ܽ��档
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
