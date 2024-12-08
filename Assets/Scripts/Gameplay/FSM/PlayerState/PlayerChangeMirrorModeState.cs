using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChangeMirrorModeState : PlayerState
{

    public PlayerChangeMirrorModeState(PlayerFSM manager, string animationName) : base(manager, animationName){}

    public int step; //第一步：回收镜子且旋转，第二步：镜子旋转，且一边缩小；第三步：新的镜子一边放大一边旋转；第三步结束，退出本状态。每一步都与前一步耦合，所以写的耦合在一起。
    public float nowTime;//第三步里用来计时
    public bool step3;
    public override void OnEnter()
    {
        base.OnEnter();
        parameter.MirrorControlMode = !parameter.MirrorControlMode;
        step = 1;
        parameter.nowSetMirror.Clear(); //清空设置镜子列表，在场上的镜子会自动回到角色身边;
        parameter.nowOrientation = 0;
        EventCenter.Instance.EventTrigger(E_EventType.E_OutMirrorRecycle, 1);
        EventCenter.Instance.EventTrigger(E_EventType.E_OutMirrorRecycle, 2);
        EventCenter.Instance.EventTrigger(E_EventType.E_OutMirrorRecycle, 3);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        switch (step)
        {
            case 1:
                foreach (var mirror in parameter.mirrors)
                {
                    //回收镜子且旋转
                    if (!parameter.nowSetMirror.Contains(mirror.Key))
                    {
                        if (!MoveMirror(mirror.Key)) //一边执行一边判断
                        {
                            //旋转
                            mirror.Value.transform.Rotate(0, 30, 0);
                        }
                        else
                        {
                            step = 2;
                            manager.StartCoroutine(DurationStep2());
                        }
                    }
                }
                break;
            case 2:
                foreach (var mirror in parameter.mirrors)
                {
                    //回收镜子且旋转
                    if (!parameter.nowSetMirror.Contains(mirror.Key))
                    {
                        if (!MoveMirror(mirror.Key)) //一边执行一边判断
                        {
                            //旋转
                            mirror.Value.transform.Rotate(0, 3, 0);
                        }
                        else
                        {
                            step = 2;
                            manager.StartCoroutine(DurationStep2());
                        }
                    }

                    //一边旋转一边缩小
                    if (!parameter.nowSetMirror.Contains(mirror.Key))
                    {
                        //旋转
                        mirror.Value.transform.Rotate(0, 3, 0);
                        //缩小
                        mirror.Value.transform.localScale = mirror.Value.transform.localScale * (1 - parameter.reduceSpeed * Time.deltaTime);
                    }
                }
                if (step3)
                {
                    changeMirrorActive();
                    manager.StartCoroutine(DurationStep3());
                    step = 3;
                    nowTime = 0;
                }
                break;
            case 3:

                foreach (var mirror in parameter.mirrors)
                {
                    //旋转
                    mirror.Value.transform.Rotate(0, 3, 0);
                    //放大
                    nowTime += Time.deltaTime;
                    float scaleFactor = Mathf.Lerp(parameter.minScale, 1f, nowTime / parameter.reduceDuration);
                    switch (mirror.Value.name)
                    {
                        case "Mirror1":
                            mirror.Value.transform.localScale = parameter.mirror1OriginalScale * scaleFactor;
                            break;
                        case "Mirror2":
                            mirror.Value.transform.localScale = parameter.mirror2OriginalScale * scaleFactor;
                            break;
                        case "Mirror3":
                            mirror.Value.transform.localScale = parameter.mirror3OriginalScale * scaleFactor;
                            break;
                    }

                }
                break;
            case 4:
                //时间结束以后让镜子变为正常形态
                foreach (var mirror in parameter.mirrors)
                {
                    //旋转
                    mirror.Value.transform.rotation = new Quaternion(0, 0, 0,0);
                    switch (mirror.Value.name)
                    {
                        case "Mirror1":
                            mirror.Value.transform.localScale = parameter.mirror1OriginalScale;
                            break;
                        case "Mirror2":
                            mirror.Value.transform.localScale = parameter.mirror2OriginalScale;
                            break;
                        case "Mirror3":
                            mirror.Value.transform.localScale = parameter.mirror3OriginalScale;
                            break;
                    }
                }
                manager.TransitionState(E_PlayerStateType.Idle);
                break;
        }
    }

    public override void OnFixedUpdate()
    {
        //本状态不执行base的镜子操作，因为要重新定制。

        //该状态可移动
        Move();
    }

    public override void OnExit()
    {
        base.OnExit();
        parameter.isChangeMirrorMode = false;
        EventCenter.Instance.EventTrigger(E_EventType.E_OutMirrorModeChange);
    }

    //切换镜子的失活情况
    public void changeMirrorActive()
    {
        if (parameter.MirrorControlMode)
        {
            foreach (var mirror in parameter.mirrors)
            {
                if (mirror.Value.name == "Mirror1")
                {
                    mirror.Value.gameObject.SetActive(false);
                }
                if (mirror.Value.name == "Mirror2")
                {
                    mirror.Value.gameObject.SetActive(false);
                }
                if (mirror.Value.name == "Mirror3")
                {
                    mirror.Value.gameObject.SetActive(true);
                    mirror.Value.transform.localScale = parameter.mirror3OriginalScale * parameter.minScale;
                }
            }
        }
        else
        {
            foreach (var mirror in parameter.mirrors)
            {
                if (mirror.Value.name == "Mirror1")
                {
                    mirror.Value.gameObject.SetActive(true);
                    mirror.Value.transform.localScale = parameter.mirror1OriginalScale * parameter.minScale;
                }
                if (mirror.Value.name == "Mirror2")
                {
                    mirror.Value.gameObject.SetActive(true);
                    mirror.Value.transform.localScale = parameter.mirror2OriginalScale * parameter.minScale;
                }
                if (mirror.Value.name == "Mirror3")
                {
                    mirror.Value.gameObject.SetActive(false);
                }
            }
        }
    }

    //第二阶段持续的时间
    IEnumerator DurationStep2()
    {
        yield return new WaitForSeconds(parameter.reduceDuration); // 等待
        step3 = true; //结束以后切换到第三阶段
    }

    IEnumerator DurationStep3()
    {
        yield return new WaitForSeconds(parameter.reduceDuration); // 等待
        step = 4; //结束以后切换到第四阶段
    }
}
