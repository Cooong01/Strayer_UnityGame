using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerControlMirrorPostionState : PlayerState
{
    public PlayerControlMirrorPostionState(PlayerFSM manager, string animationName) : base(manager, animationName) { }


    public Transform anchor1; //目标物体的第一个锚点
    public Transform anchor2; //目标物体的第二个锚点
    public Vector3 anchor1Position;//目标物体的第一个锚点位置
    public Vector3 anchor2Position;//目标物体的第二个锚点位置
    public Vector3 nowMirrorPosition; //目前控制的镜子的位置
    public bool isOnFacility = false; //正在控制的镜子正在设施上
    public bool isSet = false;
    public float startDistance = 0; //用来计算镜子飞行过程中的旋转角度插值
    public bool added = false;
    public bool orientated = true;
    private Vector3 lastPosition; 
    private Vector3 targetPosition; //飞向镜子设施时的


    public override void OnEnter()
    {
        base.OnEnter();
        EnterMirrorPlaceMode(parameter.nowControlMirrorNumber);
        isOnFacility = false;
        Debug.Log("正在操控镜子状态");
        startDistance = Vector3.Distance(anchor1Position, parameter.mirrors[parameter.nowControlMirrorNumber].transform.position);
        added = false;
        isSet = false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        //如果离开了可交互距离，自动取消操控，进入休闲状态。如果主动取消镜子交互，也取消操控，进入休闲状态。
        if (!parameter.isInteractionPlace)
        {
            EventCenter.Instance.EventTrigger(E_EventType.E_LeaveMirrorPlace); //触发离开交互状态的事件
            //
            manager.TransitionState(E_PlayerStateType.Idle);
        }
        if (parameter.isDash)
        {
            manager.TransitionState(E_PlayerStateType.Dash);
        }
        if (parameter.isJump)
        {
            manager.TransitionState(E_PlayerStateType.Jump);
        }
        if (parameter.isChangeMirrorMode)
        {
            manager.TransitionState(E_PlayerStateType.Mirror_ChangeMirrorMode);
        }
        //把现在的速度逐渐变成默认移速，模仿加速的过程
        parameter.currentSpeed = Mathf.MoveTowards(parameter.currentSpeed, parameter.normalSpeed, parameter.acceration * Time.deltaTime);

        if (parameter.isControlling) //镜子在控制模式上，就进入调整镜子的模式
        {
            ControlMirrorPosition();
        }

    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (!isOnFacility && Vector3.Distance(targetPosition, parameter.mirrors[parameter.nowControlMirrorNumber].transform.position) <= 0.1)
        {
            parameter.mirrors[parameter.nowControlMirrorNumber].transform.position = targetPosition;
            parameter.isControlling = true;
            isOnFacility = true;
            //注册监听调整镜子角度和放置镜子的事件
            orientated = false;
            EventCenter.Instance.AddEventListener(E_EventType.E_PressMirrorOrientationButton, ChangeMirrorOrientation);
            EventCenter.Instance.AddEventListener(E_EventType.E_PressMirrorSetButton, SetMirror);
            Vector3 eulerAngle = parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles;
            eulerAngle.z = parameter.nowOrientation;
            parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles = eulerAngle;
            
        }
        if (!isOnFacility) //如果不在设施上，就飞向设施
        {
            //每帧移动镜子到设施的第一个锚点上
            parameter.mirrors[parameter.nowControlMirrorNumber].transform.position = Vector3.Lerp(parameter.mirrors[parameter.nowControlMirrorNumber].transform.position, targetPosition, parameter.MirrorFindTargetSpeed);

            Vector3 eulerAngle = parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles;
            eulerAngle.z = Mathf.LerpAngle(eulerAngle.z,parameter.nowOrientation,Vector3.Distance(targetPosition, parameter.mirrors[parameter.nowControlMirrorNumber].transform.position) /startDistance);
            parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles = eulerAngle;
        }

        //此状态允许移动
        Move();
    }

    public override void OnExit()
    {
        base.OnExit();
        //todo:退出的时候，清空对应事件的监听,把没放下的镜子收回
        if (!isSet)
        {
            Vector3 euler = parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles;
            euler.z = 0;
            parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles = euler;
        }


        parameter.isControlling = false;
        parameter.nowControlMirrorNumber = 0;

        EventCenter.Instance.EventTrigger(E_EventType.E_LeaveMirrorPlace);
        EventCenter.Instance.Claer(E_EventType.E_PressMirrorSetButton);
        EventCenter.Instance.Claer(E_EventType.E_PressMirrorOrientationButton);

    }

    public void ControlMirrorPosition()
    {

        float distance1 = Vector3.Distance(parameter.pointPosition, anchor1Position);
        float distance2 = Vector3.Distance(parameter.pointPosition, anchor2Position);
        float distanceAnchors = Vector3.Distance((Vector2)anchor1Position, (Vector2)anchor2Position);
        //每一帧根据玩家点击的位置设置镜子的位置到玩家点击的投影位置
        //如果点击距离在设施一定范围内，才移动镜子，否则不移动

        if (Projection.DistanceFromPoint2Line(parameter.pointPosition, anchor1Position, anchor2Position) <= 4)
        {

            Vector2 projectionPosition = Projection.ProjectPointOntoLine(parameter.pointPosition, anchor1Position, anchor2Position);
            Debug.Log(parameter.pointPosition.y);
            //如果改变之后的位置的碰撞体碰到墙了，就不允许改变，返回上一个位置。

            parameter.mirrors[parameter.nowControlMirrorNumber].transform.position = new Vector3(projectionPosition.x, projectionPosition.y, parameter.mirrors[parameter.nowControlMirrorNumber].transform.position.z);
            //todo优化:方案1：加一个isControlling标记，如果镜子目前正在墙里，在该标记为false的时候，镜子飞向最近的非墙位置。
            //方案2：:加一个“canSet”标记，当镜子处于非墙位置的时候，才允许set。
        }
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Wall", "Ground"));
        Collider2D[] result = new Collider2D[10];
        Debug.Log(parameter.mirrors[parameter.nowControlMirrorNumber].transform.Find("detection").GetComponent<CircleCollider2D>().OverlapCollider(contactFilter, result));
        if (parameter.nowControlMirrorNumber == 1 || parameter.nowControlMirrorNumber ==2)
        {
            if (parameter.mirrors[parameter.nowControlMirrorNumber].transform.Find("detection").GetComponent<CircleCollider2D>().OverlapCollider(contactFilter, result) > 0)
            {
                EventCenter.Instance.EventTrigger(E_EventType.E_LeaveMirrorPlace);
            }
            else
            {
                EventCenter.Instance.EventTrigger(E_EventType.E_EnterMirrorInteraction);
            }
        }
        else
        {

            EventCenter.Instance.EventTrigger(E_EventType.E_EnterMirrorInteraction);
        }

    }

    //进入镜子模式
    public void EnterMirrorPlaceMode(int mirrorNumber)
    {
        //要让镜子自动飞向设施的默认位置
        //首先找到目标设施，然后

        anchor1 = parameter.check.touchedMirrorPlaceObj.transform.GetComponent<ItemRectangle>().anchor1.transform;
        anchor2 = parameter.check.touchedMirrorPlaceObj.transform.GetComponent<ItemRectangle>().anchor2.transform;

        //获得两个锚点的世界坐标
        anchor1Position = anchor1.position;
        anchor2Position = anchor2.position;

        targetPosition = (anchor1Position + anchor2Position) / 2;

        nowMirrorPosition = parameter.mirrors[mirrorNumber].transform.position;
        parameter.mirrors[mirrorNumber].transform.position = Vector3.Lerp(nowMirrorPosition, targetPosition, parameter.MirrorFindTargetSpeed);
    }

    //改变镜子角度
    public void ChangeMirrorOrientation() 
    {
        MusicMgr.Instance.PlaySound("Heavy Object Move");
        orientated = true;
        int orientation = parameter.orientationList[ ++parameter.currentOrientationIndex % parameter.orientationList.Count];
        Debug.Log(orientation);
        Vector3 eulerAngle = parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles;
        parameter.nowOrientation = orientation;
        eulerAngle.z = parameter.nowOrientation;
        parameter.mirrors[parameter.nowControlMirrorNumber].transform.eulerAngles = eulerAngle;


        foreach (var mirror in parameter.nowSetMirror)
        {
            eulerAngle = parameter.mirrors[mirror].transform.eulerAngles;
            eulerAngle.z = parameter.nowOrientation;
            parameter.mirrors[mirror].transform.eulerAngles = eulerAngle;
        }
    }

    //放下镜子,退出本状态
    public void SetMirror()
    {
        MusicMgr.Instance.PlaySound("Whoosh Blade 003");
        if (!added)
        {
            MusicMgr.Instance.PlaySound("setMirror");
            isSet = true;
            added = true;
            parameter.nowSetMirror.Add(parameter.nowControlMirrorNumber);
            //只要放下了镜子，就通知回收按钮出现
            EventCenter.Instance.EventTrigger(E_EventType.E_EnterMirrorRecycle, parameter.nowControlMirrorNumber);
            manager.TransitionState(E_PlayerStateType.Idle);
        }
    }
}
