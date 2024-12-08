using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerEnterNextLevelState : PlayerState
{
    public PlayerEnterNextLevelState(PlayerFSM manager, string animationName) : base(manager, animationName) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("正在转移关卡的状态");
        //转移关卡时，角色要做的事情：1.回收镜子 2.角色往下一关移动,期间禁用输出 3.检测到达下一个关卡的起始点，进入空闲状态
        for (int i=1;i<=3;i++)
        {
            parameter.nowSetMirror.Remove(i);
            Vector3 euler = parameter.mirrors[i].transform.eulerAngles;
            euler.z = 0;
            parameter.mirrors[i].transform.eulerAngles = euler;
        }
        UIMgr.Instance.HidePanel<PanelPlay>(true);
        parameter.HP = parameter.HPMax;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        //
        if (!parameter.isEnteringNextLevel)
        {
            manager.TransitionState(E_PlayerStateType.Idle);
        }
        parameter.inputDirection = Vector2.right;
        parameter.currentSpeed = parameter.normalSpeed;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        Move();
    }

    public override void OnExit()
    {
        base.OnExit();
        SetVelocity(0, 0);
        UIMgr.Instance.ShowPanel<PanelPlay>();

    }
}
