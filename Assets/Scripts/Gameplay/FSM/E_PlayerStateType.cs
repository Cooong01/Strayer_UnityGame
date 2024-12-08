using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_PlayerStateType
{
    Idle, //空闲
    Move, //移动
    Dash, //冲锋
    Hit, //受击
    Death,//死亡
    Jump,//跳跃
    Fall, //下落
    Land,//落地
    ChangePlayerMode,//切换玩家形态

    //以下是镜子相关操作
    Mirror_ChangeMirrorMode,//切换镜子操控模式
    Mirror_CloseMirrorPlace,//靠近镜子操控区域 1
    Mirror_ControlMirror,//控制镜子 1
    Mirror_HoldMirror,//拿起镜子
    Mirror_RecycleMirror,//收回镜子
    Mirror_transfer,//在镜子之间传送

    //转移关卡的操作
    enterNextLevel,
}
