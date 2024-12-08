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
        //���ó�����ȴʱ��
        manager.OnStartCoroutine(DashWaitTime());
        Debug.Log("���ڳ��״̬");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        //����Ŀǰ�ĳ�����
        SetVelocity(parameter.dashPostion.x * parameter.facingDirection, parameter.dashPostion.y);

    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();


        ////����Ӱ��
        //ObjectPool.GetObject(parameter.shadowPrefab);
        //�˴�û���л��߼�����Ϊ����������������������������Զ��������״̬
    }

    public override void OnExit()
    {
        base.OnExit();
        parameter.isDashing = false;
        parameter.isClickDash = false; //ȡ����̴�ָ��
    }

    IEnumerator DashExitTime()
    {
        yield return new WaitForSeconds(parameter.dashTime); // �ȴ�

        parameter.isClickDash = false; //ȡ����̴�ָ��
        manager.TransitionState(E_PlayerStateType.Idle);
    }

    IEnumerator DashWaitTime()
    {
        yield return new WaitForSeconds(parameter.dashCD); // �ȴ�

        parameter.isClickDash = false; //ȡ����̴�ָ��
        parameter.isWaitDash = false;
    }
}

