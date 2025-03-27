using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型 枚举
/// 所有需要注册的事件都要在这里写上，并用注释标注参数类型，便于编写代码悬停的时候查看。
/// </summary>
public enum E_EventType 
{

    /// <summary>
    /// 场景切换时的进度变化
    /// 参数：float，加载进度
    /// </summary>
    E_SceneLoadChange,

    /// <summary>
    /// 水平热键 -1~1的事件监听
    /// </summary>
    E_Input_Horizontal,

    /// <summary>
    /// 竖直热键 -1~1的事件监听
    /// </summary>
    E_Input_Vertical,

    /// <summary>
    /// 提示事件
    /// 参数：string，用于填充提示信息
    /// </summary>
    E_Tip,

    /// <summary>
    /// 专门用于需要堵塞主线程、进行耗时等待时的情况。期间展示加载动画。
    /// </summary>
    E_TimeBlocking,

    /// <summary>
    /// 按下移动摇杆
    /// 参数：PointerEventData，摇杆位置
    /// </summary>
    E_PressMoveButton,

    /// <summary>
    /// 抬起移动按键/摇杆
    /// </summary>
    E_UpMoveButton,


    /// <summary>
    /// 按下跳跃按钮
    /// </summary>
    E_PressJumpButton,

    /// <summary>
    /// 按下冲刺按钮
    /// </summary>
    E_PressDashButton,

    /// <summary>
    /// 抬起冲刺按钮
    /// </summary>
    E_UpDashButton,

    /// <summary>
    /// 按下摇杆
    /// 参数：int，摇杆编号
    /// </summary>
    E_PressJoystick,

    /// <summary>
    /// 拖动摇杆
    /// 参数：int，摇杆编号；PointerEventData，摇杆位置
    /// </summary>
    E_DragJoyStick,

    /// <summary>
    /// 抬起摇杆
    /// 参数：int，摇杆编号
    /// </summary>
    E_UpJoyStick,

    /// <summary>
    /// 按下镜子方向切换按钮
    /// 参数：int，按钮编号
    /// </summary>
    E_PressMirrorOrientationButton,

    /// <summary>
    /// 按下镜子放置按钮，
    /// 参数：int，镜子编号
    /// </summary>
    E_PressMirrorSetButton,

    /// <summary>
    /// 按下镜子模式切换按钮
    /// </summary>
    E_PressChangeMirrorModeButton,

    /// <summary>
    /// 退出镜子模式切换状态
    /// </summary>
    E_OutMirrorModeChange,

    /// <summary>
    /// 退出玩家模式切换状态
    /// </summary>
    E_OutPlayerModeChange,


    /// <summary>
    /// 按下玩家模式切换按钮
    /// </summary>
    E_PressChangePlayerModeButton,

    /// <summary>
    /// 接近放置镜子的位置
    /// 参数：int,包含要显示按钮的镜子编号
    /// </summary>
    E_CloseMirrorPlace,

    /// <summary>
    /// 进入镜子放置交互状态
    /// 参数：int，镜子编号
    /// </summary>
    E_EnterMirrorInteraction,

    /// <summary>
    /// 离开镜子交互区域
    /// </summary>
    E_LeaveMirrorPlace,

    /// <summary>
    /// 进入控制镜子状态
    /// 参数：int，要控制的镜子编号
    /// </summary>
    E_ControlMirror,

    
    /// <summary>
    /// 有镜子需要回收
    /// 参数：int，要控制的镜子编号
    /// </summary>
    E_EnterMirrorRecycle,

    /// <summary>
    /// 隐藏镜子回收按键
    /// 参数：int，要回收的镜子编号
    /// </summary>
    E_OutMirrorRecycle,

    /// <summary>
    /// 按下镜子回收按钮
    /// 参数：int，要回收的镜子编号
    /// </summary>
    E_PressMirrorRecycleButton,

    /// <summary>
    /// 玩家受击
    /// </summary>
    E_PlayerAttacked,

    /// <summary>
    /// 玩家血量减少。辅助进行UI显示的方法
    /// 参数：int，正在减少第x格血
    /// </summary>
    E_PlayerHPDecreasing,

    /// <summary>
    /// 剧情交互
    /// </summary>
    E_TalkWithOther,

    /// <summary>
    /// 进入关卡
    /// </summary>
    E_EnterLevel,

    /// <summary>
    /// 关卡失败
    /// </summary>
    E_FailLevel,

    /// <summary>
    /// 关卡成功
    /// </summary>
    E_SuccessLevel,

    /// <summary>
    /// 整体通关
    /// </summary>
    E_AllSuccessLevel,

    /// <summary>
    /// 接近对话处
    /// </summary>
    E_CloseTalk,

    /// <summary>
    /// 离开对话处
    /// </summary>
    E_LeaveTalk,

    /// <summary>
    /// 真正的失败（为了教程关卡的特殊情况而设置）
    /// </summary>
    E_RealFail,
    
    /// <summary>
    /// 达到Npc拯救的要求
    /// </summary>
    E_isValid2SaveNPC

}
