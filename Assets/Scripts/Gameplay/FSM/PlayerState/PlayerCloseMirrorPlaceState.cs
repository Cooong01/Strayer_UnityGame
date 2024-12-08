using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCloseMirrorPlaceState : PlayerState
{
    public PlayerCloseMirrorPlaceState(PlayerFSM manager, string animationName) : base(manager, animationName) { }
    private bool enterControl = false;

    public override void OnEnter()
    {
        base.OnEnter();
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_ControlMirror,EnterControl);

        Debug.Log("����ӽ����ӷ�����ʩ��״̬");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        //֮���԰Ѵ�����ťд�����������Ϊ�п���ĳ���������ڽ���close����״̬֮����ҲŻ��գ�����ʱ�����һֱ��⡣
        //todo:д������ʾx�ž��ӡ���ǣ�������һֱ�����¼�����ϵͳ��
        if (parameter.MirrorControlMode)
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_CloseMirrorPlace, 3);
        }
        else
        {
            if (!parameter.nowSetMirror.Contains(1))
            {
                EventCenter.Instance.EventTrigger(E_EventType.E_CloseMirrorPlace, 1);
            }
            if (!parameter.nowSetMirror.Contains(2))
            {
                EventCenter.Instance.EventTrigger(E_EventType.E_CloseMirrorPlace, 2);
            }
        }
        //��״̬��״̬ת�������Idle״̬���ƣ������ƶ���Ĭ�Ͽ���
        if (parameter.isDash)
        {
            manager.TransitionState(E_PlayerStateType.Dash);
        }
        if (parameter.isChangeMirrorMode)
        {
            manager.TransitionState(E_PlayerStateType.Mirror_ChangeMirrorMode);
        }

        if (!parameter.isInteractionPlace) //����뿪������ʩ�Ŀɽ�����Χ
        {
            if (parameter.isMove)
            {
                manager.TransitionState(E_PlayerStateType.Move);
            }
            else
            {
                manager.TransitionState(E_PlayerStateType.Idle);
            }
        }
        if (parameter.isJump) //�����Ծ
        {
            manager.TransitionState(E_PlayerStateType.Jump);
        }
        if (enterControl)
        {
            enterControl = false;
            manager.TransitionState(E_PlayerStateType.Mirror_ControlMirror);
        }
        //�����ڵ��ٶ��𽥱��Ĭ�����٣�ģ�¼��ٵĹ���
        parameter.currentSpeed = Mathf.MoveTowards(parameter.currentSpeed, parameter.normalSpeed, parameter.acceration * Time.deltaTime);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        //��״̬���ƶ�
        Move();

    }

    public override void OnExit()
    {
        base.OnExit();
        EventCenter.Instance.Claer(E_EventType.E_ControlMirror);
        EventCenter.Instance.EventTrigger(E_EventType.E_LeaveMirrorPlace);
    }

    private void EnterControl(int mirrorNumber)
    {
        parameter.nowControlMirrorNumber = mirrorNumber;
        enterControl = true;
    }
}
