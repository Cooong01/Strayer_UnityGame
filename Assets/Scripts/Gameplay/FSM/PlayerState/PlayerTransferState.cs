using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色传送
/// </summary>
public class PlayerTransferState : PlayerState
{
    //传送分两步：首先角色位置移动到最近的镜子，然后角色位置瞬间设置为另一个镜子,然后退出状态
    public PlayerTransferState(PlayerFSM manager, string animationName) : base(manager, animationName){}

    private bool isClosing; //正在接近最近的镜子

    private bool isEnd;

    private float nowTime; //飞向镜子时的当前时间

    

    public override void OnEnter()
    {
        //进入传送状态
        Debug.Log("正在传送状态");
        SetVelocity(0, 0); //速度归零
        //状态值清零
        isClosing = true;
        isEnd = false;
        nowTime = 0;
        MusicMgr.Instance.PlaySound("transfer");
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (isEnd)
        {
            manager.TransitionState(E_PlayerStateType.Idle);
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        if (isClosing)
        {
            Transform();
        }
        else
        {
            isEnd = true;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        parameter.check.shouldDetectAway = true;
        parameter.check.shouldDetectionMirrorObj = parameter.check.touchedMirrorObj;
    }

    //移动到镜子
    private void Transform()
    {
        if (Vector3.Distance(manager.transform.position, parameter.check.touchedMirrorObj.transform.position) <= 0.1)
        {
            //直接传送到另一个镜子的位置
            manager.transform.position = FindTheOtherMirror(parameter.check.touchedMirrorObj).transform.position;
            isClosing = false; //传送完了就退出本状态;
        }
        else
        {
            ////todo:需要优化。
            //manager.transform.position = Vector3.Lerp(manager.transform.position,parameter.check.touchedMirrorObj.transform.position,parameter.flyingSpeed); 
            manager.transform.position = parameter.check.touchedMirrorObj.transform.position;

        }
    }

    private GameObject FindTheOtherMirror(GameObject mirror)
    {
        if (mirror.name == "Mirror1")
        {
            Debug.Log(parameter.mirrors[2].name);
            return parameter.mirrors[2];
        }
        return parameter.mirrors[1];
    }

}

