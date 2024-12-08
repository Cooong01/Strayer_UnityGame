using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 实现状态接口，作为玩家的状态的基类。
/// 按我的理解，基类的作用，本质上是方便状态类之间的数据沟通，以及把一些在每个状态都会有的逻辑写进去（比如动画播放逻辑）
/// 接口太抽象，需要一个基类来做这些事情。
/// 基本上，这个基类里实现动画相关的控制
/// </summary>
[Serializable]
public class PlayerState : IState
{
    protected PlayerFSM manager;
    protected PlayerParameter parameter;
    protected string animationName;
    protected AnimatorStateInfo animatorStateInfo;   // 动画状态信息

    //需求：在处于某些状态的时候，有可能动画需要根据更高优先级的动作来播放，同时不退出本状态。因此需要记录更高优先级的动画。
    private string realAnimationName; //真正的动画名字
    private bool isMove;
    private bool alreadyMove;
    private bool alreadyReal;

    //计时器，用来参与进行状态切换的逻辑判断
    float stateStartTime;
    protected float StateDuration => Time.time - stateStartTime;

    public PlayerState(PlayerFSM manager, string animationName)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
        this.animationName = animationName;
    }

    public virtual void OnEnter()
    {
        alreadyMove = false;
        Debug.Log("正在转换状态");
        stateStartTime = Time.time;
        realAnimationName = this.animationName;
        //将当前动画平滑过渡到名为animationName的动画状态，并且过渡持续时间为 0.1 秒
        parameter.animator.CrossFade(animationName, 0.1f);
    }

    public virtual void OnUpdate()
    {
        if(parameter.rb.velocity.x == 0)
        {
            isMove = false;
        }
        if (isMove && !alreadyMove)
        {
            alreadyMove = true;
            alreadyReal = false;
            parameter.animator.CrossFade("Run", 0.1f);
        }
        else if (isMove && alreadyMove)
        {
            
        }
        else if (!isMove && alreadyReal)
        {
        }
        else if (!isMove && !alreadyReal)
        {
            alreadyMove = false;
            alreadyReal = true;
            Debug.Log(realAnimationName);
            parameter.animator.CrossFade(realAnimationName, 0.1f);
        }
        //如果死亡
        if (parameter.isDead)
        {
            //失败
            EventCenter.Instance.EventTrigger(E_EventType.E_FailLevel);
            Level.Instance.DestroyLevel();
            parameter.isDead = false;
        }

        animatorStateInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);// 获取当前动画状态信息 

        if (parameter.rb.velocity.y < 0.1f && parameter.check.isTouchGround)
        {
            parameter.currentJumpNum = 0;//接触地面重置跳跃次数
        }
        //镜子的一些自动行为，在这里直接写上。

        foreach (var mirror in parameter.mirrors)
        {
            //如果镜子不被操控且不在放置状态，就跟随主角
            if (mirror.Key != parameter.nowControlMirrorNumber && !parameter.nowSetMirror.Contains(mirror.Key))
            {
                MoveMirror(mirror.Key);
            }
        }


        if (parameter.nowSetMirror.Contains(1))
        {
            parameter.mirrors[1].GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            parameter.mirrors[1].GetComponent<BoxCollider2D>().enabled = false;
        }

        if (parameter.nowSetMirror.Contains(2))
        {
            parameter.mirrors[2].GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            parameter.mirrors[2].GetComponent<BoxCollider2D>().enabled = false;
        }

        if (parameter.nowSetMirror.Contains(3))
        {
            parameter.mirrors[3].GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            parameter.mirrors[3].GetComponent<BoxCollider2D>().enabled = false;
        }


    }


    public virtual void OnFixedUpdate() 
    {

    }

    public virtual void OnExit() { }

    //动画是否播放完成
    public bool PlayComplete()
    {
        // 当前动画是否播放了95%
        if (animatorStateInfo.normalizedTime >= .95f && animatorStateInfo.IsName(animationName)) return true;
        return false;
    }

    //移动.
    public void Move()
    {
        isMove = true;
        // 根据输入方向移动角色
        SetVelocity(parameter.inputDirection.x * parameter.currentSpeed, parameter.rb.velocity.y);
        //只有移动可能翻转角色。
        //记录角色翻转前的
        manager.FlipTo();
    }

    //设置玩家速度
    public void SetVelocity(float setVelocityX, float setVelocityY)
    {
        parameter.rb.velocity = new Vector2(setVelocityX, setVelocityY);
    }
    
    //设置镜子位于角色周围固定位置.返回是否到达目标位置
    public bool MoveMirror(int mirrorNumber)
    {
        Vector3 mirrorPositionNow = parameter.mirrors[mirrorNumber].transform.position;
        parameter.mirrors[mirrorNumber].transform.position = Vector3.Lerp(mirrorPositionNow, manager.transform.TransformPoint(parameter.IdleMirrorPosition[mirrorNumber]),parameter.MirrorFollowSpeed);

        return Vector3.Distance(mirrorPositionNow, manager.transform.TransformPoint(parameter.IdleMirrorPosition[mirrorNumber])) <= 0.1;
    }

    public bool isOnTargetPostion(int mirrorNumber)
    {
        Vector3 mirrorPositionNow = parameter.mirrors[mirrorNumber].transform.position;
        return Vector3.Distance(mirrorPositionNow, manager.transform.TransformPoint(parameter.IdleMirrorPosition[mirrorNumber])) <= 0.1;
    }

}

