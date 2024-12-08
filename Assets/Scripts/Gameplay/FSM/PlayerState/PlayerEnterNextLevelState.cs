using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerEnterNextLevelState : PlayerState
{
    public PlayerEnterNextLevelState(PlayerFSM manager, string animationName) : base(manager, animationName) { }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("����ת�ƹؿ���״̬");
        //ת�ƹؿ�ʱ����ɫҪ�������飺1.���վ��� 2.��ɫ����һ���ƶ�,�ڼ������� 3.��⵽����һ���ؿ�����ʼ�㣬�������״̬
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
