using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.U2D;

/// <summary>
/// 继承了玩家状态机的控制类。
/// 在这里写的是各种操控的回调函数。
/// 通过接收玩家输入，在这里来改变对应参数的值。
/// </summary>
public class PlayerController : PlayerFSM
{

    protected override void Awake()
    {
        base.Awake();
        // 注册各种输入事件对应的监听回调函数
        EventCenter.Instance.AddEventListener<Vector3>(E_EventType.E_PressMoveButton, Move);
        EventCenter.Instance.AddEventListener(E_EventType.E_UpMoveButton, StopMove);
        EventCenter.Instance.AddEventListener(E_EventType.E_PressJumpButton, Jump);
        EventCenter.Instance.AddEventListener(E_EventType.E_PressDashButton, Dash);
        EventCenter.Instance.AddEventListener(E_EventType.E_PressChangeMirrorModeButton, TransformMirrorMode);
        EventCenter.Instance.AddEventListener(E_EventType.E_PressChangePlayerModeButton, TransformPlayerMode);
        EventCenter.Instance.AddEventListener<int>(E_EventType.E_PressMirrorRecycleButton, RecycleMirror);
        EventCenter.Instance.AddEventListener(E_EventType.E_PlayerAttacked, Dead);

        InitSpriteShape();
        //todo：传送事件
        //EventCenter.Instance.AddEventListener();
        //EventCenter.Instance.AddEventListener<int>(E_EventType.E_DragJoyStick, ControlMirror);
        //EventCenter.Instance.AddEventListener(E_EventType.E_PressChangeMirrorModeButton, TransformMirrorMode);
        //EventCenter.Instance.AddEventListener(E_EventType.E_PressChangePlayerModeButton, TransformPlayerMode);

    }

    protected override void Update()
    {
        base.Update();

        // 检查设备类型
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            // 处理移动设备的触摸输入
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == UnityEngine.TouchPhase.Began ||
                    touch.phase == UnityEngine.TouchPhase.Moved ||
                    touch.phase == UnityEngine.TouchPhase.Stationary)
                {
                    // 获取触摸的屏幕坐标
                    Vector3 screenPosition = touch.position;
                    // 将屏幕坐标转换为世界坐标
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -1));
                    parameter.pointPosition = new Vector2(worldPosition.x, worldPosition.y);
                }
            }
        }
        else if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            // 处理桌面设备的鼠标输入
            if (Input.GetMouseButton(0))
            {
                // 获取鼠标点击的屏幕坐标
                Vector3 screenPosition = Input.mousePosition;
                // 将屏幕坐标转换为世界坐标
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -1));
                parameter.pointPosition = new Vector2(worldPosition.x, worldPosition.y);
            }
        }


        if (parameter.canTransfer) //如果处于可传送模式
        {
            UpdateSpriteShape();
        }
        else
        {
            parameter.check.spriteShapeController.gameObject.SetActive(false);
        }
        

    }


    //移动
    public void Move(Vector3 context)
    {
        if (context.x >0)
        {
            parameter.inputDirection = Vector2.right;
        }
        else if (context.x < 0)
        {
            parameter.inputDirection = Vector2.left;
        }
    }

    //停止移动
    public void StopMove()
    {
        parameter.inputDirection = Vector2.zero;
    }

    // 跳跃
    public void Jump()
    {
        parameter.isClickJump = true;
        parameter.isClickStopJump = false;
        ////预输入计时
        // 根据策划设计，预输入已废弃
        //StopCoroutine(PreInputExitTime());
        //StartCoroutine(PreInputExitTime());
    }

    IEnumerator PreInputExitTime()
    {
        parameter.HasJumpInputBuffer = true;
        yield return new WaitForSeconds(parameter.waitJumpInputBufferTime); // 等待
        parameter.HasJumpInputBuffer = false;
    }

    private void Dash()
    {
        parameter.isClickDash = true;
    }

    //切换玩家模式
    private void TransformPlayerMode()
    {
        parameter.isChangePlayerMode = true;
    }

    //切换镜子模式
    private void TransformMirrorMode()
    {
        parameter.isChangeMirrorMode = true;
    }

    //回收镜子
    private void RecycleMirror(int mirrorNumber)
    {
        MusicMgr.Instance.PlaySound("recycle");
        parameter.nowSetMirror.Remove(mirrorNumber);
        Vector3 euler = parameter.mirrors[mirrorNumber].transform.eulerAngles;
        euler.z = 0;
        parameter.mirrors[mirrorNumber].transform.eulerAngles = euler;
        //回收完检查当前场上剩余镜子数量，为0就重置镜子角度
        if(parameter.nowSetMirror.Count == 0)
        {
            parameter.nowOrientation = parameter.orientationList[0];
        }
    }

    //在镜子之间传送
    private void Transfer()
    {

    }

    private void InitSpriteShape()
    {
        // 获取 Spline 对象
        Spline spline = parameter.check.spriteShapeController.spline;
        // 清空现有的控制点
        spline.Clear();

        // 插入起点（第一个物体的位置）
        spline.InsertPointAt(0,Vector3.zero);

        // 插入终点（第二个物体的位置）
        spline.InsertPointAt(1, Vector3.zero);

        // 确保两个顶点是直线连接
        spline.SetTangentMode(0, ShapeTangentMode.Linear);
        spline.SetTangentMode(1, ShapeTangentMode.Linear);
    }

    private void UpdateSpriteShape()
    {

        Spline spline = parameter.check.spriteShapeController.spline;
        parameter.check.spriteShapeController.gameObject.SetActive(true);
        print( "第一个位置"+parameter.check.spriteShapeController.transform.InverseTransformPoint(parameter.mirrors[1].transform.position));
        Vector3 position1 = parameter.check.spriteShapeController.transform.InverseTransformPoint(parameter.mirrors[1].transform.position);
        position1.z = -3;
        spline.SetPosition(0, position1);
        spline.SetHeight(0, Random.Range(3f, 4f));

        int lastIndex = spline.GetPointCount() - 1;
        print("第二个位置" + parameter.check.spriteShapeController.transform.InverseTransformPoint(parameter.mirrors[2].transform.position));
        Vector3 position2 = parameter.check.spriteShapeController.transform.InverseTransformPoint(parameter.mirrors[2].transform.position);
        position2.z = -3;
        spline.SetPosition(lastIndex, position2);
        spline.SetHeight(lastIndex, Random.Range(3f, 4f));
        
        parameter.check.spriteShapeController.BakeMesh();
    }
    
    //玩家受到攻击
    private void Hit()
    {
        parameter.isHit = true;
    }

    private void Dead()
    {
        print(Level.Instance.nowLevelObj.name);
        if (Level.Instance.nowLevelObj.name == "level0-1") //todo：一个很不优雅的特殊标记，因为策划的需求是第一关要教玩家基础操作，不引入人质。
        {
            return;
        }
        EventCenter.Instance.EventTrigger(E_EventType.E_RealFail);
        parameter.isDead = true;
    }

}
