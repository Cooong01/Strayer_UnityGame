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

        Debug.Log("进入接近镜子放置设施的状态");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        //之所以把触发按钮写到更新里，是因为有可能某个镜子是在进入close镜子状态之后，玩家才回收，而此时必须就一直检测。
        //todo:写“已显示x号镜子”标记，让它不一直触发事件中心系统。
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
        //本状态的状态转换情况与Idle状态类似，除了移动的默认可行
        if (parameter.isDash)
        {
            manager.TransitionState(E_PlayerStateType.Dash);
        }
        if (parameter.isChangeMirrorMode)
        {
            manager.TransitionState(E_PlayerStateType.Mirror_ChangeMirrorMode);
        }

        if (!parameter.isInteractionPlace) //如果离开镜子设施的可交互范围
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
        if (parameter.isJump) //如果跳跃
        {
            manager.TransitionState(E_PlayerStateType.Jump);
        }
        if (enterControl)
        {
            enterControl = false;
            manager.TransitionState(E_PlayerStateType.Mirror_ControlMirror);
        }
        //把现在的速度逐渐变成默认移速，模仿加速的过程
        parameter.currentSpeed = Mathf.MoveTowards(parameter.currentSpeed, parameter.normalSpeed, parameter.acceration * Time.deltaTime);
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        //该状态可移动
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
